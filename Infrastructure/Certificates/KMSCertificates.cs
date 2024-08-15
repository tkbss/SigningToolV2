using infrastructure.security.provider;
using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.Pkcs;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.X509;
using System.IO;

namespace Infrastructure.Certificates
{
    public class KMSCertificates: KeyStoreBase
    {
        public void ExportCACertificate(string target_fn,ENVIROMENT e)
        {
            KeyStoreHandling ksh = new KeyStoreHandling();
            string sn = ksh.StoreName(MANUFACTURER.SIX, e, STORETYPE.KMS, CERTTYPE.CA);
            string store_path = ksh.StorePath(MANUFACTURER.SIX, e, STORETYPE.KMS);
            string fp = Path.Combine(store_path, sn + ".cer");
            if (File.Exists(fp))
                File.Copy(fp, target_fn);
            else
                throw new FileNotFoundException("CA certificate file not found");          

        }
        public void CreateCACertificate(ENVIROMENT e,Infrastructure.HSM.HSM hsm)
        {
            string cert_type = "CA";
            string env = Converter.Env(e);
            string key = hsm.ReadPublicKey(env,cert_type);
            SIXCertificateManagement cm = new SIXCertificateManagement();
            SubjectPublicKeyInfo spki=cm.MakeSubjectPubliKeyInfo(key);
            cm.MakeKMSCACert(cm.MakeCACommonName(env));
            X509Certificate ca_cert=cm.GetCACertificate();
            TbsCertificateStructure c = ChangePublicKeyInCertCert(ca_cert, spki);
            byte[] raw_cert = c.GetDerEncoded();
            //string hex_str_cert = Converter.ToHexString(raw_cert);            
            string signature = hsm.Sign(raw_cert, env,cert_type);
            byte[] s = Converter.HexStrToBytes(signature);
            DerBitString der_sig = new DerBitString(s);
            AlgorithmIdentifier alogId = new AlgorithmIdentifier(PkcsObjectIdentifiers.Sha256WithRsaEncryption);
            X509CertificateStructure css = new X509CertificateStructure(c, alogId, der_sig);
            SaveCertificate(css.GetDerEncoded(), e, CERTTYPE.CA);
            X509Certificate cert = new X509Certificate(css);
            StoreSIXCertificate(cert, CERTTYPE.CA, e);
        }
        public void CreateSigningCertificate(ENVIROMENT e,CERTTYPE ct, Infrastructure.HSM.HSM hsm)
        {
            string cert_type = Converter.CertType(ct);
            string env = Converter.Env(e);
            string key = hsm.ReadPublicKey(env, cert_type);
            SIXCertificateManagement cm = new SIXCertificateManagement();
            SubjectPublicKeyInfo spki = cm.MakeSubjectPubliKeyInfo(key);
            //string common_name = "KMS "+env+" "+cert_type+" "+"SIGNING KEY";
            cm.MakeKMSSigningCert(cm.MakeSigningCertCommonName("KMS",env,cert_type),ct,cm.MakeCACommonName(env));
            X509Certificate sig_cert = cm.GetCertificate();
            TbsCertificateStructure c = ChangePublicKeyInCertCert(sig_cert, spki);
            byte[] raw_cert = c.GetDerEncoded();
            //string hex_str_cert = Converter.ToHexString(raw_cert);
            string signature = string.Empty;
            if (ct == CERTTYPE.QA)
                signature = hsm.Sign(raw_cert, env, "QA"); 
            else
                signature = hsm.Sign(raw_cert, env, "CA");
            byte[] s = Converter.HexStrToBytes(signature);
            DerBitString der_sig = new DerBitString(s);
            AlgorithmIdentifier alogId = new AlgorithmIdentifier(PkcsObjectIdentifiers.Sha256WithRsaEncryption);
            X509CertificateStructure css = new X509CertificateStructure(c, alogId, der_sig);
            SaveCertificate(css.GetDerEncoded(), e, ct);
            if(ct == CERTTYPE.QA)
                SaveCertificate(css.GetDerEncoded(), ENVIROMENT.PROD, ct);
            X509Certificate cert = new X509Certificate(css);
            StoreSIXCertificate(cert, ct, e);
            if(ct == CERTTYPE.QA)
                StoreSIXCertificate(cert, ct, ENVIROMENT.PROD);
        }
        public string SignManufacturerCertificate(string cert_req,string targetPath,string env, Infrastructure.HSM.HSM hsm)
        {
            ENVIROMENT e = Converter.Env(env);
            X509Certificate signer=GetBCCertificate(e, CERTTYPE.QA);
            SIXCertificateManagement cm = new SIXCertificateManagement();
            X509Certificate ManuCert=cm.BuildSimpleCertificateFromRequest(cert_req, signer.SubjectDN);
            byte[] raw_cert = ManuCert.CertificateStructure.TbsCertificate.GetDerEncoded();            
            //string hex_str_cert = Converter.ToHexString(raw_cert);
            string signature = hsm.Sign(raw_cert, env, "QA");
            byte[] s = Converter.HexStrToBytes(signature);
            DerBitString der_sig = new DerBitString(s);
            AlgorithmIdentifier alogId = new AlgorithmIdentifier(PkcsObjectIdentifiers.Sha256WithRsaEncryption);
            X509CertificateStructure css = new X509CertificateStructure(ManuCert.CertificateStructure.TbsCertificate, alogId, der_sig);
            string certPath= Path.Combine(targetPath, Path.GetFileName(cert_req));
            string cert_path = Path.ChangeExtension(certPath, "cer");
            FileStream writeStream = new FileStream(cert_path, FileMode.Create);
            byte[] encoded_cert = css.GetDerEncoded();
            writeStream.Write(encoded_cert, 0, encoded_cert.Length);
            writeStream.Close();
            X509Certificate cert = new X509Certificate(css);
            StoreManuCertificate(cert);
            return cert_path;
        }
        private TbsCertificateStructure ChangePublicKeyInCertCert(X509Certificate cert, SubjectPublicKeyInfo spki)
        {
            V3TbsCertificateGenerator n_c = new V3TbsCertificateGenerator();
            TbsCertificateStructure tbs = cert.CertificateStructure.TbsCertificate;

            n_c.SetEndDate(tbs.EndDate);
            n_c.SetStartDate(tbs.StartDate);
            n_c.SetExtensions(tbs.Extensions);
            n_c.SetIssuer(tbs.Issuer);
            n_c.SetSerialNumber(tbs.SerialNumber);
            n_c.SetSignature(tbs.Signature);
            n_c.SetSubject(tbs.Subject);
            n_c.SetSubjectPublicKeyInfo(spki);
            return n_c.GenerateTbsCertificate();        
        }
        
