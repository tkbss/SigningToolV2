using Infrastructure.Certificates;
using Infrastructure.Exceptions;
using Org.BouncyCastle.Pkcs;
using Org.BouncyCastle.Security;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure
{
    public class KeyStoreHandling: KeyStoreBase
    {
        
        Dictionary<MANUFACTURER,string> password_store_test = new Dictionary<MANUFACTURER, string>();
        Dictionary<MANUFACTURER,string> password_store_prod = new Dictionary<MANUFACTURER, string>();

        
        public KeyStoreHandling()
        {
            setup_keystore_root();
        }
        public void ChangePassword(MANUFACTURER m,ENVIROMENT e, CERTTYPE ct,string oldp,string newp)
        {
            string sn = StoreName(m, e, STORETYPE.UNMANAGED, ct);
            string store_path = StorePath(m, e, STORETYPE.UNMANAGED);
            string fp = Path.Combine(store_path, sn + ".p12");
            if (File.Exists(fp) == false)
                return;
            Pkcs12Store s;
            try
            {
                s = LoadP12Store(fp, oldp);
            }
            catch
            {
                throw new StorePasswordExceptions();
            }
            //File.Delete(fp);
            X509CertificateEntry ce = s.GetCertificate(sn);
            AsymmetricKeyEntry key = s.GetKey(sn);
            X509CertificateEntry[] chain = new X509CertificateEntry[] { ce };
            Pkcs12Store new_store = new Pkcs12StoreBuilder().Build();
            new_store.SetKeyEntry(sn, key, chain);

            FileStream fileStream = new FileStream(fp, FileMode.Create);
            char[] p = newp.ToCharArray();
            new_store.Save(fileStream, p, new SecureRandom());
            fileStream.Close();
        }
        public string GetStorePassword(MANUFACTURER m,ENVIROMENT e)
        {
            try
            {
                if (m == MANUFACTURER.SIX)
                    return "1234";
                string pwd=string.Empty;
                switch (e)
                {
                    case ENVIROMENT.TEST:
                        pwd = password_store_test[m];
                        break;
                    case ENVIROMENT.PROD:
                        pwd = password_store_prod[m];
                        break;
                }
                return pwd;
            }
            catch
            {
                return string.Empty;
            }
        }
        public void SetStorePassword(MANUFACTURER m,ENVIROMENT e,string pwd)
        {
            try
            {
                
                switch (e)
                {
                    case ENVIROMENT.TEST:
                       password_store_test[m]=pwd;
                        break;
                    case ENVIROMENT.PROD:
                        password_store_prod[m]=pwd;
                        break;
                }
            }
            catch
            {
                
            }
        }
        public void DeleteStorePassword(MANUFACTURER m,ENVIROMENT e)
        {            
            switch (e)
            {
                case ENVIROMENT.TEST:
                    password_store_test.Remove(m);
                    break;
                case ENVIROMENT.PROD:
                    password_store_prod.Remove(m);
                    break;
            }
        }
        
        

        public string UnmanagedCAKeyStoreName
        {
            get { return "SIXTestUnmanagedCAKeyStore"; }
        }
        public string UnmanagedQAKeyStoreName
        {
            get { return "SIXTestUnmanagedQAKeyStore"; }
        }
        public string UnmanagedATMKeyStoreName
        {
            get { return "SIXTestUnmanagedATMKeyStore"; }
        }
        
        public Pkcs12Store LoadP12Store(string fp,string pwd)
        {
            Pkcs12Store store = new Pkcs12Store();
            FileStream fs = File.Open(fp, FileMode.Open);
            store.Load(fs, pwd.ToCharArray());
            fs.Close();
            return store;
        }
        private void setup_keystore_root()
        {
            string app_data= System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData);
            root_path = Path.Combine(app_data,root_dir);
            if(Directory.Exists(root_path)==false)
            {
                Directory.CreateDirectory(root_path);
            }
            keystore_path= Path.Combine(root_path,"KEYSTORES");
            if (Directory.Exists(keystore_path) == false)
            {
                Directory.CreateDirectory(keystore_path);
            }
            foreach (string m in manu)
            {
                string manu_path = Path.Combine(keystore_path,m);
                if (Directory.Exists(manu_path) == false)
                {
                    Directory.CreateDirectory(manu_path);
                    //string bck_path = Path.Combine(manu_path, "BACKUP");
                    //if (Directory.Exists(bck_path) == false)
                    //{
                    //    Directory.CreateDirectory(bck_path);
                    //}
                }
            }
            setup_six_keystore_env("TEST");
            setup_six_keystore_env("PROD");

        }
        private void setup_six_keystore_env(string env)
        {
            string six_path = Path.Combine(keystore_path, manu[0]);
            env_path = Path.Combine(six_path, env);
            if (Directory.Exists(env_path) == false)
            {
                Directory.CreateDirectory(env_path);
            }
            setup_managed_stores(KMS);
            setup_managed_stores(UMD);
        }
        private void setup_managed_stores(string storetype)
        {
            string man_path = Path.Combine(env_path, storetype);
            if (Directory.Exists(man_path) == false)
            {
                Directory.CreateDirectory(man_path);
            }
            //CREATE SIX KEYSTORE
            //string manu_path = Path.Combine(man_path, manu[0]);
            //if (Directory.Exists(manu_path) == false)
            //{
            //    Directory.CreateDirectory(manu_path);
            //    string bck_path = Path.Combine(manu_path, "BACKUP");
            //    if (Directory.Exists(bck_path) == false)
            //    {
            //        Directory.CreateDirectory(bck_path);
            //    }
            //}
            
        }
    }
}
