//-----------------------------------------------------------------------
// <copyright file="Options.cs" company="Studio A&T s.r.l.">
//     Copyright (c) Studio A&T s.r.l. All rights reserved.
// </copyright>
// <author>Nicogis</author>
//-----------------------------------------------------------------------
namespace Studioat.ArcGISServer.UsageReports
{
    using System.Collections.Generic;
    using ArcGIS.Server.Rest.Classes;
    using CommandLine;
    using CommandLine.Text;
    
    /// <summary>
    /// class args parameter
    /// </summary>
    internal class Options
    {
        /// <summary>
        /// Gets or sets servers
        /// </summary>
        [OptionList('s', "servers", Separator = ';', Required = true, MutuallyExclusiveSet = "usageReports", HelpText = "List of the urls using delimitator ';' (for instance: http://server1:6080/arcgis;http://server2:6080/arcgis ...")]
        public IList<string> Servers
        { 
            get; 
            set; 
        }

        /// <summary>
        /// Gets or sets users
        /// </summary>
        [OptionList('u', "users", Separator = ';', Required = true, MutuallyExclusiveSet = "usageReports", HelpText = "List of the admins using delimitator ';' (for instance: siteadmin;siteadmin ...")]
        public IList<string> Users
        { 
            get; 
            set; 
        }

        /// <summary>
        /// Gets or sets passwords
        /// </summary>
        [OptionList('p', "pwds", Separator = ';', Required = true, MutuallyExclusiveSet = "usageReports", HelpText = "List of the passwords of the admins using delimitator ';' (for instance: mypwd1;mypwd2 ...")]
        public IList<string> Passwords
        { 
            get; 
            set; 
        }

        /// <summary>
        /// Gets or sets output
        /// </summary>
        [Option('o', "output", Required = true, MutuallyExclusiveSet = "usageReports", HelpText = "Folder output files")]
        public string Directory
        { 
            get; 
            set; 
        }

        /// <summary>
        /// Gets or sets interval
        /// </summary>
        [Option('a', "interval", Required = false, MutuallyExclusiveSet = "usageReports", HelpText = "Server metrics are summarized and returned for time slices using the specified interval in minutes")]
        public int? AggregationInterval
        { 
            get; 
            set; 
        }

        /// <summary>
        /// Gets or sets sinceType
        /// </summary>
        [Option('c', "sinceType", Required = false, DefaultValue = SinceType.LAST_DAY, MutuallyExclusiveSet = "usageReports", HelpText = "The time duration for the report. Values: LAST_DAY, LAST_WEEK, LAST_MONTH, CUSTOM")]
        public SinceType Since
        { 
            get; 
            set; 
        }

        /// <summary>
        /// Gets or sets from of interval
        /// </summary>
        [Option('f', "from", Required = false, MutuallyExclusiveSet = "usageReports", HelpText = "For the beginning period of the report dd-MM-yyyy-HH:mm:ss. Used when since parameter is CUSTOM")]
        public string From
        {
            get; 
            set; 
        }

        /// <summary>
        /// Gets or sets delimiter
        /// </summary>
        [Option('d', "delimiter", Required = false, MutuallyExclusiveSet = "usageReports", HelpText = "Delimiter character (csv)")]
        public string Delimiter
        { 
            get; 
            set; 
        }

        /// <summary>
        /// Gets or sets to of interval
        /// </summary>
        [Option('t', "to", Required = false, MutuallyExclusiveSet = "usageReports", HelpText = "For the ending period of the report dd-MM-yyyy-HH:mm:ss. Used when since parameter is CUSTOM")]
        public string To
        { 
            get; 
            set; 
        }

        /// <summary>
        /// Gets or sets LastParserState
        /// </summary>
        [ParserState]
        public IParserState LastParserState
        { 
            get; 
            set; 
        }

        /// <summary>
        /// help of application
        /// </summary>
        /// <returns>string of help</returns>
        [HelpOption]
        public string GetUsage()
        {
            return HelpText.AutoBuild(this, (HelpText current) => HelpText.DefaultParsingErrorsHandler(this, current));
        }

        /// <summary>
        /// message of errors
        /// </summary>
        /// <returns>string of message of errors</returns>
        public string GetUsageError()
        {
            var help = new HelpText();
            if (this.LastParserState.Errors.Count > 0)
            {
                var errors = help.RenderParsingErrorsText(this, 2); // indent with two spaces
                if (!string.IsNullOrEmpty(errors))
                {
                    help.AddPreOptionsLine(string.Concat("\n", "Error(s):"));
                    help.AddPreOptionsLine(errors);
                }
            }

            return help;
        }
    }
}
