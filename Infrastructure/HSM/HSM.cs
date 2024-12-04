using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity;

namespace Infrastructure.HSM
{
    public class HSM
    {
        
        HSMTestProdEnviroment te = new HSMTestProdEnviroment();        
        List<HSMStatusInfo> status = new List<HSMStatusInfo>();
        IUnityContainer _container;
        public bool PasswordCheckSuccessfull { get; set; }

        

        public HSM(IUnityContainer container)
        {
            _container = container; 
            PasswordCheckSuccessfull = false;
        }
        
        public string Sign(byte[] data,string e,string cert_type)
        {
            IEnviroment es = te;
            int s = es.GetSlot();
            string kn=es.SigningKeyName(cert_type,e);
            return es.Sign(s, data, kn);
        }
        public string ReadPublicKey(string e,string cert_type)
        {
            IEnviroment es = te; 
            int s = es.GetSlot();
            string pk=es.PublicKey(s, cert_type,e);
            return pk; 
        }
        public List<HSMStatusInfo> HSMStatus(string e)
        {
            status.Clear();
            Dictionary<string, HSMStatusInfo> es = te.HSMStatus;
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
            
        }
        public List<KEY_STATUS> KeyStatus(string e)
        {
            var pwdSafe=_container.Resolve<StorePasswordSafe>();
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
                pk.TokenPwd = pwdSafe.GetSIXPassword(STORETYPE.KMS,ENVIROMENT.TEST,CERTTYPE.QA);
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
