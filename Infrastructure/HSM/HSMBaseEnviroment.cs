using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.HSM
{
    public class HSMBaseEnviroment
    {
        public Dictionary<string, HSMStatusInfo> HSMStatus = new Dictionary<string, HSMStatusInfo>();
        static bool ENV_SET = false;
        protected static string[] Slots;
        protected ConfigurationAccess cfa = new ConfigurationAccess();
        public static string default_token_name = "ATM_SOFTWARE_SIGNING";
        public string TokenPwd;
        public static bool Initialized { get; set; }
        public static string EnviromentIPAddressString
        {
            get;
            set;
        }
        public int GetSlot()
        {
            var s = HSMStatus.Values.Where(c => c.Connection == "CONNECTED").First();
            return s.Slot;
        }
        

        
        
        private void Setup()
        {
            ConfigurationAccess cfa = new ConfigurationAccess();
            cfa.SetRegistry(@"SOFTWARE\SafeNet\PTKC\GENERAL", "ET_PTKC_GENERAL_LIBRARY_MODE", "NORMAL");
            string adr=cfa.AccessKey(cfa.CK_QA_IP_ADR);
            if (string.IsNullOrEmpty(adr) == true)
                return;//use the address already set in the registry
            cfa.SetRegistry(@"SOFTWARE\SafeNet\HSM\NETCLIENT", "ET_HSM_NETCLIENT_SERVERLIST", adr);
            SetEnvVariable(adr);
        }
        private void SetEnvVariable(string adr) 
        {
            //Environment.SetEnvironmentVariable("ET_HSM_NETCLIENT_SERVERLIST", adr, EnvironmentVariableTarget.Process);
            //Environment.SetEnvironmentVariable("ET_PTKC_GENERAL_LIBRARY_MODE", "NORMAL", EnvironmentVariableTarget.Process);
        }

        protected  void SetIPEnviroment()
        {
            if (ENV_SET == true)
                return;            
            Setup();
            //int r = InteropHSM.Initialize();
            //if (r == 0)
            //    Initialized = true;
            //else
            //{
            //    Initialized = false;
            //    return;
            //}
            Initialized=true;
            string tn = cfa.AccessKey(cfa.CK_TOKEN_NAME);
            if (string.IsNullOrEmpty(tn) == true)
                tn = HSMBaseEnviroment.default_token_name;            
            ENV_SET = true;
        }
        protected void IpStatus(string ak,string default_ip_adr)
        {            
            HSMStatus.Clear();
            string ip_adr = cfa.AccessKey(ak);
            if (string.IsNullOrEmpty(ip_adr) == true)
                ip_adr = default_ip_adr;
            char[] del = { ' ' };
            string[] adrs = ip_adr.Split(del);
            foreach (string adr in adrs)
            {
                HSMStatusInfo info = new HSMStatusInfo();
                info.Slot = -1;
                info.IPAdr = adr;
                info.Connection = "CONNECTED";                
                HSMStatus.Add(adr, info);
            }
        }
       
        
    }
}
