using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArcGIS.Server.Rest.Classes
{

    public class Services
    {
        public string folderName { get; set; }
        public string description { get; set; }
        public bool webEncrypted { get; set; }
        public bool isDefault { get; set; }
        public List<Service> services { get; set; }
    }

    public class ServicesRoot : Services
    {
        public string[] folders { get; set; }
        public Foldersdetail[] foldersDetail { get; set; }
    }

    public class Foldersdetail
    {
        public string folderName { get; set; }
        public string description { get; set; }
        public bool webEncrypted { get; set; }
        public bool isDefault { get; set; }
    }

    public class Service
    {
        public string folderName { get; set; }
        public string serviceName { get; set; }
        public string type { get; set; }
        public string description { get; set; }
    }

}
