using infrastructure.security.provider;
using Infrastructure.Exceptions;
using Infrastructure.Interfaces;
using Infrastructure.Security;
using Unity;
using Org.BouncyCastle.Cms;
using Org.BouncyCastle.Pkcs;
using Org.BouncyCastle.X509;
using Org.BouncyCastle.X509.Store;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;

using System.Text;
using System.Threading.Tasks;

namespace Infrastructure
{
    class UnmanagedSecurityProvider : BaseSecurityProvider, ISecurity
    {
        IUnityContainer _container;
        public UnmanagedSecurityProvider(IUnityContainer container)
        {
            _container = container;
        }
        public System.Security.Cryptography.X509Certificates.X509Certificate VerifyPackageSignature(PackageInfo pi)
        {
            string sig_inf = Path.Combine(pi.ExtractionPath, pi.FileName + ".sign");
            string info_f = Path.Combine(pi.ExtractionPath, pi.FileName + ".info");
            string export_f = Path.Combine(pi.ExtractionPath, pi.FileName + ".export");
            return VerifyPackageSignatureInternal(sig_inf, info_f);
        }
        
        public void CheckVerificationCert(string pwd, System.Security.Cryptography.X509Certificates.X509Certificate c)
        {            
            UnmanagedCertificates uc = new UnmanagedCertificates();
            string iss = c.Issuer;
            ENVIROMENT e = ENVIROMENT.TEST;
            CERTTYPE ct = ParseIssuer(iss,out e);
            string store_type = Converter.ST(STORETYPE.UNMANAGED);
            string store_path = uc.GetSIXKeyStorePath(store_type, "TEST");
            string sn=uc.GetStoreName(MANUFACTURER.SIX, ENVIROMENT.TEST, STORETYPE.UNMANAGED, ct);            
            string ca_keystore_fp = Path.Combine(store_path, sn + ".p12");
            KeyStoreHandling ksh = new KeyStoreHandling();
            Pkcs12Store store= ksh.LoadP12Store(ca_keystore_fp, pwd);           
            X509Certificate ca_cert = store.GetCertificate(sn).Certificate;           
            
            verification_cert.Verify(ca_cert.GetPublicKey());
            verification_cert.CheckValidity();           
           

        }
        public bool ExportSignatureExists(PackageInfo pi,CERTTYPE ct,ENVIROMENT e)
        {
            string sig_inf = string.Empty;
            string env = "." + Converter.Env(e);
            if (ct==CERTTYPE.MANU)
                sig_inf = Path.Combine(pi.ExtractionPath, pi.FileName +env+ ".sign.export");
            else
                sig_inf = Path.Combine(pi.ExtractionPath, pi.FileName + ".sign.export");
            if (File.Exists(sig_inf) == false)
            {
                return false;

            }
            else
                return true;
        }
        

        public void GeneratePackageSignature(MANUFACTURER manufacturer,ENVIROMENT e, CERTTYPE ct, PackageInfo pi,string pwd)
        {
            UnmanagedCertificates uc = new UnmanagedCertificates();        
            string store_path = uc.GetKeyStorePath(manufacturer, STORETYPE.UNMANAGED, e);

            string info_f = Path.Combine(pi.ExtractionPath, pi.FileName + ".info");
            if (File.Exists(info_f) == false)
            {
                FileNotFoundException ex = new FileNotFoundException("Setup info file not found in package extraction path.", info_f);
                throw ex;
            }
            byte[] cms_content;
            try
            {
                FileStream fs = File.OpenRead(info_f);
                cms_content = new byte[fs.Length];
                fs.Read(cms_content, 0, (int)fs.Length);
                fs.Close();
            }
            catch (Exception b2)
            {
                IOException ex = new IOException("Cannot read Setup info file.", b2);
                throw ex;
            }

            KeyStoreHandling ksh = new KeyStoreHandling();
            string sn = ksh.StoreName(manufacturer, e, STORETYPE.UNMANAGED, ct);
            string fp = Path.Combine(store_path, sn+".p12");
            Pkcs12Store store;
            try
            {
                store = ksh.LoadP12Store(fp, pwd);
            }
            catch
            {
                throw new StorePasswordExceptions();
            }
            AsymmetricKeyEntry privkey= store.GetKey(sn);
            X509Certificate signing_cert = store.GetCertificate(sn).Certificate;
            IList certList = new ArrayList();
            certList.Add(signing_cert);
            IX509Store signing_cert_in_store = X509StoreFactory.Create("Certificate/Collection", new X509CollectionStoreParameters(certList));
            Org.BouncyCastle.Cms.CmsSignedDataGenerator gen = new Org.BouncyCastle.Cms.CmsSignedDataGenerator();
            string oid = CryptoConfig.MapNameToOID("SHA256");
            gen.AddSigner(privkey.Key, signing_cert, oid);
            gen.AddCertificates(signing_cert_in_store);
            Org.BouncyCastle.Cms.CmsProcessable msg = new Org.BouncyCastle.Cms.CmsProcessableByteArray(cms_content);
            Org.BouncyCastle.Cms.CmsSignedData cmsSignedData = gen.Generate(msg, false);
            byte[] cms = cmsSignedData.ContentInfo.GetDerEncoded();
            WriteSignature(pi, cms,ct,e);
            
        }

        
    }
}