        public void SaveCertificate(byte[] cert, ENVIROMENT e, CERTTYPE ct)
        {
            string sp = StorePath(MANUFACTURER.SIX, e, STORETYPE.KMS);
            string sn = StoreName(MANUFACTURER.SIX, e, STORETYPE.KMS, ct);
            string fp = Path.Combine(sp, sn + ".cer");
            if (File.Exists(fp) == true)
            {
                //Make backup
            }
            FileStream fs = new FileStream(fp, FileMode.Create);
            fs.Write(cert, 0, cert.Length);
            fs.Close();
        }
        public System.Security.Cryptography.X509Certificates.X509Certificate GetCertificate(ENVIROMENT e, CERTTYPE ct)
        {
            Org.BouncyCastle.X509.X509Certificate c = GetBCCertificate(e, ct);
            if (c == null)
                return null;
            System.Security.Cryptography.X509Certificates.X509Certificate cert = Org.BouncyCastle.Security.DotNetUtilities.ToX509Certificate(c);
            return cert;
        }
        public DateTime GetCertificateCreationDate(ENVIROMENT e, CERTTYPE ct)
        {
            string sp = StorePath(MANUFACTURER.SIX, e, STORETYPE.KMS);
            string sn = StoreName(MANUFACTURER.SIX, e, STORETYPE.KMS, ct);
            string fp = Path.Combine(sp, sn + ".cer");
            if (File.Exists(fp) == false)
                return DateTime.MinValue;
            return File.GetCreationTime(fp);
        }
        public X509Certificate GetBCCertificate(ENVIROMENT e, CERTTYPE ct)
        {
            string sp = StorePath(MANUFACTURER.SIX, e, STORETYPE.KMS);
            string sn = StoreName(MANUFACTURER.SIX, e, STORETYPE.KMS, ct);
            string fp = Path.Combine(sp, sn + ".cer");
            if (File.Exists(fp) == false)
                return null;
            Org.BouncyCastle.X509.X509Certificate c = null;
            try
            {
                X509CertificateParser p = new X509CertificateParser();
                FileStream fileStream = new FileStream(fp, FileMode.Open);
                c = p.ReadCertificate(fileStream);
                fileStream.Close();
            }
            catch
            {
                return null;
            }
            return c;
        }
        public System.Security.Cryptography.X509Certificates.X509Certificate GetCertificate(string env,string certtype)
        {
            ENVIROMENT e=Converter.Env(env);
            CERTTYPE ct = Converter.CertType(certtype);
            return GetCertificate(e, ct);
        }
        private void StoreSIXCertificate(X509Certificate c, CERTTYPE ct, ENVIROMENT e)
        {
            string p = CertifiedPath(MANUFACTURER.SIX, e, STORETYPE.KMS);
            string cn = SIXCertName(e, STORETYPE.KMS, ct);
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
            string manu = string.Empty;
            string env = string.Empty;
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
            string cn = CertificateName(manu, env);
            MANUFACTURER m = Converter.Manu(manu);            
            string p = CertifiedPath(MANUFACTURER.SIX, ENVIROMENT.TEST, STORETYPE.KMS);
            string fn = Path.Combine(p, cn);
            byte[] cert = c.GetEncoded();
            FileStream fs = File.Create(fn);
            fs.Write(cert, 0, cert.Length);
            fs.Close();
        }
        private string CertificateName(string manu, string enviroment)
        {
            string n = "CERTIFICATE_" + manu + "_" + enviroment + ".cer";
            return n;
        }
    }
}
