//-----------------------------------------------------------------------
// <copyright file="Program.cs" company="Studio A&T s.r.l.">
//     Copyright (c) Studio A&T s.r.l. All rights reserved.
// </copyright>
// <author>Nicogis</author>
//-----------------------------------------------------------------------
namespace Studioat.ArcGISServer.UsageReports
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Text;
    using System.Threading.Tasks;
    using ArcGIS.Server.Rest;
    using ArcGIS.Server.Rest.Classes;
    using CommandLine;
    using OfficeOpenXml;
    using Newtonsoft.Json.Linq;

    /// <summary>
    /// class main
    /// </summary>
    public class Program
    {
        /// <summary>
        /// path and filename of error log
        /// </summary>
        private static string pathFileNameLog = null;

        /// <summary>
        /// name of file
        /// </summary>
        private static string nameFile = null;

        /// <summary>
        /// list of metrics of usage reports
        /// </summary>
        private enum Metrics
        {
            /// <summary>
            /// Request Count
            /// </summary>
            RequestCount,

            /// <summary>
            /// Requests Failed
            /// </summary>
            RequestsFailed,

            /// <summary>
            /// Requests Timed Out
            /// </summary>
            RequestsTimedOut,

            /// <summary>
            /// Request Max Response Time
            /// </summary>
            RequestMaxResponseTime,

            /// <summary>
            /// Request Avg Response Time
            /// </summary>
            RequestAvgResponseTime,

            /// <summary>
            /// Service Active Instances
            /// </summary>
            ///ServiceActiveInstances -> help wrong
        }

        private enum Metrics2
        {
            /// <summary>
            /// Service Running Instances Max (if this metric is with metrics give data wrong)
            /// </summary>
            ServiceRunningInstancesMax
        }
        /// <summary>
        /// extension dei file
        /// </summary>
        private enum ExtensionFile
        {
            /// <summary>
            /// comma-separated values file
            /// </summary>
            csv,

            /// <summary>
            /// txt file
            /// </summary>
            txt,

            /// <summary>
            /// Excel File
            /// </summary>
            xlsx
        }

        /// <summary>
        /// field in Header
        /// </summary>
        private enum Header
        {
            /// <summary>
            /// name of host
            /// </summary>
            Host,

            /// <summary>
            /// name of service
            /// </summary>
            Service,

            /// <summary>
            /// date time
            /// </summary>
            Time
        }

        /// <summary>
        /// method entry point
        /// </summary>
        /// <param name="args">list of arguments from console</param>
        public static void Main(string[] args)
        {
            Task.Run(async () =>
            {
                await Program.DoWork(args);
            }).Wait();
        }

        /// <summary>
        /// do work
        /// </summary>
        /// <param name="args">list of arguments from console</param>
        /// <returns>object task</returns>
        private static async Task DoWork(string[] args)
        {
            try
            {
                string path = new Uri(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().CodeBase)).LocalPath;

                string directoryCurrent = Path.GetDirectoryName(path);

                nameFile = string.Format("log_{1}_{0:yyyy-MM-dd_HH-mm-ss}", DateTime.Now, AssemblyInfo.Product.Replace(" ", "_"));

                pathFileNameLog = Path.ChangeExtension(Path.Combine(directoryCurrent, nameFile), Enum.GetName(typeof(ExtensionFile), ExtensionFile.txt));

                ServicePointManager.ServerCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true;

                var options = new Options();

                Parser parser = new Parser();

                if (args.Length == 0 || args[0] == "-h" || args[0].Trim().ToLowerInvariant() == "help")
                {
                    Console.Out.WriteLine(options.GetUsage());
                    Console.WriteLine("Press any key to continue ...");
                    Console.ReadKey();
                    return;
                }

                if (parser.ParseArguments(args, options))
                {
                    string directoryOutput = options.Directory;

                    if (!Directory.Exists(directoryOutput))
                    {
                        File.AppendAllText(pathFileNameLog, string.Format("Folder output '{0}' doesn't exist!", directoryOutput));
                        return;
                    }

                    pathFileNameLog = Path.ChangeExtension(Path.Combine(directoryOutput, nameFile), Enum.GetName(typeof(ExtensionFile), ExtensionFile.txt));

                    string pathFileName = Path.ChangeExtension(Path.Combine(directoryOutput, nameFile), Enum.GetName(typeof(ExtensionFile), ExtensionFile.xlsx));

                    string[] servers = options.Servers.Select(s =>
                    {
                        if (!s.EndsWith("/"))
                        {
                            s += "/";
                        }

                        s += "admin/";

                        return s;
                    }).ToArray();

                    string[] u = options.Users.ToArray();
                    string[] p = options.Passwords.ToArray();

                    long? fromUnix = null;
                    long? toUnix = null;

                    if (options.Since == SinceType.CUSTOM)
                    {
                        if ((options.From == null) || (options.To == null))
                        {
                            File.AppendAllText(pathFileNameLog, options.GetUsageError());
                            return;
                        }

                        try
                        {
                            DateTime from = DateTime.ParseExact(options.From, "dd-MM-yyyy-HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);

                            DateTime to = DateTime.ParseExact(options.To, "dd-MM-yyyy-HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);

                            fromUnix = EncodingHelper.GetUnixTimestampMillis(from);
                            toUnix = EncodingHelper.GetUnixTimestampMillis(to);
                        }
                        catch
                        {
                            File.AppendAllText(pathFileNameLog, options.GetUsageError());
                            return;
                        }
                    }

                    List<Task<List<ReportData>>> taskList = new List<Task<List<ReportData>>>();

                    for (int i = 0; i < servers.Length; i++)
                    {
                        taskList.Add(Report(servers[i], u[i], p[i], options, fromUnix, toUnix));
                    }

                    await Task.WhenAll(taskList.ToArray());

                    List<ReportData> r = taskList.SelectMany(x => x.Result).ToList();

                    FileInfo fileInfoOutput = new FileInfo(pathFileName);
                    using (ExcelPackage pck = new ExcelPackage(fileInfoOutput))
                    {
                        //Create the worksheet
                        ExcelWorksheet ws = pck.Workbook.Worksheets.Add(string.Format("{0:yyyy-MM-dd_HH-mm-ss}", DateTime.Now));

                        var dataRange = ws.Cells["A1"].LoadFromCollection
                            (
                            from s in r
                            select new { HostName = s.HostName,
                                ServiceName = s.NameService,
                                Time = s.Time,
                                RequestCount = s.RequestCount,
                                RequestsFailed = s.RequestsFailed,
                                RequestsTimedOut = s.RequestsTimedOut,
                                RequestMaxResponseTime = s.RequestMaxResponseTime,
                                RequestAvgResponseTime = s.RequestAvgResponseTime,
                                ServiceRunningInstancesMax = s.ServiceRunningInstancesMax
                            }
                            ,
                           true, OfficeOpenXml.Table.TableStyles.Dark1);

                        ws.Cells[2, 3, dataRange.End.Row, 3].Style.Numberformat.Format = "m/dd/yyyy hh:mm:ss AM/PM";
                        

                        dataRange.AutoFitColumns();

                        pck.Save();
                    }

                }
                else
                {
                    File.AppendAllText(pathFileNameLog, options.GetUsageError());
                }
            }
            catch (AggregateException exception)
            {
                StringBuilder sb = new StringBuilder();
                foreach (Exception ex in exception.Flatten().InnerExceptions)
                {
                    sb.AppendFormat(string.Format("Error: Message[{0}]\r\nSource[{1}]\r\nTrace[{2}]", ex.Message, ex.Source, ex.StackTrace)).AppendLine();
                }

                File.AppendAllText(pathFileNameLog, sb.ToString());
            }

            ////Console.Out.WriteLine("Press enter to continue.");
            ////Console.ReadLine();
        }

        /// <summary>
        /// task that generate csvExport object
        /// </summary>
        /// <param name="url">url of arcgis server</param>
        /// <param name="user">user of admin</param>
        /// <param name="pwd">password of admin</param>
        /// <param name="options">object options</param>
        /// <param name="fromUnix">from of time</param>
        /// <param name="toUnix">to of time</param>
        /// <returns>csvExport object</returns>
        private static async Task<List<ReportData>> Report(string url, string user, string pwd, Options options, long? fromUnix, long? toUnix)
        {
            string urlServer = url;
            Uri serverUri = new Uri(urlServer);
            string host = serverUri.Host;

            var ags = new AGSClient(url, user, pwd);

            await ags.Authenticate();

            ////Console.Out.WriteLine("Authenticated against {0}: {1}", ags.ServerUrl, ags.IsAuthenticated);
            ////Console.Out.WriteLine("Session expires at {0}", ags.TokenExpiration.ToLocalTime());
            ////Console.Out.WriteLine("------------------");

            List<string> services = await ags.ListServices(options.IncludeSystemFolders);

            string usageReport1 = Guid.NewGuid().ToString();
            string[] metrics = Enum.GetNames(typeof(Metrics));
            Task<JObject> addUsageReport1 = ags.AddUsageReport(usageReport1, services, options, fromUnix, toUnix, metrics);

            string usageReport2 = Guid.NewGuid().ToString();
            string[] metrics2 = Enum.GetNames(typeof(Metrics2));
            Task<JObject> addUsageReport2 = ags.AddUsageReport(usageReport2, services, options, fromUnix, toUnix, metrics2);

            Task.WaitAll(new Task<JObject>[] { addUsageReport1, addUsageReport2 });

            Task<ReportResponse> queryUsageReport1 =  ags.QueryUsageReport(usageReport1);
            Task<ReportResponse> queryUsageReport2 =  ags.QueryUsageReport(usageReport2);

            Task.WaitAll(new Task<ReportResponse>[] { queryUsageReport1, queryUsageReport2 });

            ArcGIS.Server.Rest.Classes.ReportData[] reportDatas1 = queryUsageReport1.Result.report.reportdata[0]; // one filter machine '*' so get [0]
            ArcGIS.Server.Rest.Classes.ReportData[] reportDatas2 = queryUsageReport2.Result.report.reportdata[0]; // one filter machine '*' so get [0]

            long[] time = queryUsageReport1.Result.report.timeslices;
            long numElement = time.LongCount();

            string[] header = Enum.GetNames(typeof(Header));

            List<ReportData> results = new List<ReportData>();
            int cont1 = 0;
            int cont2 = 0;
            while (cont1 < reportDatas1.LongLength)
            {
                string nameService = reportDatas1[cont1].resourceURI;

                long?[] requestCount = reportDatas1[cont1++].data.Cast<long?>().ToArray();
                
                long?[] requestsFailed = reportDatas1[cont1++].data.Cast<long?>().ToArray();
                
                long?[] requestsTimedOut = reportDatas1[cont1++].data.Cast<long?>().ToArray();
                
                double?[] requestMaxResponseTime = reportDatas1[cont1++].data.Cast<double?>().ToArray();
                
                double?[] requestAvgResponseTime = reportDatas1[cont1++].data.Cast<double?>().ToArray();

                long?[] serviceRunningInstancesMax = reportDatas2[cont2++].data.Cast<long?>().ToArray();
                
                for (long j = 0; j < numElement; j++)
                {
                    ReportData reportData = new ReportData();
                    reportData.HostName = host;
                    reportData.NameService = nameService;
                    reportData.Time = EncodingHelper.DateTimeFromUnixTimestampMillis(time[j]);
                    reportData.RequestCount = requestCount[j];
                    reportData.RequestsFailed = requestsFailed[j];
                    reportData.RequestsTimedOut = requestsTimedOut[j];
                    reportData.RequestMaxResponseTime = requestMaxResponseTime[j];
                    reportData.RequestAvgResponseTime = requestAvgResponseTime[j];
                    reportData.ServiceRunningInstancesMax = serviceRunningInstancesMax[j];
                    results.Add(reportData);
                }

            }

            Task<JObject> deleteUsageReport1 = ags.DeleteUsageReport(usageReport1);
            Task<JObject> deleteUsageReport2 = ags.DeleteUsageReport(usageReport2);

            Task.WaitAll(new Task<JObject>[] { deleteUsageReport1, deleteUsageReport2 });

            return results;
        }
    }
}
