using infrastructure.security.provider;
using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.X509;
using System.IO;

namespace Infrastructure.Certificates
{
    public class SignerCertificateMapping
    {
        
        
        public List<string> CertifiedManufactures(STORETYPE st)
        {
            List<string> cert_manu_list = new List<string>();
            ENVIROMENT e;
            if (st == STORETYPE.KMS)
                e = ENVIROMENT.PROD;
            else
                e= ENVIROMENT.TEST;            
            KeyStoreHandling ks = new KeyStoreHandling();
            string env = Converter.Env(e);            
            STORETYPE manu_st = st;
            string ps = ks.CertifiedPath(MANUFACTURER.SIX, ENVIROMENT.TEST, manu_st);            
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
            SIXCertificateManagement c_m = new SIXCertificateManagement();
            string tb = c_m.Thumbprint(c);
            string r = SignerIsManu(ENVIROMENT.TEST, tb);
            if (string.IsNullOrEmpty(r) == false)
            {
                s = SIGNER.MANU;
                return r;
            }
            r= SignerIsManu(ENVIROMENT.PROD, tb);
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
        private string SignerIsManu(ENVIROMENT e,string tb)
        {
            STORETYPE st;
            string env =Converter.Env(e);
            st=STORETYPE.KMS;
            //if (e == ENVIROMENT.TEST)
            //    st = STORETYPE.UNMANAGED;
            //else
            //    st = STORETYPE.KMS;
            string signer = string.Empty;
            KeyStoreHandling ks = new KeyStoreHandling();
            string ps = ks.CertifiedPath(MANUFACTURER.SIX, ENVIROMENT.TEST, st);
            foreach (MANUFACTURER m in Converter.Manufactures)
            {
                string manu = Converter.Manu(m);
                string mcn = ManuCertificateName(manu, env);
                string fns = Path.Combine(ps, mcn);
                if (IsMatchingCert(tb, fns) == true)
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
        private bool IsMatchingCert(string tb, string fn)
        {
            if (File.Exists(fn) == false)
                return false;            
            FileStream rs = new FileStream(fn, FileMode.Open);
            X509CertificateParser p = new X509CertificateParser();
            X509Certificate c = p.ReadCertificate(rs);
            rs.Close();
            SIXCertificateManagement c_m = new SIXCertificateManagement();
            string mc_tb = c_m.Thumbprint(c);
            if (mc_tb == tb)
                return true;
            else
                return false;
        }
    }
}
