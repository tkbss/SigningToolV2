using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
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
            get { return "CK_HSM_IP_ADR"; }
        }
        


        public ConfigurationAccess()
        {
            exeConfigPath = Assembly.GetEntryAssembly()?.Location;
            try
            {
                if (!string.IsNullOrEmpty(exeConfigPath))
                {
                    config = ConfigurationManager.OpenExeConfiguration(exeConfigPath);
                }
            }
            catch
            {
                //handle error here.. means DLL has no satellite configuration file.
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
        public void SetRegistry(string regKey, string parameter, string value)
        {
            var key = Registry.CurrentUser.OpenSubKey(regKey, true);
            if (key == null)
            {
                key = Registry.CurrentUser.CreateSubKey(regKey);
                if (key == null)
                {
                    throw new InvalidOperationException($"Creating registry key {regKey} failed");
                }
            }

            key.SetValue(parameter, value);
        }
        public string ReadRegistry(string regKey, string parameter)
        {
            var key = Registry.CurrentUser.OpenSubKey(regKey);
            if (key != null)
            {
                var value = key.GetValue(parameter);
                if (value != null)
                {
                    return value.ToString();
                }
            }
            return string.Empty;
        }

        public void ModifyKey(string key, string value)
        {
            if (config != null)
            {
                KeyValueConfigurationElement element = config.AppSettings.Settings[key];
                if (element != null)
                {
                    config.AppSettings.Settings[key].Value = value;
                    config.Save(ConfigurationSaveMode.Modified);
                }
            }
        }

        public void AddKey(string key, string value)
        {
            if (config != null)
            {
                KeyValueConfigurationElement element = config.AppSettings.Settings[key];
                if (element == null)
                {
                    config.AppSettings.Settings.Add(key, value);
                    config.Save(ConfigurationSaveMode.Modified);
                }
                else
                {
                    ModifyKey(key, value);
                }
            }
        }

        public void RemoveKey(string key)
        {
            if (config != null)
            {
                config.AppSettings.Settings.Remove(key);
                config.Save(ConfigurationSaveMode.Modified);
            }
        }
    }
}
