//-----------------------------------------------------------------------
// <copyright file="CsvExport.cs" company="Studio A&T s.r.l.">
//     Copyright (c) Studio A&T s.r.l. All rights reserved.
// </copyright>
// <author>Nicogis</author>
//-----------------------------------------------------------------------
namespace Studioat.ArcGISServer.UsageReports
{
    using System;
    using System.Collections.Generic;
    using System.Data.SqlTypes;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Web;

    /// <summary>
    /// Simple CSV export
    /// </summary>
    /// <example>
    /// CsvExport myExport = new CsvExport();
    /// myExport.AddRow();
    /// myExport["Region"] = "New York, USA";
    /// myExport["Sales"] = 100000;
    /// myExport["Date Opened"] = new DateTime(2003, 12, 31);
    /// myExport.AddRow();
    /// myExport["Region"] = "Sydney \"in\" Australia";
    /// myExport["Sales"] = 50000;
    /// myExport["Date Opened"] = new DateTime(2005, 1, 1, 9, 30, 0);
    /// Then you can do any of the following three output options:
    /// string myCsv = myExport.Export();
    /// myExport.ExportToFile("file.csv");
    /// byte[] myCsvData = myExport.ExportToBytes();
    /// </example>
    public class CsvExport
    {
        /// <summary>
        /// To keep the ordered list of column names
        /// </summary>
        private List<string> fields = new List<string>();

        /// <summary>
        /// The list of rows
        /// </summary>
        private List<Dictionary<string, object>> rows = new List<Dictionary<string, object>>();

        /// <summary>
        /// Initializes a new instance of the <see cref="CsvExport" /> class
        /// </summary>
        public CsvExport()
        {
            this.Delimiter = ",";
            this.Header = true;
        }

        /// <summary>
        /// Gets or sets the separator of csv
        /// </summary>
        public string Delimiter
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether header in csv
        /// </summary>
        public bool Header
        {
            get;
            set;
        }

        /// <summary>
        /// Gets The current row
        /// </summary>
        private Dictionary<string, object> CurrentRow
        {
            get
            {
                return this.rows[this.rows.Count - 1];
            }
        }

        /// <summary>
        /// Set a value on this column
        /// </summary>
        /// <param name="field">name of field</param>
        /// <returns>value of column</returns>
        public object this[string field]
        {
            set
            {
                // Keep track of the field names, because the dictionary loses the ordering
                if (!this.fields.Contains(field))
                {
                    this.fields.Add(field);
                }

                this.CurrentRow[field] = value;
            }
        }

        /// <summary>
        /// Append an object csvExport in a current csvExport
        /// </summary>
        /// <param name="csvExport">object csvExport</param>
        public void Append(CsvExport csvExport)
        {
            this.rows.AddRange(csvExport.rows);
        }

        /// <summary>
        /// Call this before setting any fields on a row
        /// </summary>
        public void AddRow()
        {
            this.rows.Add(new Dictionary<string, object>());
        }

        /// <summary>
        /// Add a list of typed objects, maps object properties to csvFields
        /// </summary>
        /// <typeparam name="T">type of values</typeparam>
        /// <param name="list">list of values</param>
        public void AddRows<T>(IEnumerable<T> list)
        {
            if (list.Any())
            {
                foreach (var obj in list)
                {
                    this.AddRow();
                    var values = obj.GetType().GetProperties();
                    foreach (var value in values)
                    {
                        this[value.Name] = value.GetValue(obj, null);
                    }
                }
            }
        }

        /// <summary>
        /// Output all rows as a CSV returning a string
        /// </summary>
        /// <returns>string of csv</returns>
        public string Export()
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("sep=" + this.Delimiter);

            if (this.Header)
            {
                // The header
                sb.Append(string.Join(this.Delimiter, this.fields.ToArray()));
                sb.AppendLine();
            }

            // The rows
            foreach (Dictionary<string, object> row in this.rows)
            {
                this.fields.Where(f => !row.ContainsKey(f)).ToList().ForEach(k =>
                        {
                            row[k] = null;
                        });
                sb.Append(string.Join(this.Delimiter, this.fields.Select(field => this.MakeValueCsvFriendly(row[field])).ToArray()));
                sb.AppendLine();
            }

            return sb.ToString();
        }

        /// <summary>
        /// Exports to a file
        /// </summary>
        /// <param name="path">path store the file</param>
        public void ExportToFile(string path)
        {
            File.WriteAllText(path, this.Export());
        }

        /// <summary>
        /// Exports as raw UTF8 bytes
        /// </summary>
        /// <returns>export in bytes</returns>
        public byte[] ExportToBytes()
        {
            var data = Encoding.UTF8.GetBytes(this.Export());
            return Encoding.UTF8.GetPreamble().Concat(data).ToArray();
        }

        /// <summary>
        /// Converts a value to how it should output in a csv file
        /// </summary>
        /// <param name="value">convert value</param>
        /// <example>
        /// If it has a comma, it needs surrounding with double quotes
        /// Example Sydney, Australia -> "Sydney, Australia"
        /// Also if it contains any double quotes ("), then they need to be replaced with quad quotes[sic] ("")
        /// Example "Dangerous Dan" McGrew -> """Dangerous Dan"" McGrew"
        /// </example>
        /// <returns>string of value friendly</returns>
        private string MakeValueCsvFriendly(object value)
        {
            if (value == null)
            {
                return string.Empty;
            }

            if (value is INullable && ((INullable)value).IsNull)
            {
                return string.Empty;
            }

            if (value is DateTime)
            {
                ////if (((DateTime)value).TimeOfDay.TotalSeconds == 0)
                ////    return ((DateTime)value).ToString("yyyy-MM-dd");
                return ((DateTime)value).ToString("yyyy-MM-dd HH:mm:ss");
            }

            string output = value.ToString();
            if (output.Contains(",") || output.Contains("\"") || output.Contains("\n") || output.Contains("\r"))
            {
                output = '"' + output.Replace("\"", "\"\"") + '"';
            }

            // cropping value for stupid Excel
            if (output.Length > 30000)
            {
                if (output.EndsWith("\""))
                {
                    output = output.Substring(0, 30000) + "\"";
                }
                else
                {
                    output = output.Substring(0, 30000);
                }
            }

            return output.Length <= 32767 ? output : output.Substring(0, 32767);
        }
    }
}
