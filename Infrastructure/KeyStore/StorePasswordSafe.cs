using Unity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure
{
    public class StorePasswordSafe
    {
        
        KeyStoreHandling unmanaged_ks = new KeyStoreHandling();
        IUnityContainer _container;
        Dictionary<string, string> _six = new Dictionary<string, string>();
        public bool PasswordChanged { get; set; }
        public string DefaultStorePwd
        {
            get
            {
                return "1234";
            }
        }
        public StorePasswordSafe(IUnityContainer container)
        {
            _container = container;
        }
        public void DeleteStorePassword(MANUFACTURER m,ENVIROMENT e)
        {
            unmanaged_ks.DeleteStorePassword(m,e);
        }
        public string GetStorePassword(MANUFACTURER m,ENVIROMENT e)
        {
            return unmanaged_ks.GetStorePassword(m,e);
        }
        public void SetStorePassword(MANUFACTURER m,ENVIROMENT e, string pwd)
        {
            unmanaged_ks.SetStorePassword(m,e, pwd);
        }
        
        public void AddSIXPwd(STORETYPE store_type,ENVIROMENT e,CERTTYPE cert_type,string pwd)
        {
            string st = Converter.ST(store_type);
            string env = Converter.Env(e);
            string ct = Converter.CertType(cert_type);
            string key = st + "_" + env + "_" + ct;
            _six[key] = pwd;
        }
        
        public string GetSIXPassword(STORETYPE store_type, ENVIROMENT e, CERTTYPE cert_type)
        {
            string st = Converter.ST(store_type);
            string env = Converter.Env(e);
            string ct = Converter.CertType(cert_type);
            string key = st + "_" + env + "_" + ct;
            string pwd = string.Empty;
            try
            {
                pwd = _six[key];
            }
            catch { }
            return pwd;
        }
        public bool CheckHSMPwd(string pwd,string e)
        {
            Infrastructure.HSM.HSM hsm = _container.Resolve<Infrastructure.HSM.HSM>();
            return hsm.CheckPassword(pwd, e);
        }
        public bool ManuStoreExists(MANUFACTURER m, ENVIROMENT e)
        {
            string sp = unmanaged_ks.StorePath(m, e, STORETYPE.UNMANAGED);
            string sn = unmanaged_ks.StoreName(m, e, STORETYPE.UNMANAGED, CERTTYPE.MANU) + ".p12";
            string fn = Path.Combine(sp, sn);
            if (File.Exists(fn) == true)
                return true;
            else
                return false;
        }
        public bool CheckManuStorePwd(MANUFACTURER m,ENVIROMENT e,string pwd)
        {
            string sp=unmanaged_ks.StorePath(m, e, STORETYPE.UNMANAGED);
            string sn = unmanaged_ks.StoreName(m, e, STORETYPE.UNMANAGED, CERTTYPE.MANU)+".p12";
            string fn = Path.Combine(sp, sn);
            try
            {
                unmanaged_ks.LoadP12Store(fn, pwd);
                return true;
            }
            catch
            {
                return false;
            }
            
        }
        public void ChangeStorePassword(MANUFACTURER m,ENVIROMENT e, CERTTYPE ct, string oldp, string newp)
        {
            unmanaged_ks.ChangePassword(m,e, ct, oldp, newp);
        }

    }
}
