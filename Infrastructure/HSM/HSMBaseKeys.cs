using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.HSM
{
    public class KEY_STATUS
    {
        public string KEY_NAME { get; set; }
        public string TYPE { get; set; }
        public bool ESTABLISHED { get; set; }
    }

    public class HSMBaseKeys
    {
        protected ConfigurationAccess ca = new ConfigurationAccess();
        protected List<KEY_STATUS> key_status = new List<KEY_STATUS>();
        protected Dictionary<string, string> MapDefaultValues;
        public string TokenPwd { get; set; }
        public List<KEY_STATUS> KeyStatus
        {
            get
            {
                return key_status;
            }
        }
        
        public int OpenSession(int slot)
        {
            string slot_name = ca.AccessKey(ca.CK_TOKEN_NAME);
            if (string.IsNullOrEmpty(slot_name) == true)
                slot_name = HSMBaseEnviroment.default_token_name;
            //string pwd = "324324";
            if (string.IsNullOrEmpty(TokenPwd) == true)
                return 100;
            int r = InteropHSM.OpenSession(TokenPwd, slot_name,slot);
            return r;
        }
        public void CloseSession()
        {
            InteropHSM.CloseSession();
        }
        protected string KeyName(string key)
        {
            try
            {
                string kn = ca.AccessKey(key);
                if (string.IsNullOrEmpty(kn) == true)
                    kn = MapDefaultValues[key];
                return kn;
            }
            catch
            {
                return string.Empty;
            }
        }
        protected void Status(string entry)
        {
            string kn = KeyName(entry);
            bool p = kn.ToLower().Contains("priv");
            bool res = InteropHSM.KeyStatus(kn, p);
            KEY_STATUS ks = new KEY_STATUS();
            ks.ESTABLISHED = res;
            ks.KEY_NAME = kn;
            if (p == true)
                ks.TYPE = "PRIVATE";
            else
                ks.TYPE = "PUBLIC";
            key_status.Add(ks);
        }
        public string Sign(int slot, byte[] data, string kn)
        {
            if (OpenSession(slot) > 0)
                return string.Empty;
            string sig=InteropHSM.Sign(data, kn);
            CloseSession();
            return sig;
        }

    }
}
