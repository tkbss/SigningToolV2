using Infrastructure;
using Infrastructure.Certificates;
using Infrastructure.HSM;
using Unity;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using SigningKeyManagment.Models;
using SoftwareSigning.ViewModels;

using System.Collections.ObjectModel;

using System.Windows.Input;
using System.Windows.Media;

namespace SigningKeyManagment.ViewModels
{
    public class HSMStatusViewModel : BindableBase , INavigationAware
    {
        ObservableCollection<HSMStatusModel> hsm_status= new ObservableCollection<HSMStatusModel>();

        ObservableCollection<HSMKeyStatusModel> qa_keys= new ObservableCollection<HSMKeyStatusModel>();
        ObservableCollection<HSMKeyStatusModel> ca_keys= new ObservableCollection<HSMKeyStatusModel>();
        ObservableCollection<HSMKeyStatusModel> signing_keys= new ObservableCollection<HSMKeyStatusModel>();
        HSMCertStatusModel cert_status = new HSMCertStatusModel();
        IUnityContainer _container;
        Infrastructure.HSM.HSM hsm;
        string enviroment;
        string password_enter;
        
        public string Enviroment
        {
            get { return enviroment; }
        }
        public ICommand CheckPwdCommand { get; private set; }
        
        public string PasswordEnter
        {
            get { return password_enter; }
            set
            {
                SetProperty(ref password_enter, value);
            }
        }
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
        Brush pwd_status;
        public Brush PasswordStatus
        {
            get { return pwd_status; }
            set
            {
                SetProperty(ref pwd_status, value);
            }
        }
        public ObservableCollection<HSMStatusModel> HSMStatus
        {
            get
            {
                return hsm_status;
            }
            
        }
        public HSMCertStatusModel CertificateStatus
        {
            get
            {
                return cert_status;
            }

        }
        public ObservableCollection<HSMKeyStatusModel> QAKeys
        {
            get
            {
                return qa_keys;
            }

        }
        public ObservableCollection<HSMKeyStatusModel> SigningKeys
        {
            get
            {
                return signing_keys;
            }

        }
        public ObservableCollection<HSMKeyStatusModel> CAKeys
        {
            get
            {
                return ca_keys;
            }

        }
        public HSMStatusViewModel(IUnityContainer container)
        {
            _container = container;
            hsm=_container.Resolve<Infrastructure.HSM.HSM>();
            this.CheckPwdCommand = new DelegateCommand(this.OnCheckPwd);
            PasswordStatus = new SolidColorBrush(Colors.Red);
        }
        public void OnCheckPwd()
        {
            SIXSoftwareSigningStatusBarViewModel status_bar = _container.Resolve<SIXSoftwareSigningStatusBarViewModel>();
            if (hsm.CheckPassword(PasswordEnter, enviroment) == false)
            {
                PasswordStatus = new SolidColorBrush(Colors.Red);                
                status_bar.Error("CHECK PASSWORD", "WRONG PASSWORD ENTERED. Try again");
            }
            else
            {
                PasswordStatus = new SolidColorBrush(Colors.Green);
                SetupKeyStatus();
                status_bar.Success("CHECK PASSWORD", "Correct password entered!");
                
            }
        }
        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            //throw new NotImplementedException();
            return true;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
            //throw new NotImplementedException();
        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            enviroment = (string)navigationContext.Parameters["ENV"];
            string Origin = (string)navigationContext.Parameters["MANU"];            
            SetToolbarTitle(Origin + " HSM AND CERTIFICATE STATUS");
            hsm_status.Clear();
            foreach (HSMStatusInfo i in hsm.HSMStatus(enviroment))
            {
                hsm_status.Add(new HSMStatusModel(i));
            }
            SetupKeyStatus();
            SetUpCertStatus();
            SignerCertificateMapping cm = _container.Resolve<SignerCertificateMapping>();
            CertifiedManufactures = new ObservableCollection<string>(cm.CertifiedManufactures(STORETYPE.KMS));
        }
        private void SetToolbarTitle(string title)
        {
            KMSKeyGenerationToolbarViewModel toolbar = _container.Resolve<KMSKeyGenerationToolbarViewModel>();
            toolbar.KeyGenTitle = title;
        }
        public void SetUpCertStatus()
        {
            KMSCertificates certs = new KMSCertificates();
            KMSKeyGenerationToolbarViewModel toolbar = _container.Resolve<KMSKeyGenerationToolbarViewModel>();
            //cert_status.Clear();
            string[] cts = { "CA", "QA", "ATM" };
            string[] cns = { "CA Certificate", "QA Signing", "ATM Signing" };
            string[] t = { "CA", "SIGNING", "SIGNING" };
            int i = 0;
            bool enable = false;
            foreach (string ct in cts)
            {
                CERTTYPE cert_t = Converter.CertType(ct);
                System.Security.Cryptography.X509Certificates.X509Certificate c = certs.GetCertificate(enviroment, ct);               
                if (c == null)
                {
                    string status= "MISSING";
                    DateTime cd = certs.GetCertificateCreationDate(Converter.Env(enviroment),cert_t);
                    CertificateStatus.SetStatus(cert_t, status, c, false, cd);                    
                    enable = true;
                    
                }
                else
                {
                    string status = "CREATED";
                    DateTime cd = certs.GetCertificateCreationDate(Converter.Env(enviroment), cert_t);
                    CertificateStatus.SetStatus(cert_t, status, c, true, cd);
                    enable = false;
                }                
                EnableToolbarButton(toolbar, enable, ct);
                i++;
            }
            
        }
        private void EnableToolbarButton(KMSKeyGenerationToolbarViewModel toolbar,bool enable,string cert_type)
        {
            bool ca_cert_available = false;
            var k = ca_keys.Where(e => e.KeyStatus == "ESTABLISHED");
            if (k == null || k.Count() == 0)
            {
                ca_cert_available = false;
            }
            else
            {
                ca_cert_available = true;
            }
            if (cert_type == Converter.CertType(CERTTYPE.ATM))
            {
                if (ca_cert_available == false)
                {                    
                    toolbar.CreateATMCertEnabled = false;
                }
                else
                {                    
                    toolbar.CreateATMCertEnabled = enable;                    
                }
                
            }
            if (cert_type == Converter.CertType(CERTTYPE.QA))
            {
                toolbar.CreateQACertEnabled = enable;
                if (toolbar.CreateQACertEnabled == false)
                    toolbar.SignRequestEnabled = true;
                else
                    toolbar.SignRequestEnabled = false;
            }
            if (cert_type == Converter.CertType(CERTTYPE.CA))
            {
                if (ca_cert_available == false)
                    toolbar.CreateCACertEnabled = false;
                else
                    toolbar.CreateCACertEnabled = enable;
            }
            

        }
        private void SetupKeyStatus()
        {
            SIXSoftwareSigningStatusBarViewModel status_bar = _container.Resolve<SIXSoftwareSigningStatusBarViewModel>();
            if(hsm.PasswordCheckSuccessfull==false)
            {
                status_bar.Error("SetupKeyStatus", "HSM Token authentication missing");
            }
            qa_keys.Clear();
            ca_keys.Clear();
            signing_keys.Clear();
            List<KEY_STATUS> status = hsm.KeyStatus(enviroment);
            foreach (var s in status)
            {
                if (s.KEY_NAME.ToLower().Contains("qa") == true)
                    qa_keys.Add(new HSMKeyStatusModel(s));
                else if (s.KEY_NAME.ToLower().Contains("ca") == true)
                     ca_keys.Add(new HSMKeyStatusModel(s));
                 else if (s.KEY_NAME.ToLower().Contains("sign") == true)
                        signing_keys.Add(new HSMKeyStatusModel(s));                
            }           
            
        }
    }
}
