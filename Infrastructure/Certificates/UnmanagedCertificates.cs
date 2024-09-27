using infrastructure.security.provider;
using Infrastructure.Exceptions;
using Microsoft.Xaml.Behaviors.Media;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.X509;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Infrastructure
{
    public class UnmanagedCertificates : IUnmanagedCertificates
    {
        private KeyStoreHandling keystore;
        public string SIXSQASigningKeys
        { get { return "SIXSQASigningKeys"; } }
        public string SIXSATMSigningKeys
        { get { return "SIXSATMSigningKeys"; } }

        
        public UnmanagedCertificates()
        {
            keystore = new KeyStoreHandling(); 
        }
        public void CreateCACertificate(string cn,string ou,string o,string c,string pwd)
        {
            SIXCertificateManagement cert_gen = new SIXCertificateManagement();
            cert_gen.CreateCAKeys(2048);
            cert_gen.MakeCACertificate(c, o, ou, "ZH", cn);
            string storename = keystore.UnmanagedCAKeyStoreName;            
            string fn = Path.Combine(keystore.StorePath(MANUFACTURER.SIX, ENVIROMENT.TEST, STORETYPE.UNMANAGED), storename+".p12");
            X509Certificate cert=cert_gen.SaveCACeritifcatesWithPrivateKey(fn, storename,pwd);
            StoreSIXCertificate(cert,CERTTYPE.CA, ENVIROMENT.TEST);
        }
        public void CreateSIXSigningCertificate(string cn, string ou, string o, string c, string type, string pwd)
        {
            SIXCertificateManagement cert_gen = new SIXCertificateManagement();           
            string ca_storename = keystore.UnmanagedCAKeyStoreName;
            string fn = Path.Combine(keystore.StorePath(MANUFACTURER.SIX, ENVIROMENT.TEST, STORETYPE.UNMANAGED), ca_storename + ".p12");
            cert_gen.LoadCAKeys(fn, pwd);
            string storename;
            CERTTYPE ct;
            if (type == SIXSQASigningKeys)
            {
               storename = keystore.UnmanagedQAKeyStoreName;
                ct = CERTTYPE.QA;
            }
            else
            {
                storename = keystore.UnmanagedATMKeyStoreName;
                ct = CERTTYPE.ATM;
            }
            fn= Path.Combine(keystore.StorePath(MANUFACTURER.SIX, ENVIROMENT.TEST, STORETYPE.UNMANAGED), storename + ".p12");
            cert_gen.CreateKeys(2048);
            if(ct==CERTTYPE.QA)
                cert_gen.MakeCertificate(c, o, ou, "ZH", cn, true);
            else
                cert_gen.MakeCertificate(c, o, ou, "ZH", cn, false);
            cert_gen.SaveCeritifcatesWithPrivateKey(fn, storename,pwd);
            StoreSIXCertificate(cert_gen.GetCertificate(),ct, ENVIROMENT.TEST);
        }
        public bool CheckCAKeys(string pwd)
        {
            try
            {
                SIXCertificateManagement cert_gen = new SIXCertificateManagement();
                string storename = keystore.UnmanagedCAKeyStoreName;
                string fn = Path.Combine(keystore.StorePath(MANUFACTURER.SIX, ENVIROMENT.TEST, STORETYPE.UNMANAGED), storename + ".p12");
                cert_gen.LoadCAKeys(fn, pwd);
                return true;
            }
            catch
            {
                return false;
            }
        }
        

        public KEYStoreStatus GetKeyStoreStatus(ENVIROMENT env, STORETYPE st,CERTTYPE t,MANUFACTURER signer)
        {
            KEYStoreStatus status = new KEYStoreStatus();
            
            status.Manufacturer = Converter.Manu(signer);
            status.Enviroment = Converter.Env(env);
            status.Managed = Converter.ST(st);

            string cfp= keystore.StorePath(signer, env, st);
            string sn= keystore.StoreName(signer, env, st, t);
            if (st == STORETYPE.KMS)
            {
                status.FilePath = Path.Combine(cfp, sn + ".cer");
            }
            else
            {
                status.FilePath = Path.Combine(cfp, sn + ".p12");
            }
            
            if (File.Exists(status.FilePath) == true)
            {
                status.Creation = File.GetLastWriteTime(status.FilePath).ToString();
                status.StoreStatus = KEYStoreStatus.CREATED;//"CREATED";
            }
            else
                status.StoreStatus = KEYStoreStatus.MISSING;
            if(signer!=MANUFACTURER.SIX)
            {
                string rp = Path.Combine(cfp, PKCS10_Request_Name(status.Manufacturer,status.Enviroment));
                if (File.Exists(rp) == true)
                {
                    status.RequestStatus = KEYStoreStatus.CREATED;
                    status.RequestCreation= File.GetLastWriteTime(rp).ToString();
                }
                else
                    status.RequestStatus = KEYStoreStatus.MISSING;
                string cp= Path.Combine(cfp, CertificateName(status.Manufacturer,status.Enviroment));
                if (File.Exists(cp) == true)
                {
                    status.CertificateStatus = KEYStoreStatus.CREATED;
                    status.CertificateImport = File.GetLastWriteTime(cp).ToString();
                }
                else
                    status.CertificateStatus = KEYStoreStatus.MISSING;
            }
            return status;
             
        }
        public string GetKeyStorePath(MANUFACTURER m,STORETYPE st,ENVIROMENT e)
        {
            return keystore.StorePath(m, e, st);
        }

        public string GetSIXKeyStorePath(string type, string env)
        {
            if (Converter.ST(type) == STORETYPE.UNMANAGED)
                return keystore.StorePath(MANUFACTURER.SIX, ENVIROMENT.TEST, STORETYPE.UNMANAGED);
            else
            {
                if (Converter.Env(env) == ENVIROMENT.TEST)
                    return keystore.StorePath(MANUFACTURER.SIX, ENVIROMENT.TEST, STORETYPE.KMS);
                else
                    return keystore.StorePath(MANUFACTURER.SIX, ENVIROMENT.PROD, STORETYPE.KMS);
            }

        }
        public string GetStoreName(MANUFACTURER m,ENVIROMENT e,STORETYPE st,CERTTYPE ct)
        {
            return keystore.StoreName(m, e, st, ct);
           
        }


        public void CreateManufacturerSigningKeys(string manu,string env, string pwd, string cn, string ou, string o, string c)
        {
            SIXCertificateManagement cert_gen = new SIXCertificateManagement();
            ENVIROMENT e = Converter.Env(env);
            cert_gen.CreateCAKeys(2048);
            cert_gen.MakeCACertificate(c, o, ou, "ZH", cn);
            MANUFACTURER m = Converter.Manu(manu);
            string storename = keystore.StoreName(m,e , STORETYPE.UNMANAGED, CERTTYPE.MANU);
            string store_p = keystore.StorePath(m,e , STORETYPE.UNMANAGED);
            string fn = Path.Combine(store_p, storename + ".p12");
            string ctn= Path.Combine(store_p,CertificateName(manu,env));
            if (File.Exists(ctn) == true)
                File.Delete(ctn);
            string req = Path.Combine(store_p, PKCS10_Request_Name(manu,env));
            if (File.Exists(req) == true)
                File.Delete(req);
            cert_gen.SaveCACeritifcatesWithPrivateKey(fn, storename,pwd);
        }

        public string SignManufacturerCertificate(string manu, string cert_req,string pwd,out string sn_cert)
        {           
            SIXCertificateManagement cert_gen = new SIXCertificateManagement();
            string storename = keystore.StoreName(MANUFACTURER.SIX,ENVIROMENT.TEST,STORETYPE.UNMANAGED,CERTTYPE.QA);
            string fn = Path.Combine(keystore.StorePath(MANUFACTURER.SIX, ENVIROMENT.TEST, STORETYPE.UNMANAGED), storename + ".p12");
            try
            {
                cert_gen.LoadCAKeys(fn, pwd);
            }
            catch(IOException)
            {
                throw new StorePasswordExceptions();
            }
            string cert_path = Path.ChangeExtension(cert_req, "cer");
            X509Certificate manu_cert=cert_gen.BuildCertificateFromRequest(cert_req,cert_path);
            sn_cert = manu_cert.SubjectDN.ToString();
            StoreManuCertificate(manu_cert);
            return cert_path;
        }
        public string[] ParseManufacturerFromCert(string cp) 
        {
            FileStream rs = new FileStream(cp, FileMode.Open);
            X509CertificateParser p = new X509CertificateParser();
            X509Certificate c = p.ReadCertificate(rs);
            rs.Close();
            int index = c.SubjectDN.ToString().IndexOf("CN=");
            string dn=c.SubjectDN.ToString().Substring(index+3);
            string[] cn=dn.Split(new char[] { '_' });
            return cn;
        }
        public void ImportCertifiedManufacturer(string certPath,STORETYPE st,ENVIROMENT e) 
        {
            string prod = keystore.CertifiedPath(MANUFACTURER.SIX, ENVIROMENT.PROD, st);
            string test = keystore.CertifiedPath(MANUFACTURER.SIX, ENVIROMENT.TEST, st);
            string[] manu = ParseManufacturerFromCert(certPath);
            string cn = CertificateName(manu[0].ToUpper(), manu[1].ToUpper());
            string destination=Path.Combine(prod, cn);
            File.Copy(certPath, destination,true);
            destination = Path.Combine(test, cn);
            File.Copy(certPath, destination, true);
        }
        private void StoreSIXCertificate(X509Certificate c,CERTTYPE ct,ENVIROMENT e)
        {
            string p = keystore.CertifiedPath(MANUFACTURER.SIX, e, STORETYPE.UNMANAGED);
            string cn=keystore.SIXCertName( e, STORETYPE.UNMANAGED, ct);
            string fn = Path.Combine(p, cn);
            byte[] cert = c.GetEncoded();
            FileStream fs = File.Create(fn);
            fs.Write(cert, 0, cert.Length);
            fs.Close();
        }
        private void StoreManuCertificate(X509Certificate c)
        {
            X509Name n = c.SubjectDN;
            X509NameTokenizer nt = new X509NameTokenizer(n.ToString());
            string manu=string.Empty;
            string env=string.Empty;
            while (nt.HasMoreTokens() == true)
            {
                string t = nt.NextToken();
                if (t.ToUpper().Contains("O="))
                {
                    manu = t.Split(new char[] { '=' })[1];
                }
                if (t.ToUpper().Contains("CN="))
                {
                    if (t.ToUpper().Contains("PROD"))
                        env = "PROD";
                    else
                        env = "TEST";
                }
            }
            string cn=CertificateName(manu, env);
            MANUFACTURER m = Converter.Manu(manu);
            ENVIROMENT e = Converter.Env(env);
            string p=keystore.CertifiedPath(MANUFACTURER.SIX, e, STORETYPE.UNMANAGED);
            string fn = Path.Combine(p, cn);
            byte[] cert=c.GetEncoded();
            FileStream fs = File.Create(fn);
            fs.Write(cert, 0, cert.Length);
            fs.Close();
        }
        public void ImportManufacturerSigningCertificate(string manu,string env,string pwd,string cert_path)
        {
            MANUFACTURER m = Converter.Manu(manu);
            ENVIROMENT e = Converter.Env(env);
            SIXCertificateManagement cert_gen = new SIXCertificateManagement();
            string store_name = keystore.StoreName(m,e , STORETYPE.UNMANAGED, CERTTYPE.MANU);
            string store_path = Path.Combine(keystore.StorePath(m, e, STORETYPE.UNMANAGED), store_name + ".p12");
            try
            {
                cert_gen.InsertCertificateToStore(cert_path, store_path, store_name, pwd);
            }
            catch(IOException)
            {
                throw new StorePasswordExceptions();
            }
            string cp=Path.Combine(keystore.StorePath(m, e, STORETYPE.UNMANAGED), CertificateName(manu,env));
            File.Delete(cp);            
            File.Copy(cert_path, cp);        
        }
       

        public void ExportManufacturerSigningRequest(string manu,string env,string pwd,string target_fn)
        {
            MANUFACTURER m = Converter.Manu(manu);
            ENVIROMENT e = Converter.Env(env);
            string storename = keystore.StoreName(m, e, STORETYPE.UNMANAGED, CERTTYPE.MANU);
            string fn = Path.Combine(keystore.StorePath(m, e, STORETYPE.UNMANAGED), storename + ".p12");
            SIXCertificateManagement cert_gen = new SIXCertificateManagement();
            byte[] request = null;
            try
            {
                request = cert_gen.BuildPKCS10Request(fn, storename, pwd);
            }
            catch(IOException)
            {
                throw new StorePasswordExceptions();
            }
            string store_path = keystore.StorePath(m, e, STORETYPE.UNMANAGED);
            string rn= Path.Combine(store_path, PKCS10_Request_Name(manu,env));
            if (File.Exists(rn) == true)
                File.Delete(rn);
            FileStream fs = new FileStream(rn, FileMode.Create);
            fs.Write(request, 0, request.Length);
            fs.Close();
            if (File.Exists(target_fn) == true)
                File.Delete(target_fn);
            fs = new FileStream(target_fn, FileMode.Create);
            fs.Write(request, 0, request.Length);
            fs.Close();
            string cp= Path.Combine(store_path, CertificateName(manu,env));
            File.Delete(cp);
        }
        private string PKCS10_Request_Name(string m,string enviroment)
        {
            string r = "PKCS10_" + m +"_"+enviroment +".req";
            return r;
        }
        private string CertificateName(string manu,string enviroment)
        {
            string n = "CERTIFICATE_" + manu + "_" + enviroment+".cer";
            return n;
        }

        public void ExportCACertificate(string target_fn, string pwd)
        {
            KeyStoreHandling ksh = new KeyStoreHandling();
            string sn = ksh.StoreName(MANUFACTURER.SIX, ENVIROMENT.TEST, STORETYPE.UNMANAGED, CERTTYPE.CA);
            string store_path = ksh.StorePath(MANUFACTURER.SIX, ENVIROMENT.TEST, STORETYPE.UNMANAGED);
            string fp = Path.Combine(store_path, sn + ".p12");
            SIXCertificateManagement cert_handling = new SIXCertificateManagement();
            cert_handling.LoadCAKeys(fp, pwd);
            cert_handling.ExportCACertificate(target_fn);
            
        }

        public System.Security.Cryptography.X509Certificates.X509Certificate GetCertificate(MANUFACTURER m,ENVIROMENT e,CERTTYPE ct,string pwd)
        {
            KeyStoreHandling ksh = new KeyStoreHandling();
            string env = Converter.Env(e);
            string sn = ksh.StoreName(m, e, STORETYPE.UNMANAGED, ct);
            string store_path = ksh.StorePath(m, e, STORETYPE.UNMANAGED);
            SIXCertificateManagement cert_handling = new SIXCertificateManagement();
            string fp = Path.Combine(store_path, sn + ".p12");
            Org.BouncyCastle.X509.X509Certificate c;
            if (ct== CERTTYPE.CA)
            {
                cert_handling.LoadCAKeys(fp, pwd);
                c = cert_handling.GetCACertificate();
            }
            else if(ct==CERTTYPE.MANU)
            {
                string mfp = Path.Combine(store_path, CertificateName(Converter.Manu(m),env));
                if (File.Exists(mfp) == false)
                    return null;
                X509CertificateParser p = new X509CertificateParser();
                FileStream fileStream = new FileStream(mfp, FileMode.Open);
                c = p.ReadCertificate(fileStream);
                fileStream.Close();
            }
            else 
            {
                cert_handling.LoadCertificateKeys(fp, pwd);
                c = cert_handling.GetCertificate();
            }
            return Org.BouncyCastle.Security.DotNetUtilities.ToX509Certificate(c);
        }
        /*
        public void ChangeStorePassword(MANUFACTURER m, CERTTYPE ct, string oldp, string newp)
        {
            KeyStoreHandling ksh = new KeyStoreHandling();
            ksh.ChangePassword(m, ct, oldp, newp);
        }
        public void DeleteStorePassword(MANUFACTURER m)
        {
            keystore.DeleteStorePassword(m);
        }
        public string GetStorePassword(MANUFACTURER m)
        {
            return keystore.GetStorePassword(m);
        }
        public void SetStorePassword(MANUFACTURER m, string pwd)
        {
            keystore.SetStorePassword(m, pwd);
        }
        */
    }
}
