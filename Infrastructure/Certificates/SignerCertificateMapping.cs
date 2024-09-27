using infrastructure.security.provider;
using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.X509;
using System.IO;

namespace Infrastructure.Certificates
{
    public class SignerCertificateMapping
    {
        
        public System.Security.Cryptography.X509Certificates.X509Certificate GetCertifiedManufacturer(string manufacturer) 
        {
            KeyStoreHandling ks = new KeyStoreHandling();
            string manu=manufacturer.Split('-')[0];
            string env = manufacturer.Split('-')[1];
            string p = ks.CertifiedPath(MANUFACTURER.SIX, Converter.Env(env), STORETYPE.KMS);
            string fn = Path.Combine(p, ManuCertificateName(manu, env));
            if (File.Exists(fn) == false)
            {                
                    return null;
            }
            FileStream rs = new FileStream(fn, FileMode.Open);
            X509CertificateParser p1 = new X509CertificateParser();
            X509Certificate c = p1.ReadCertificate(rs);
            rs.Close();
            System.Security.Cryptography.X509Certificates.X509Certificate cert = Org.BouncyCastle.Security.DotNetUtilities.ToX509Certificate(c);
            return cert;
        }
        public List<string> CertifiedManufactures(STORETYPE st)
        {
            List<string> cert_manu_list = new List<string>();                               
            KeyStoreHandling ks = new KeyStoreHandling();
                      
            STORETYPE manu_st = st;
            string ps = ks.CertifiedPath(MANUFACTURER.SIX, ENVIROMENT.TEST, manu_st);
            string env = Converter.Env(ENVIROMENT.TEST);
            foreach (MANUFACTURER m in Converter.Manufactures)
            {
                string manu = Converter.Manu(m);
                string mcn = ManuCertificateName(manu, env);
                string fns = Path.Combine(ps, mcn);
                if (File.Exists(fns) == true)
                {
                    string certified = manu + "-" + env;
                    cert_manu_list.Add(certified);
                }                
            }
            env = Converter.Env(ENVIROMENT.PROD);
            ps = ks.CertifiedPath(MANUFACTURER.SIX, ENVIROMENT.PROD, manu_st);
            foreach (MANUFACTURER m in Converter.Manufactures)
            {
                string manu = Converter.Manu(m);
                string mcn = ManuCertificateName(manu, env);
                string fns = Path.Combine(ps, mcn);
                if (File.Exists(fns) == true)
                {
                    string certified = manu + "-" + env;
                    cert_manu_list.Add(certified);
                }
            }
            return cert_manu_list;
        }
        
        public string ResolveSigner(System.Security.Cryptography.X509Certificates.X509Certificate cert,ENVIROMENT e,STORETYPE st,out SIGNER s)
        {
            s = SIGNER.MANU;
            if (cert == null)
                return string.Empty;
            X509Certificate c = Org.BouncyCastle.Security.DotNetUtilities.FromX509Certificate(cert);
            
            string r = SignerIsManu(ENVIROMENT.TEST, c);
            if (string.IsNullOrEmpty(r) == false)
            {
                s = SIGNER.MANU;
                return r;
            }
            r= SignerIsManu(ENVIROMENT.PROD, c);
            if (string.IsNullOrEmpty(r) == false)
            {
                s = SIGNER.MANU;
                return r;
            }
            KeyStoreHandling ks = new KeyStoreHandling();            
            string env = Converter.Env(e);
            string p = ks.CertifiedPath(MANUFACTURER.SIX, e, st);
            string store_type = Converter.ST(st);
            string six_name=ks.SIXCertName(e, st, CERTTYPE.QA);
            string fn = Path.Combine(p, six_name);
            s = SIGNER.QA;
            if (IsMatchingPublicKey(c.CertificateStructure.SubjectPublicKeyInfo.PublicKeyData, fn) == true)
                return "SIX-QA" +"-"+store_type+"-"+ env;
            six_name = ks.SIXCertName(e, st, CERTTYPE.ATM);
            fn = Path.Combine(p, six_name);
            s = SIGNER.ATM;
            if (IsMatchingPublicKey(c.CertificateStructure.SubjectPublicKeyInfo.PublicKeyData, fn) == true)
                return "SIX-ATM" + "-" + store_type + "-" + env;
            env = "TEST";
            p = ks.CertifiedPath(MANUFACTURER.SIX, ENVIROMENT.TEST, st);
            six_name = ks.SIXCertName(ENVIROMENT.TEST, st, CERTTYPE.ATM);
            fn = Path.Combine(p, six_name);
            s = SIGNER.ATM;
            if (IsMatchingPublicKey(c.CertificateStructure.SubjectPublicKeyInfo.PublicKeyData, fn) == true)
                return "SIX-ATM" + "-" + store_type + "-" + env;
            else
                return string.Empty;
        }
        private string SignerIsManu(ENVIROMENT e, X509Certificate c)
        {
            STORETYPE st;
            string env =Converter.Env(e);
            st=STORETYPE.KMS;            
            string signer = string.Empty;
            KeyStoreHandling ks = new KeyStoreHandling();
            string ps = ks.CertifiedPath(MANUFACTURER.SIX, e, st);
            foreach (MANUFACTURER m in Converter.Manufactures)
            {
                string manu = Converter.Manu(m);
                string mcn = ManuCertificateName(manu, env);
                string fns = Path.Combine(ps, mcn);
                if (IsMatchingCert(c.IssuerDN.ToString(),c.SubjectDN.ToString(), fns) == true)
                    signer= manu + "-" + env;
            }
            return signer;
        }
        private string ManuCertificateName(string manu, string enviroment)
        {
            string n = "CERTIFICATE_" + manu + "_" + enviroment + ".cer";
            return n;
        }
        private bool IsMatchingPublicKey(DerBitString pk,string fn)
        {
            if (File.Exists(fn) == false)
                return false;
            FileStream rs = new FileStream(fn, FileMode.Open);
            X509CertificateParser p = new X509CertificateParser();
            X509Certificate c = p.ReadCertificate(rs);
            rs.Close();
            DerBitString kd = c.CertificateStructure.SubjectPublicKeyInfo.PublicKeyData;
            bool r = kd.Equals(pk);
            return r;
        }
        private bool IsMatchingCert(string i,string s, string fn)
        {
            if (File.Exists(fn) == false)
                return false;            
            FileStream rs = new FileStream(fn, FileMode.Open);
            X509CertificateParser p = new X509CertificateParser();
            X509Certificate c = p.ReadCertificate(rs);
            rs.Close();
            if ( i==c.IssuerDN.ToString() && s == c.SubjectDN.ToString())        
                return true;
            else
                return false;
        }
    }
}
