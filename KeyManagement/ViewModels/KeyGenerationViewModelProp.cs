
using System.Collections.ObjectModel;
using System.Windows;


namespace SigningKeyManagment.ViewModels
{
    public partial class KeyGenerationViewModel
    {
        ObservableCollection<string> certified_manu;
        public ObservableCollection<string> CertifiedManufactures
        {
            get
            {
                return certified_manu;
            }
            set
            {
                SetProperty(ref certified_manu, value);
            }
        }
        public string Enviroment
        {
            get { return enviroment; }
            set
            {
                SetProperty(ref enviroment, value);
            }
        }
        Visibility certified_manus;
        public Visibility CertifiedManufacturers
        {
            get { return certified_manus; }
            set
            {
                SetProperty(ref certified_manus, value);
            }
        }
        Visibility change_store_pwd;
        public Visibility ChangeStorePassword
        {
            get { return change_store_pwd; }
            set
            {
                SetProperty(ref change_store_pwd, value);
            }
        }
        

        bool show_ca_cert;
        public bool ShowCACertificate
        {
            get { return show_ca_cert; }
            set
            {
                SetProperty(ref show_ca_cert, value);
            }
        }
        bool show_qa_cert;
        public bool ShowSigningQA
        {
            get { return show_qa_cert; }
            set
            {
                SetProperty(ref show_qa_cert, value);
            }
        }
        bool show_atm_manu_cert;
        public bool ShowSigningATMMANU
        {
            get { return show_atm_manu_cert; }
            set
            {
                SetProperty(ref show_atm_manu_cert, value);
            }
        }
        
        bool chk_override;
        public bool ChkOverride
        {
            get { return chk_override; }
            set
            {
                SetProperty(ref chk_override, value);
            }
        }

        bool chk_backup;
        public bool ChkBackup
        {
            get { return chk_backup; }
            set
            {
                SetProperty(ref chk_backup, value);
            }
        }
        bool chk_abort;
        public bool ChkAbort
        {
            get { return chk_abort; }
            set
            {
                SetProperty(ref chk_abort, value);
            }
        }
        
        string old_manu_pwd;
        public string OldManuStorePwd
        {
            get { return old_manu_pwd; }
            set
            {
                SetProperty(ref old_manu_pwd, value);
            }
        }
        string new_manu_pwd;
        public string NewManuStorePwd
        {
            get { return new_manu_pwd; }
            set
            {
                SetProperty(ref new_manu_pwd, value);
            }
        }
        string static_text_1;
        public string StaticText_1
        {
            get { return static_text_1; }
            set
            {
                SetProperty(ref static_text_1, value);
            }
        }
        string static_text_2;
        public string StaticText_2
        {
            get { return static_text_2; }
            set
            {
                SetProperty(ref static_text_2, value);
            }
        }
        string static_text_3;
        public string StaticText_3
        {
            get { return static_text_3; }
            set
            {
                SetProperty(ref static_text_3, value);
            }
        }
        string ca_key_status;
        public string CAKeyStatus
        {
            get { return ca_key_status; }
            set
            {
                SetProperty(ref ca_key_status, value);
            }
        }
        string ca_key_creation;
        public string CAKeyCreation
        {
            get { return ca_key_creation; }
            set
            {
                SetProperty(ref ca_key_creation, value);
            }
        }
        string qa_key_status;
        public string QAKeyStatus
        {
            get { return qa_key_status; }
            set
            {
                SetProperty(ref qa_key_status, value);
            }
        }
        string qa_key_creation;
        public string QAKeyCreation
        {
            get { return qa_key_creation; }
            set
            {
                SetProperty(ref qa_key_creation, value);
            }
        }
        string atm_key_status;
        public string ATMKeyStatus
        {
            get { return atm_key_status; }
            set
            {
                SetProperty(ref atm_key_status, value);
            }
        }
        string atm_key_creation;
        public string ATMKeyCreation
        {
            get { return atm_key_creation; }
            set
            {
                SetProperty(ref atm_key_creation, value);
            }
        }
        
        DateTime expiry_date;
        public DateTime ExpiryDate
        {
            get { return expiry_date; }
            set
            {
                SetProperty(ref expiry_date, value);
            }
        }
        bool enable_password;
        public bool EnablePassword
        {
            get { return enable_password; }
            set
            {
                SetProperty(ref enable_password, value);
            }
        }

    }
}
