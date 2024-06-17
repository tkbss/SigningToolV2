using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TracingModule
{
    public class TraceData
    {
        public System.DateTime SessionDate { get; set; }

        public string Prename { get; set; }

        public string Name { get; set; }

        public string Operation { get; set; }

        public string Enviroment { get; set; }

        public string StoreType { get; set; }

        public string Signer_Type { get; set; }

        public string Signer { get; set; }

        public string Operation_Result { get; set; }
        public string ErrorMessage { get; set; }

        public string SessionId { get; set; }
    }
}
