using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.HSM
{
    public class HSMTestEnviroment: HSMBaseEnviroment, IEnviroment
    {
        
        HSMTestKeys tk = new HSMTestKeys();
        public static string default_atm_test_ip = "hsmt01.wl.six-group.net";
        public List<KEY_STATUS> KeyStatus
        {
            get
            {
                return tk.KeyStatus;
            }
        }
        public HSMTestEnviroment()
        {
            SetIPEnviroment();
            IpStatus(cfa.CK_QA_IP_ADR, default_atm_test_ip);
            SetUpSlots();
        }
        public void DetermineKeyStatus()
        {
            var s= HSMStatus.Values.Where(e => e.Connection == "CONNECTED").First();
            tk.DetermineKeyStatus(s.Slot);
        }
        public bool CheckPwd(string pwd)
        {
            var s = HSMStatus.Values.Where(e => e.Connection == "CONNECTED").First();
            tk.TokenPwd = pwd;
            try {
                tk.OpenSession(s.Slot);
                tk.CloseSession();
                return true;
            }
            catch 
            {
                tk.TokenPwd = string.Empty;
                return false;
            }         
        }       
        public string PublicKey(int slot,string cert_type)
        {
            string kn=tk.DeterminePublicKeyName(cert_type);
            if (tk.OpenSession(slot) > 0)
                return string.Empty;
            string key = InteropHSM.ReadPublicKey(kn);
            tk.CloseSession();
            return key;
        }

        public string SigningKeyName(string cert_type)
        {
            return tk.DetermineSigningKeyName(cert_type);
        }

        public string Sign(int slot, byte[] data, string kn)
        {
            return tk.Sign(slot,data,kn);
        }

        public bool IsConnected()
        {
            try
            {
                var s = HSMStatus.Values.Where(e => e.Connection == "CONNECTED").First();
                if (s == null)
                    return false;
                else
                    return true;
            }
            catch
            {
                return false;
            }
        }

        public bool TokenIsAvailable()
        {
            try
            {
                var s = HSMStatus.Values.Where(e => e.Connection == "CONNECTED" && e.Slot>=0).First();
                if (s == null)
                    return false;
                else
                    return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
