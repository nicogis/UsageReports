using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArcGIS.Server.Rest.Classes
{
    public class UsageReport
    {
        public string reportname { get; set; }
        public SinceType since { get; set; }
        public long from { get; set; }
        public long to { get; set; }
        public int  aggregationInterval {get; set;}
        public Query[] queries { get; set; }

        //public string metadata { get; set; }

    }

    public class Query
    {
        public string[] resourceURIs { get; set; }
        public string[] metrics { get; set; }
    }




    [JsonConverter(typeof(Newtonsoft.Json.Converters.StringEnumConverter))]
    public enum SinceType
    {
        LAST_DAY,
        LAST_WEEK,
        LAST_MONTH,
        CUSTOM
    }
}
