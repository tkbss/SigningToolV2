using Infrastructure;
using Infrastructure.Certificates;
using Prism.Commands;
using Prism.Mvvm;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Security.Cryptography.X509Certificates;

using System.Windows.Input;
using System.Windows.Media;

namespace SigningKeyManagment.Models
{
    public class HSMCertStatusModel :BindableBase
    {
        Brush ca_cert_status;
        public Brush CACertStatusStatus
        {
            get { return ca_cert_status; }
            set
            {
                SetProperty(ref ca_cert_status, value);
            }
        }
        string _selectedCertifiedManu;
        public string SelectedCertifiedManu
        {
            get
            {
                return _selectedCertifiedManu;
            }
            set
            {
                SetProperty(ref _selectedCertifiedManu, value);
            }
        }
        string ca_key_status;
        public string CAKeyStatus
        {
            get
            {
                return ca_key_status;
            }
            set
            {
                SetProperty(ref ca_key_status, value);
            }
        }
        string ca_key_creation;
        public string CAKeyCreation
        {
            get
            {
                return ca_key_creation;
            }
            set
            {
                SetProperty(ref ca_key_creation, value);
            }
        }
        bool ca_cert;
        public bool ShowCACertificate
        {
            get
            {
                return ca_cert;
            }
            set
            {
                SetProperty(ref ca_cert, value);
            }
        }
        public ICommand ShowCACertificateCommand { get; private set; }
        X509Certificate CACert;

        Brush qa_cert_status;
        public Brush QACertStatusStatus
        {
            get { return qa_cert_status; }
            set
            {
                SetProperty(ref qa_cert_status, value);
            }
        }
        string qa_key_status;
        public string QAKeyStatus
        {
            get
            {
                return qa_key_status;
            }
            set
            {
                SetProperty(ref qa_key_status, value);
            }
        }
        string qa_key_creation;
        public string QAKeyCreation
        {
            get
            {
                return qa_key_creation;
            }
            set
            {
                SetProperty(ref qa_key_creation, value);
            }
        }
        bool qa_cert;
        public bool ShowSigningQA
        {
            get
            {
                return qa_cert;
            }
            set
            {
                SetProperty(ref qa_cert, value);
            }
        }
        public ICommand ShowSigningCertificateQACommand { get; private set; }
        X509Certificate QACert;

        Brush atm_cert_status;
        public Brush ATMCertStatusStatus
        {
            get { return atm_cert_status; }
            set
            {
                SetProperty(ref atm_cert_status, value);
            }
        }
        string atm_key_status;
        public string ATMKeyStatus
        {
            get
            {
                return atm_key_status;
            }
            set
            {
                SetProperty(ref atm_key_status, value);
            }
        }
        string atm_key_creation;
        public string ATMKeyCreation
        {
            get
            {
                return atm_key_creation;
            }
            set
            {
                SetProperty(ref atm_key_creation, value);
            }
        }
        bool atm_cert;
        public bool ShowSigningATMMANU
        {
            get
            {
                return atm_cert;
            }
            set
            {
                SetProperty(ref atm_cert, value);
            }
        }
        public ICommand ShowSigningCertificateATMMANUCommand { get; private set; }
        X509Certificate ATMCert;

        public void SetStatus(CERTTYPE ct,string status,X509Certificate c,bool enable,DateTime creationdate)
        {
            switch(ct)
            {
                case CERTTYPE.CA:
                    CAKeyStatus = status;
                    if (status == "CREATED")
                        CACertStatusStatus = new SolidColorBrush(Colors.Green);
                    else
                        CACertStatusStatus = new SolidColorBrush(Colors.Red);
                    ShowCACertificate = enable;
                    CAKeyCreation = creationdate.ToShortDateString();
                    CACert = c;
                    break;
                case CERTTYPE.QA:
                    QAKeyStatus = status;
                    if(status=="CREATED")
                        QACertStatusStatus= new SolidColorBrush(Colors.Green);
                    else
                        QACertStatusStatus = new SolidColorBrush(Colors.Red);
                    ShowSigningQA = enable;
                    QAKeyCreation = creationdate.ToShortDateString();
                    QACert = c;
                    break;
                case CERTTYPE.ATM:
                    ATMKeyStatus = status;
                    if (status == "CREATED")
                        ATMCertStatusStatus = new SolidColorBrush(Colors.Green);
                    else
                        ATMCertStatusStatus = new SolidColorBrush(Colors.Red);
                    ShowSigningATMMANU = enable;
                    ATMKeyCreation = creationdate.ToShortDateString();
                    ATMCert = c;
                    break;
            }
        }

        public HSMCertStatusModel()
        {
            this.ShowCACertificateCommand = new DelegateCommand(this.OnShowCACertificate);
            this.ShowSigningCertificateQACommand = new DelegateCommand(this.OnShowQACertificate);
            this.ShowSigningCertificateATMMANUCommand = new DelegateCommand(this.OnShowATMCertificate);
            ShowCertifiedManuCertCommand = new DelegateCommand(OnShowCertifiedManuCert);
        }


        private void OnShowQACertificate()
        {
            
            System.Security.Cryptography.X509Certificates.X509Certificate2 c = new System.Security.Cryptography.X509Certificates.X509Certificate2(QACert);
            IntPtr handle = Process.GetCurrentProcess().MainWindowHandle;
            System.Security.Cryptography.X509Certificates.X509Certificate2UI.DisplayCertificate(c, handle);
        }
        private void OnShowCACertificate()
        {
            
            System.Security.Cryptography.X509Certificates.X509Certificate2 c = new System.Security.Cryptography.X509Certificates.X509Certificate2(CACert);
            IntPtr handle = Process.GetCurrentProcess().MainWindowHandle;
            System.Security.Cryptography.X509Certificates.X509Certificate2UI.DisplayCertificate(c, handle);
        }
        private void OnShowATMCertificate()
        {

            System.Security.Cryptography.X509Certificates.X509Certificate2 c = new System.Security.Cryptography.X509Certificates.X509Certificate2(ATMCert);
            IntPtr handle = Process.GetCurrentProcess().MainWindowHandle;
            System.Security.Cryptography.X509Certificates.X509Certificate2UI.DisplayCertificate(c, handle);
        }
        public ICommand ShowCertifiedManuCertCommand { get; private set; }
        private void OnShowCertifiedManuCert() 
        {
            string manu=SelectedCertifiedManu;
            SignerCertificateMapping cm = new SignerCertificateMapping();
            var c= cm.GetCertifiedManufacturer(manu);
            if (c == null)
                return;
            var CertifiedManufacture = new System.Security.Cryptography.X509Certificates.X509Certificate2(c); 
            if(CertifiedManufacture == null)
                return; 
            IntPtr handle = Process.GetCurrentProcess().MainWindowHandle;
            System.Security.Cryptography.X509Certificates.X509Certificate2UI.DisplayCertificate(CertifiedManufacture, handle);
        }
    }
}
