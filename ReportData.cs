namespace Studioat.ArcGISServer.UsageReports
{
    using System;
    using System.ComponentModel;

    public class ReportData
    {
        
        public string HostName;

        
        public string NameService;

        
        public DateTime Time;

        
        public long? RequestCount;

        
        public long? RequestsFailed;

        
        public long? RequestsTimedOut;

        
        public double? RequestMaxResponseTime;

        
        public double? RequestAvgResponseTime;

        
        public long? ServiceActiveInstances;
    }
}
