using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure
{
    public class ConfigurationAccess
    {
        string exeConfigPath = string.Empty;
        Configuration config = null;
        public List<string> ATM_TEST = new List<string>();
        public List<string> ATM_PROD = new List<string>();
        

        public string CK_ATM_TEST_CA_PUBL_KEY
        {
            get { return "CK_ATM_TEST_CA_PUBL_KEY"; }
        } 
        public string CK_ATM_TEST_CA_PRIV_KEY
        {
            get { return "CK_ATM_TEST_CA_PRIV_KEY"; }
        }
        public string CK_ATM_PROD_CA_PUBL_KEY
        {
            get { return "CK_ATM_PROD_CA_PUBL_KEY"; }
        }
        public string CK_ATM_PROD_CA_PRIV_KEY
        {
            get { return "CK_ATM_PROD_CA_PRIV_KEY"; }
        }

        public string CK_ATM_TEST_SIG_PUBL_KEY
        {
            get { return "CK_ATM_TEST_SIG_PUBL_KEY"; }
        }
        public string CK_ATM_TEST_SIG_PRIV_KEY
        {
            get { return "CK_ATM_TEST_SIG_PRIV_KEY"; }
        }
        public string CK_ATM_PROD_SIG_PUBL_KEY
        {
            get { return "CK_ATM_PROD_SIG_PUBL_KEY"; }
        }
        public string CK_ATM_PROD_SIG_PRIV_KEY
        {
            get { return "CK_ATM_PROD_SIG_PRIV_KEY"; }
        }
        public string CK_QA_SIG_PUBL_KEY
        {
            get { return "CK_QA_SIG_PUBL_KEY"; }
        }
        public string CK_QA_SIG_PRIV_KEY
        {
            get { return "CK_QA_SIG_PRIV_KEY"; }
        }

        public string CK_TOKEN_NAME
        {
            get { return "CK_TOKEN_NAME"; }
        }
        public string CK_QA_IP_ADR
        {
            get { return "CK_QA_IP_ADR"; }
        }
        public string CK_ATM_IP_ADR
        {
            get { return "CK_ATM_IP_ADR"; }
        }
       

        public ConfigurationAccess()
        {
            exeConfigPath = System.Reflection.Assembly.GetEntryAssembly().Location;
            try
            {
                config = ConfigurationManager.OpenExeConfiguration(exeConfigPath);

            }
            catch 
            {
                //handle errror here.. means DLL has no sattelite configuration file.
            }
            ATM_TEST.Add(CK_ATM_TEST_CA_PUBL_KEY);
            ATM_TEST.Add(CK_ATM_TEST_CA_PRIV_KEY);
            ATM_TEST.Add(CK_ATM_TEST_SIG_PUBL_KEY);
            ATM_TEST.Add(CK_ATM_TEST_SIG_PRIV_KEY);
            ATM_TEST.Add(CK_QA_SIG_PUBL_KEY);
            ATM_TEST.Add(CK_QA_SIG_PRIV_KEY);
            ATM_PROD.Add(CK_ATM_PROD_CA_PUBL_KEY);
            ATM_PROD.Add(CK_ATM_PROD_CA_PRIV_KEY);            
            ATM_PROD.Add(CK_ATM_PROD_SIG_PUBL_KEY);
            ATM_PROD.Add(CK_ATM_PROD_SIG_PRIV_KEY);
            ATM_PROD.Add(CK_QA_SIG_PUBL_KEY);
            ATM_PROD.Add(CK_QA_SIG_PRIV_KEY);


        }
        public string AccessKey(string key)
        {
            string value = string.Empty;
            if (config != null)
            {
                KeyValueConfigurationElement element = config.AppSettings.Settings[key];
                if (element != null)
                {
                    value = element.Value;
                }
            }
            return value;
        }
        public void ModifyKey(string key, string value)
        {
            KeyValueConfigurationElement element = config.AppSettings.Settings[key];
            if (element != null)
            {
                config.AppSettings.Settings[key].Value = value;
                config.Save(ConfigurationSaveMode.Modified);               

            }
        }
        public void AddKey(string key,string value)
        {
            KeyValueConfigurationElement element = config.AppSettings.Settings[key];
            if (element == null)
            {
                config.AppSettings.Settings.Add(key, value);
                config.Save(ConfigurationSaveMode.Modified);
            }
            else
                ModifyKey(key, value);
        }
        public void RemoveKey(string key)
        {
            config.AppSettings.Settings.Remove(key);
            config.Save(ConfigurationSaveMode.Modified);
        }
    }
}
