using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure
{
    public class KEYStoreStatus
    {
        public static string CREATED {get { return "CREATED"; } }
        public static string MISSING { get { return "MISSING"; } }
        public string Manufacturer { get; set; }
        public string Enviroment { get; set; }
        public string Managed { get; set; }
        public string StoreStatus { get; set; }  
        public string Creation { get; set; }

        public string FilePath { get; set; }
        public string RequestStatus { get; set; }

        public string RequestCreation { get; set; }

        public string CertificateStatus { get; set; }
        public string CertificateImport { get; set; }
    }
}
