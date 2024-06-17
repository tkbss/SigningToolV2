using Infrastructure.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography.X509Certificates;
using System.IO;
using Infrastructure.Security;
using Infrastructure.Certificates;
using System.Security.Cryptography.Pkcs;
using LipingShare.LCLib.Asn1Processor;
using Unity;

namespace Infrastructure
{
    class KMSSecurityProvider : BaseSecurityProvider, ISecurity
    {
        ENVIROMENT _enviroment;
        IUnityContainer _container;
        public KMSSecurityProvider(ENVIROMENT e,IUnityContainer container)
        {
            _enviroment = e;
            _container = container;
        }
        public System.Security.Cryptography.X509Certificates.X509Certificate VerifyPackageSignature(PackageInfo pi)
        {
            string sig_inf = Path.Combine(pi.ExtractionPath, pi.FileName + ".sign");
            string info_f = Path.Combine(pi.ExtractionPath, pi.FileName + ".info");
            return VerifyPackageSignatureInternal(sig_inf, info_f);
        }
        public void CheckVerificationCert(string pwd, System.Security.Cryptography.X509Certificates.X509Certificate c)
        {
            KMSCertificates kms = new KMSCertificates();
            string iss=verification_cert.IssuerDN.ToString();
            ENVIROMENT e = ENVIROMENT.TEST;
            CERTTYPE ct = ParseIssuer(iss,out e);
            X509Certificate ca_cert=kms.GetCertificate(e, ct);            
            verification_cert.Verify(Org.BouncyCastle.Security.DotNetUtilities.FromX509Certificate(ca_cert).GetPublicKey());
            verification_cert.CheckValidity();
        }
        
        public void GeneratePackageSignature(MANUFACTURER manufacturer,ENVIROMENT e, CERTTYPE ct, PackageInfo pi, string pwd)
        {
            string info_f = Path.Combine(pi.ExtractionPath, pi.FileName + ".info");
            string env = Converter.Env(e);
            string cert_type = Converter.CertType(ct);
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
            //get the necessary certificates
            string dummy_cert = DummyCertificate.Get();
            byte[] raw_dummy_signer_cert_content=Convert.FromBase64String(dummy_cert);
            X509Certificate2 dummy_signer_cert = new X509Certificate2(raw_dummy_signer_cert_content, "1234");
            KMSCertificates mc = new KMSCertificates();
            X509Certificate2 real_signer_cert=new X509Certificate2(mc.GetCertificate(e, ct));
            List<X509Certificate2> real_signer_certList = new List<X509Certificate2>();
            real_signer_certList.Add(real_signer_cert);

            //build content info            
            System.Security.Cryptography.Oid rawData = new System.Security.Cryptography.Oid("1.2.840.113549.1.7.1");
            Asn1Node octet = new Asn1Node();
            octet.Tag = 0x04;
            octet.Data = cms_content;
            MemoryStream lengthStream = new MemoryStream();
            octet.SaveData(lengthStream);
            byte[] t = lengthStream.ToArray();
            ContentInfo InfoToSign = new ContentInfo(rawData, t);

            //build signed data cms structure
            SignedCms signedCms = new SignedCms(InfoToSign,true);            
            CmsSigner signer = new CmsSigner(dummy_signer_cert);
            signer.IncludeOption = X509IncludeOption.ExcludeRoot;
            
            //Compute a dummy signature
            signedCms.ComputeSignature(signer);
            byte[] cmsMessage = signedCms.Encode();

            

            //Compute signature in hsm
            //string hex_str_content = Converter.ToHexString(cms_content);
            Infrastructure.HSM.HSM hsm=_container.Resolve<Infrastructure.HSM.HSM>();
            string signature = hsm.Sign(cms_content, env, cert_type);

            cms.CMSSignedData SignedData = new cms.CMSSignedData();
            SignedData.Decode(cmsMessage);
            //set algorith identifiers
            SignedData.SetSignatureAlgoAndDigestIdentifier(OIDs.SHA256, OIDs.RSAwithSHA256);
            //Replace signing and CA certificates. This is possible because they are not part of the signature
            SignedData.ReplaceCertificates(real_signer_certList.ToArray());
            //replace issuer and serial number identifier. This is possible because they are not part of the signature
            SignedData.ReplaceIssuerAndSerialNumber(real_signer_cert);
            //replace signature                
            SignedData.ReplaceSignature(Asn1Util.HexStrToBytes(signature));
            byte[] newContent = SignedData.Encode();
            WriteSignature(pi, newContent,ct,e);
        }

        

        public bool ExportSignatureExists(PackageInfo pi, CERTTYPE ct, ENVIROMENT e)
        {
            string sig_inf = Path.Combine(pi.ExtractionPath, pi.FileName + ".sign.export");
            if (File.Exists(sig_inf) == false)
            {
                return false;

            }
            else
                return true;
        }

        
        
    }
}
