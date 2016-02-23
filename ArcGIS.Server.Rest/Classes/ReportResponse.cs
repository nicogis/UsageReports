using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArcGIS.Server.Rest.Classes
{

        public class ReportResponse
        {
            public Report report { get; set; }
        }

        public class Report
        {
            public string reportname { get; set; }
            public object metadata { get; set; }
            [JsonProperty("time-slices")]
            public long[] timeslices { get; set; }
            [JsonProperty("report-data")]
            public ReportData[][] reportdata { get; set; }
        }

        public class ReportData
        {
            public string resourceURI { get; set; }
            [JsonProperty("metric-type")]
            public string metrictype { get; set; }
            public object[] data { get; set; }
        }
    
}


