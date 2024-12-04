using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.HSM
{
    public class HSMTestKeys : HSMBaseKeys
    {
        public static string default_qa_sig_publ_key = "ATMSS_PUBL_SIGN_QA";
        public static string default_qa_sig_priv_key = "ATMSS_PRIV_SIGN_QA";
        public static string default_atm_test_ca_publ_key = "ATMSS_TEST_PUBL_CA_ATM";
        public static string default_atm_test_ca_priv_key = "ATMSS_TEST_PRIV_CA_ATM";
        public static string default_atm_test_sig_publ_key = "ATMSS_TEST_PUBL_SIGN_ATM";
        public static string default_atm_test_sig_priv_key = "ATMSS_TEST_PRIV_SIGN_ATM";

       
        public static string default_atm_prod_ca_publ_key = "ATMSS_PROD_PUBL_CA_ATM";
        public static string default_atm_prod_ca_priv_key = "ATMSS_PROD_PRIV_CA_ATM";
        public static string default_atm_prod_sig_publ_key = "ATMSS_PROD_PUBL_SIGN_ATM";
        public static string default_atm_prod_sig_priv_key = "ATMSS_PROD_PRIV_SIGN_ATM";


        public HSMTestKeys()
        {           
            MapDefaultValues = new Dictionary<string, string>();
            MapDefaultValues.Add(ca.CK_QA_SIG_PUBL_KEY, default_qa_sig_publ_key);
            MapDefaultValues.Add(ca.CK_QA_SIG_PRIV_KEY, default_qa_sig_priv_key);
            MapDefaultValues.Add(ca.CK_ATM_TEST_CA_PRIV_KEY, default_atm_test_ca_priv_key);
            MapDefaultValues.Add(ca.CK_ATM_TEST_CA_PUBL_KEY, default_atm_test_ca_publ_key);
            MapDefaultValues.Add(ca.CK_ATM_TEST_SIG_PRIV_KEY, default_atm_test_sig_priv_key);
            MapDefaultValues.Add(ca.CK_ATM_TEST_SIG_PUBL_KEY, default_atm_test_sig_publ_key);
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
            foreach (var entry in ca.ATM_TEST)
            {
                Status(entry);
            }
            CloseSession();
        }
        public string DeterminePublicKeyName(string cert_type,string env)
        {
            CERTTYPE ct=Converter.CertType(cert_type);
            ENVIROMENT e = Converter.Env(env);
            string kn = string.Empty;
            switch(ct)
            {
                case CERTTYPE.QA:
                    kn=ca.AccessKey(ca.CK_QA_SIG_PUBL_KEY);
                    if (string.IsNullOrEmpty(kn) == true)
                        kn = MapDefaultValues[ca.CK_QA_SIG_PUBL_KEY];
                    break;
                case CERTTYPE.CA:
                    kn = ca.AccessKey(ca.CK_ATM_TEST_CA_PUBL_KEY);
                    if (string.IsNullOrEmpty(kn) == true)
                        kn = MapDefaultValues[ca.CK_ATM_TEST_CA_PUBL_KEY];
                    break;
                case CERTTYPE.ATM:
                    if(e == ENVIROMENT.TEST)
                    {
                        kn = ca.AccessKey(ca.CK_ATM_TEST_SIG_PUBL_KEY);
                        if (string.IsNullOrEmpty(kn) == true)
                            kn = MapDefaultValues[ca.CK_ATM_TEST_SIG_PUBL_KEY];
                    }
                    else
                    {
                        kn = ca.AccessKey(ca.CK_ATM_PROD_SIG_PUBL_KEY);
                        if (string.IsNullOrEmpty(kn) == true)
                            kn = MapDefaultValues[ca.CK_ATM_PROD_SIG_PUBL_KEY];
                    }                    
                    break;
            }
            return kn;
        }
        public string DetermineSigningKeyName(string cert_type,string env)
        {
            CERTTYPE ct = Converter.CertType(cert_type);
            ENVIROMENT e = Converter.Env(env);
            string kn = string.Empty;
            switch (ct)
            {
                case CERTTYPE.QA:
                    kn = ca.AccessKey(ca.CK_QA_SIG_PRIV_KEY);
                    if (string.IsNullOrEmpty(kn) == true)
                        kn = MapDefaultValues[ca.CK_QA_SIG_PRIV_KEY];
                    break;
                case CERTTYPE.CA:
                    kn = ca.AccessKey(ca.CK_ATM_TEST_CA_PRIV_KEY);
                    if (string.IsNullOrEmpty(kn) == true)
                        kn = MapDefaultValues[ca.CK_ATM_TEST_CA_PRIV_KEY];
                    break;
                case CERTTYPE.ATM:
                    if (e == ENVIROMENT.TEST)
                    {
                        kn = ca.AccessKey(ca.CK_ATM_TEST_SIG_PRIV_KEY);
                        if (string.IsNullOrEmpty(kn) == true)
                            kn = MapDefaultValues[ca.CK_ATM_TEST_SIG_PRIV_KEY];
                    }
                    else
                    {
                        kn = ca.AccessKey(ca.CK_ATM_PROD_SIG_PRIV_KEY);
                        if (string.IsNullOrEmpty(kn) == true)
                            kn = MapDefaultValues[ca.CK_ATM_PROD_SIG_PRIV_KEY];
                    }
                    break;
            }
            return kn;
        }
    }
}
