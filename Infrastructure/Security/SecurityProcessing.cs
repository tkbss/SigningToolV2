using Infrastructure.Exceptions;
using Infrastructure.Interfaces;
using Infrastructure.Security;
using Unity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure
{
    public class SecurityProcessing 
    {
        IUnityContainer _container;
        public System.Security.Cryptography.X509Certificates.X509Certificate Verifier
        { get; set; }
        public  SecurityProcessing(IUnityContainer container)
        {
            _container = container;
        }
        public  SecurityProcessing()
        {
            _container = null;
        }
        public void GeneratePackageSignature(STORETYPE st, MANUFACTURER m, ENVIROMENT e, CERTTYPE ct, PackageInfo pi,string pwd)
        {
            ISecurity s = SecurityFactory.CreateSecurity(st,e,_container);
            try
            {
                s.GeneratePackageSignature(m, e,ct, pi,pwd);
            }
            catch (StorePasswordExceptions)
            {
                throw;
            }
            catch (Exception )
            {
            }
            
        }
        
        public System.Security.Cryptography.X509Certificates.X509Certificate VerifyPackageSignature(string store_type,PackageInfo pi,string env,string pwd)
        {
            ISecurity s = SecurityFactory.CreateSecurity(Converter.ST(store_type),Converter.Env(env),_container);
            System.Security.Cryptography.X509Certificates.X509Certificate c;
            try
            {
                c=s.VerifyPackageSignature(pi);
                Verifier = c;
                
            }
            catch (Exception e)
            {
                throw new SignatureVerificationException("Verification of signature failed: "+e.Message);
            }
            try
            {
                s.CheckVerificationCert(pwd,c);
                return c;
            }
            catch (Exception e)
            {
                string m = e.Message;
                throw new CertificateValidationException("Verification of certificate failed. "+m);
            }

            
        }
        public bool ExportSignatureExists(STORETYPE st, ENVIROMENT e,CERTTYPE ct, PackageInfo pi)
        {
            ISecurity s = SecurityFactory.CreateSecurity(st, e,_container);
            return s.ExportSignatureExists(pi,ct,e);
        }
        public bool SigningStatus(STORETYPE st, MANUFACTURER signer, ENVIROMENT e, CERTTYPE ct, PackageInfo pi, out System.Security.Cryptography.X509Certificates.X509Certificate SigningCert)
        {
            ISecurity s = SecurityFactory.CreateSecurity(st, e,_container);
            return s.SigningStatus(ct,e,signer, pi, out SigningCert);
        }
        public string ComputeMessageDigest(string fp)
        {
            if (File.Exists(fp) == false)
                return string.Empty;
            FileStream fs = File.Open(fp, FileMode.Open);       

            SHA256 mySHA256 = SHA256.Create();
            var hashValue = mySHA256.ComputeHash(fs);
            fs.Close();
            return ByteArrayToString(hashValue);
        }
        static string ByteArrayToString(byte[] ba)
        {
            string hex = BitConverter.ToString(ba);
            return hex.Replace("-", "");
        }

    }
}
