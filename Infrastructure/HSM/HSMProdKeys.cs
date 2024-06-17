using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.HSM
{
    public class HSMProdKeys : HSMBaseKeys
    {
        public static string default_qa_sig_publ_key = "ATMSS_PUBL_SIGN_QA";
        public static string default_qa_sig_priv_key = "ATMSS_PRIV_SIGN_QA";
        public static string default_atm_prod_ca_publ_key = "ATMSS_PROD_PUBL_CA_ATM";
        public static string default_atm_prod_ca_priv_key = "ATMSS_PROD_PRIV_CA_ATM";
        public static string default_atm_prod_sig_publ_key = "ATMSS_PROD_PUBL_SIGN_ATM";
        public static string default_atm_prod_sig_priv_key = "ATMSS_PROD_PRIV_SIGN_ATM";

        //public static string default_atm_prod_ip = "10.152.16.36 10.152.16.37 10.152.16.38 10.152.144.45 10.152.144.46 10.152.144.47";
        public HSMProdKeys()
        {
            MapDefaultValues = new Dictionary<string, string>();
            MapDefaultValues.Add(ca.CK_QA_SIG_PUBL_KEY, default_qa_sig_publ_key);
            MapDefaultValues.Add(ca.CK_QA_SIG_PRIV_KEY, default_qa_sig_priv_key);
            MapDefaultValues.Add(ca.CK_ATM_PROD_CA_PRIV_KEY, default_atm_prod_ca_priv_key);
            MapDefaultValues.Add(ca.CK_ATM_PROD_CA_PUBL_KEY, default_atm_prod_ca_publ_key);
            MapDefaultValues.Add(ca.CK_ATM_PROD_SIG_PRIV_KEY, default_atm_prod_sig_priv_key);
            MapDefaultValues.Add(ca.CK_ATM_PROD_SIG_PUBL_KEY, default_atm_prod_sig_publ_key);
        }


        public void DetermineKeyStatus(int slot)
        {
            
            if (OpenSession(slot) != 0)
                return;
            key_status.Clear();
            foreach (var entry in ca.ATM_PROD)
            {
                Status(entry);
            }
            CloseSession();
        }
        public string DeterminePublicKeyName(string cert_type)
        {
            CERTTYPE ct = Converter.CertType(cert_type);
            string kn = string.Empty;
            switch (ct)
            {
                case CERTTYPE.QA:
                    kn = ca.AccessKey(ca.CK_QA_SIG_PUBL_KEY);
                    if (string.IsNullOrEmpty(kn) == true)
                        kn = MapDefaultValues[ca.CK_QA_SIG_PUBL_KEY];
                    break;
                case CERTTYPE.CA:
                    kn = ca.AccessKey(ca.CK_ATM_PROD_CA_PUBL_KEY);
                    if (string.IsNullOrEmpty(kn) == true)
                        kn = MapDefaultValues[ca.CK_ATM_PROD_CA_PUBL_KEY];
                    break;
                case CERTTYPE.ATM:
                    kn = ca.AccessKey(ca.CK_ATM_PROD_SIG_PUBL_KEY);
                    if (string.IsNullOrEmpty(kn) == true)
                        kn = MapDefaultValues[ca.CK_ATM_PROD_SIG_PUBL_KEY];
                    break;
            }
            return kn;
        }
        public string DetermineSigningKeyName(string cert_type)
        {
            CERTTYPE ct = Converter.CertType(cert_type);
            string kn = string.Empty;
            switch (ct)
            {
                case CERTTYPE.QA:
                    kn = ca.AccessKey(ca.CK_QA_SIG_PRIV_KEY);
                    if (string.IsNullOrEmpty(kn) == true)
                        kn = MapDefaultValues[ca.CK_QA_SIG_PRIV_KEY];
                    break;
                case CERTTYPE.CA:
                    kn = ca.AccessKey(ca.CK_ATM_PROD_CA_PRIV_KEY);
                    if (string.IsNullOrEmpty(kn) == true)
                        kn = MapDefaultValues[ca.CK_ATM_PROD_CA_PRIV_KEY];
                    break;
                case CERTTYPE.ATM:
                    kn = ca.AccessKey(ca.CK_ATM_PROD_SIG_PRIV_KEY);
                    if (string.IsNullOrEmpty(kn) == true)
                        kn = MapDefaultValues[ca.CK_ATM_PROD_SIG_PRIV_KEY];
                    break;
            }
            return kn;
        }

    }
}
