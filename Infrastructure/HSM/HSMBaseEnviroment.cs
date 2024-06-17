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
        protected string TokenPwd;
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
        

        public bool ProbeIPAdr(string ip_adr)
        {
            
            //IPAddress ip = IPAddress.Parse(ip_adr);
            bool res = false;
            try
            {
                //TcpClient client = new TcpClient(ip_adr, 12396);
                //if (!client.ConnectAsync(ip, 12396).Wait(500))
                //{
                //    // connection failure
                //    res = false;

                //}
                //else
                //{
                //    res = true;
                //}

                //client.Close();
                return true;
            }
            catch
            {
                //return false;
                return true;
            }
            //return res;
        }
        
        private void Setup()
        {
            SetRegistry(@"SOFTWARE\SafeNet\HSM\NETCLIENT", "ET_HSM_NETCLIENT_SERVERLIST", "10.153.82.10");
            SetRegistry(@"SOFTWARE\SafeNet\PTKC\GENERAL", "ET_PTKC_GENERAL_LIBRARY_MODE", "NORMAL");
            //Environment.SetEnvironmentVariable("ET_HSM_NETCLIENT_SERVERLIST", "10.153.82.10", EnvironmentVariableTarget.Process);
            //Environment.SetEnvironmentVariable("ET_PTKC_GENERAL_LIBRARY_MODE", "NORMAL", EnvironmentVariableTarget.Process);
        }

        private  void SetRegistry(string regKey, string parameter, string value)
        {
            var key = Registry.CurrentUser.OpenSubKey(regKey, true);
            if (key == null)
            {
                key = Registry.CurrentUser.CreateSubKey(regKey);
                if (key == null)
                {
                    throw new InvalidOperationException($"Creating registry key {regKey} failed");
                }
            }

            key.SetValue(parameter, value);
        }
        
        protected  void SetIPEnviroment()
        {
            if (ENV_SET == true)
                return;
            //HSMBaseEnviroment e = new HSMBaseEnviroment();
            //string ip_adr = HSMTestEnviroment.default_atm_test_ip;// + " " + HSMProdEnviroment.default_atm_prod_ip;
            //char[] del = { ' ' };
            //string[] adrs = ip_adr.Split(del);
            //EnviromentIPAddressString = string.Empty;
            //foreach (string adr in adrs)
            //{
            //    if (ProbeIPAdr(adr) == true)
            //    {
            //        EnviromentIPAddressString += adr;
            //        EnviromentIPAddressString += " ";
            //    }
            //}
            //EnviromentIPAddressString = EnviromentIPAddressString.Trim();
            //Environment.SetEnvironmentVariable("ET_HSM_NETCLIENT_SERVERLIST", EnviromentIPAddressString);
            //Environment.SetEnvironmentVariable("ET_PTKC_GENERAL_LIBRARY_MODE", "NORMAL");            
            //if(string.IsNullOrEmpty(EnviromentIPAddressString)==true)
            //{
            //    Initialized = false;
            //    return;
            //}
            Setup();
            int r = InteropHSM.Initialize();
            if (r == 0)
                Initialized = true;
            else
            {
                Initialized = false;
                return;
            }            
            string tn = cfa.AccessKey(cfa.CK_TOKEN_NAME);
            if (string.IsNullOrEmpty(tn) == true)
                tn = HSMBaseEnviroment.default_token_name;
            //string slots_string = InteropHSM.SlotArray(tn);
            //if (slots_string == "-")
            //    Slots = new string[0];
            //else
            //{
            //    string del2 = ",";
            //    Slots = slots_string.Split(del2.ToCharArray());
            //}
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
                if (ProbeIPAdr(adr) == true)
                {
                    info.Connection = "CONNECTED";
                }
                else
                {
                    info.Connection = "FAILED";
                }
                HSMStatus.Add(adr, info);
            }
        }
        protected void SetUpSlots()
        {
            if (Initialized == false)
                return;
            //int NuOfSlots = Slots.Length;
            //int i = 0;
            //char[] del = { ' ' };
            //string[] ipadr_env = EnviromentIPAddressString.Split(del);
            //foreach (string ip in ipadr_env)
            //{
            //    if (i < NuOfSlots)
            //    {
            //        if (HSMStatus.ContainsKey(ip) == true)
            //        {
            //            HSMStatus[ip].Slot = Convert.ToInt32(Slots[i]);
            //        }                    
            //    }
            //    i++;
            //}
        }
        
    }
}
