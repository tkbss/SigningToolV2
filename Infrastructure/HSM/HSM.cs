using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.HSM
{
    public class HSM
    {
        
        HSMTestEnviroment te = new HSMTestEnviroment();
        //HSMProdEnviroment pe = new HSMProdEnviroment();
        List<HSMStatusInfo> status = new List<HSMStatusInfo>();
        
        public bool PasswordCheckSuccessfull { get; set; }

        

        public HSM()
        {
            PasswordCheckSuccessfull = false;
        }
        public bool IsConnected(string e)
        {
            IEnviroment es = te;
            //if (e == "TEST")
            //    es = te;
            //else
            //    es = pe;
            return es.IsConnected();
        }
        
        public bool TokenIsAvailable(string e)
        {
            IEnviroment es = te;
            //if (e == "TEST")
            //    es = te;
            //else
            //    es = pe;
            return es.TokenIsAvailable();
        }
        public string Sign(byte[] data,string e,string cert_type)
        {
            IEnviroment es = te;
            //if (e == "TEST")
            //    es = te;
            //else
            //    es = pe;
            int s = es.GetSlot();
            string kn=es.SigningKeyName(cert_type);
            return es.Sign(s, data, kn);
        }
        public string ReadPublicKey(string e,string cert_type)
        {
            IEnviroment es = te;
            //if (e == "TEST")
            //    es = te;
            //else
            //    es = pe;
            
            int s = es.GetSlot();
            string pk=es.PublicKey(s, cert_type);
            return pk; 
        }
        public List<HSMStatusInfo> HSMStatus(string e)
        {
            status.Clear();
            Dictionary<string, HSMStatusInfo> es = te.HSMStatus;
            //if(e=="TEST")
            //    es=te.HSMStatus;
            //else
            //    es = pe.HSMStatus;
            foreach (var element in es)
            {
                status.Add(element.Value);
            }
            return status;
        }
        public bool CheckPassword(string pwd,string enviroment)
        {
            PasswordCheckSuccessfull = te.CheckPwd(pwd);
            return PasswordCheckSuccessfull;
            //if (enviroment == "TEST")
            //{
            //    PasswordCheckSuccessfull = te.CheckPwd(pwd);
            //    return PasswordCheckSuccessfull;
            //}
            //else
            //{
            //    PasswordCheckSuccessfull=pe.CheckPwd(pwd);
            //    return PasswordCheckSuccessfull;
            //}
        }
        public List<KEY_STATUS> KeyStatus(string e)
        {
            te.DetermineKeyStatus();
            List<KEY_STATUS> ks = null;// te.KeyStatus;
            if (e == "TEST")
            {
                te.DetermineKeyStatus();
                ks = te.KeyStatus;
            }
            else
            {
                HSMProdKeys pk = new HSMProdKeys();                
                pk.DetermineKeyStatus(0);
                ks = pk.KeyStatus;
            }
            return ks;
        }
        
    }

    public class HSMStatusInfo
    {
        public string IPAdr { get; set; }
        public string Connection { get; set; }

        public int Slot { get; set; }
    }
}
