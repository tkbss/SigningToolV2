using Org.BouncyCastle.Cms;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Security
{
    
    public class BaseSecurityProvider
    {

        protected Org.BouncyCastle.X509.X509Certificate verification_cert;
        protected CERTTYPE ParseIssuer(string iss,out ENVIROMENT e)
        {
            CERTTYPE ct = CERTTYPE.CA;            
            e = ENVIROMENT.TEST;
            char[] d = new char[] { ',' };
            string[] dns = iss.Split(d);
            foreach (string part in dns)
            {
                if (part.StartsWith("CN=") == true)
                {
                    string cn = part.Split(new char[] { '=' })[1];
                    if (cn.Contains("QA") == true)
                    {
                        ct = CERTTYPE.QA;                        
                    }
                    else
                    {
                        ct = CERTTYPE.CA;                        
                    }
                    if (cn.Contains("PROD") == true)
                        e= ENVIROMENT.PROD;
                    else
                        e= ENVIROMENT.TEST;
                    return ct;
                }
            }
            return ct;
        }
        public bool SigningStatus(CERTTYPE ct, ENVIROMENT e,MANUFACTURER signer, PackageInfo pi, out System.Security.Cryptography.X509Certificates.X509Certificate SigningCert)
        {
            try
            {
                string sig_inf = string.Empty;
                string env="."+Converter.Env(e);
                if (ct==CERTTYPE.MANU)
                    sig_inf = Path.Combine(pi.ExtractionPath, pi.FileName + env + ".sign.export");
                else
                    sig_inf = Path.Combine(pi.ExtractionPath, pi.FileName + ".sign.export");
                if (File.Exists(sig_inf) == false)
                {
                    SigningCert = null;
                    return false;

                }
                string info_f = Path.Combine(pi.ExtractionPath, pi.FileName + ".info");
                SigningCert = VerifyPackageSignatureInternal(sig_inf, info_f);
                
                return true;
            }
            catch (Exception ex)
            {
                string m = ex.Message;
                SigningCert = null;
                return false;
            }


        }
        protected System.Security.Cryptography.X509Certificates.X509Certificate VerifyPackageSignatureInternal(string sig_inf, string info_f)
        {
            byte[] file_content = null;
            byte[] cms_content = null;
            if (File.Exists(sig_inf) == false)
            {
                FileNotFoundException e = new FileNotFoundException("Signature file not found in package extraction path.", sig_inf);
                throw e;
            }
            if (File.Exists(info_f) == false)
            {
                FileNotFoundException e = new FileNotFoundException("Setup info file not found in package extraction path.", info_f);
                throw e;
            }

            try
            {
                FileStream fs = File.OpenRead(sig_inf);
                file_content = new byte[fs.Length];
                fs.Read(file_content, 0, (int)fs.Length);
                fs.Close();
            }
            catch (Exception b1)
            {
                IOException e = new IOException("Cannot read Signature file.", b1);
                throw e;
            }
            try
            {
                FileStream fs = File.OpenRead(info_f);
                cms_content = new byte[fs.Length];
                fs.Read(cms_content, 0, (int)fs.Length);
                fs.Close();
            }
            catch (Exception b2)
            {
                IOException e = new IOException("Cannot read Setup info file.", b2);
                throw e;
            }
            Org.BouncyCastle.Asn1.Asn1Object parsed = Org.BouncyCastle.Asn1.Asn1Object.FromByteArray(file_content);
            Org.BouncyCastle.Asn1.Cms.ContentInfo ci = Org.BouncyCastle.Asn1.Cms.ContentInfo.GetInstance(parsed);
            Org.BouncyCastle.Cms.CmsProcessableByteArray content = new Org.BouncyCastle.Cms.CmsProcessableByteArray(cms_content);
            Org.BouncyCastle.Cms.CmsSignedData sd = new Org.BouncyCastle.Cms.CmsSignedData(content, ci);

            Org.BouncyCastle.X509.Store.IX509Store x509Certs = sd.GetCertificates("Collection");


            Org.BouncyCastle.Cms.SignerInformationStore signers = sd.GetSignerInfos();
            System.Collections.ICollection sig = signers.GetSigners();
            foreach (Org.BouncyCastle.Cms.SignerInformation signer in sig)
            {

                Org.BouncyCastle.Cms.SignerID sid = signer.SignerID;
                System.Collections.ICollection mc = x509Certs.GetMatches(sid);
                if (mc == null || mc.Count == 0)
                {
                    string d = "Certificate to verify signature not in CMS";
                    throw new Exception(d);
                }
                System.Collections.IEnumerator certEnum = mc.GetEnumerator();
                certEnum.MoveNext();
                verification_cert = (Org.BouncyCastle.X509.X509Certificate)certEnum.Current;
                bool res = signer.Verify(verification_cert);
                System.Security.Cryptography.X509Certificates.X509Certificate c = Org.BouncyCastle.Security.DotNetUtilities.ToX509Certificate(verification_cert);
                return c;
            }
            CmsException ex = new CmsException("No SignerInfo in CMS");
            throw ex;
        }
        protected void WriteSignature(PackageInfo pi,byte[]cms,CERTTYPE ct,ENVIROMENT e)
        {
            string sig_inf = string.Empty;
            string env = Converter.Env(e);
            env = "." + env;
            if (ct==CERTTYPE.MANU)
                sig_inf = Path.Combine(pi.ExtractionPath, pi.FileName + env + ".sign.export");
            else
                sig_inf = Path.Combine(pi.ExtractionPath, pi.FileName + ".sign.export");
            try
            {
                //if (File.Exists(sig_inf) == true)
                //{
                //    File.Delete(sig_inf);
                //}
                FileStream fs = File.Create(sig_inf);
                fs.Write(cms, 0, (int)cms.Length);
                fs.Close();
            }
            catch (Exception b1)
            {
                IOException ex = new IOException("Cannot write Signature file.", b1);
                throw ex;
            }
        }
    }
}
