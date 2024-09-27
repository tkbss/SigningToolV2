using Infrastructure;
using Infrastructure.Certificates;
using Unity;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;

using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;

using System.Windows.Input;
using SigningKeyManagment.Models;
using SoftwareSigning.ViewModels;

namespace SigningKeyManagment.ViewModels
{
    public partial class KeyGenerationViewModel : BindableBase , INavigationAware
    {
        UnmanagedCertificates ch;
        IUnityContainer _container;
        public string Origin { get; set; }
        string enviroment;
        string managed;
       
        public ICommand ManuKeyGeneratorEnabledCommand { get; private set; }

        public ICommand ShowCACertificateCommand { get; private set; }

        public ICommand ShowSigningCertificateQACommand { get; private set; }

        public ICommand ShowSigningCertificateATMMANUCommand { get; private set; }
        public string Manufacturer { get; set; }
        DNSNameModel dns_name = new DNSNameModel();
        HSMCertStatusModel cert_status = new HSMCertStatusModel();
        public HSMCertStatusModel CertificateStatus
        {
            get
            {
                return cert_status;
            }

        }
        public DNSNameModel DNSName
        {
            get
            {
                return dns_name;
            }
            set
            {
                SetProperty(ref dns_name, value);
            }
        }
        string _certExportDir;
        public string CertExportDir
        {
            get
            {
                return _certExportDir;
            }
            set
            {
                SetProperty(ref _certExportDir, value);
            }
        }
        string _manuCertRequest;
        public string ManuCertRequest
        {
            get
            {
                return _manuCertRequest;
            }
            set
            {
                SetProperty(ref _manuCertRequest, value);
            }
        }
        string _manuCertDrop;
        public string ManuCertDrop
        {
            get
            {
                return _manuCertDrop;
            }
            set
            {
                SetProperty(ref _manuCertDrop, value);
            }
        }
        
        public CERTTYPE CertType { get; set; }
        
        private void InitKeyDataGen(string manu,string env,string certype,string store)
        {
            
            ENVIROMENT e= Converter.Env(env);
            MANUFACTURER m = Converter.Manu(manu);
            CERTTYPE ct = Converter.CertType(certype);
            STORETYPE st = Converter.ST(store);
            DNSName.SetDefaultName(m, ct, e, st);            
            ChkAbort = true;
            ChkBackup = false;
            ChkOverride = false;
            if(ct==CERTTYPE.MANU)            
                EnablePassword = true;
            else
                EnablePassword = false;
        }
        public KeyGenerationViewModel(IUnityContainer container)
        {
           
            _container = container;
            ch = _container.Resolve<UnmanagedCertificates>();
            StaticText_1 = "SIX CA KEYS";
            StaticText_2 = "QA SIGNING KEYS";
            StaticText_3 = "ATM SIGNING KEYS";
            ManuKeyGeneratorEnabledCommand = new DelegateCommand(this.OnManuKeyGeneratorEnabled);
            ShowCACertificateCommand = new DelegateCommand(this.OnShowCACertificate);
            ShowSigningCertificateQACommand = new DelegateCommand(this.OnShowQACertificate);
            ShowSigningCertificateATMMANUCommand = new DelegateCommand(this.OnShowATMMANUCertificate);
        }
        private string GetPassword()
        {
            string pwd = string.Empty;
            MANUFACTURER m = Converter.Manu(Manufacturer);
            ENVIROMENT e = Converter.Env(Enviroment);
            StorePasswordSafe pwds = _container.Resolve<StorePasswordSafe>();
            pwd = pwds.GetStorePassword(m, e);            
            return pwd;
        }
        public void CertifiedManuDrop(string fpCert) 
        { 
            ManuCertDrop = fpCert;            
            ch.ImportCertifiedManufacturer(fpCert,Converter.ST(managed),Converter.Env(enviroment));
            LoadCertifiedManufacturer();
            SIXSoftwareSigningStatusBarViewModel sbvm = _container.Resolve<SIXSoftwareSigningStatusBarViewModel>();
            sbvm.Success("MANUFACTURER CERTIFICATE DROP", "Certified manufacturer certificate installed successfull");
        }
        private void OnShowATMMANUCertificate()
        {
            string[] m_c = Converter.SplitManuCertype(Origin);
            MANUFACTURER m = Converter.Manu(m_c[0]);
            ENVIROMENT e = Converter.Env(Enviroment);
            if (managed == Converter.ST(STORETYPE.KMS))
            {
                KMSCertificates kms = new KMSCertificates();
                var cer = kms.GetCertificate(e, CERTTYPE.ATM);
                System.Security.Cryptography.X509Certificates.X509Certificate2 ce = new System.Security.Cryptography.X509Certificates.X509Certificate2(cer);
                IntPtr h = Process.GetCurrentProcess().MainWindowHandle;
                System.Security.Cryptography.X509Certificates.X509Certificate2UI.DisplayCertificate(ce, h);
                return;
            }
            
            CERTTYPE ct;
            string pwd;
            if (m == MANUFACTURER.SIX)
            {
                ct = CERTTYPE.ATM;
                pwd = "1234";
            }
            else
            {
                ct = CERTTYPE.MANU;
                pwd = GetPassword();
            }
            
            System.Security.Cryptography.X509Certificates.X509Certificate2 c = new System.Security.Cryptography.X509Certificates.X509Certificate2(ch.GetCertificate(m,e, ct, pwd));
            IntPtr handle = Process.GetCurrentProcess().MainWindowHandle;
            System.Security.Cryptography.X509Certificates.X509Certificate2UI.DisplayCertificate(c, handle);
        }
        private void OnShowQACertificate()
        {
            string[] m_c = Converter.SplitManuCertype(Origin);
            CERTTYPE ct = CERTTYPE.QA;
            MANUFACTURER m = Converter.Manu(m_c[0]);
            ENVIROMENT e = Converter.Env(Enviroment);
            if (managed == Converter.ST(STORETYPE.KMS))
            {
                KMSCertificates kms = new KMSCertificates();
                var cer = kms.GetCertificate(e, ct);
                System.Security.Cryptography.X509Certificates.X509Certificate2 ce = new System.Security.Cryptography.X509Certificates.X509Certificate2(cer);
                IntPtr h = Process.GetCurrentProcess().MainWindowHandle;
                System.Security.Cryptography.X509Certificates.X509Certificate2UI.DisplayCertificate(ce, h);
                return;
            }
            var cert = ch.GetCertificate(m, e, ct, "1234");
            System.Security.Cryptography.X509Certificates.X509Certificate2 c = new System.Security.Cryptography.X509Certificates.X509Certificate2(cert);
            IntPtr handle = Process.GetCurrentProcess().MainWindowHandle;
            System.Security.Cryptography.X509Certificates.X509Certificate2UI.DisplayCertificate(c, handle);
        }
        private void OnShowCACertificate()
        {
            string[] m_c=Converter.SplitManuCertype(Origin);
            CERTTYPE ct = CERTTYPE.CA;
            MANUFACTURER m = Converter.Manu(m_c[0]);
            ENVIROMENT e = Converter.Env(Enviroment);
            if(managed==Converter.ST(STORETYPE.KMS))
            {                 
                KMSCertificates kms = new KMSCertificates();
                var cer= kms.GetCertificate(e, ct);
                System.Security.Cryptography.X509Certificates.X509Certificate2 ce = new System.Security.Cryptography.X509Certificates.X509Certificate2(cer);
                IntPtr h = Process.GetCurrentProcess().MainWindowHandle;
                System.Security.Cryptography.X509Certificates.X509Certificate2UI.DisplayCertificate(ce, h);
                return;
            }            
            var cert= ch.GetCertificate(m, e, ct, "1234");
            System.Security.Cryptography.X509Certificates.X509Certificate2 c=new System.Security.Cryptography.X509Certificates.X509Certificate2(cert);
            IntPtr handle = Process.GetCurrentProcess().MainWindowHandle;
            System.Security.Cryptography.X509Certificates.X509Certificate2UI.DisplayCertificate(c,handle);
        }
        private void OnManuKeyGeneratorEnabled()
        {           
            MANUFACTURER m = Converter.Manu(Manufacturer);
            if (m == MANUFACTURER.SIX)
                EnableSIXKeyGeneration(m);
            else
                EnableManuKeyGeneration(m);
        }
        

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
            
        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            StorePasswordSafe PwdManagement = _container.Resolve<StorePasswordSafe>();
            PwdManagement.PasswordChanged = false;
            ManuCertDrop= string.Empty; 
            Origin = (string)navigationContext.Parameters["MANU"];                        
            Manufacturer = Converter.SplitManuCertype(Origin)[0];
            string ct= Converter.SplitManuCertype(Origin)[1];
            Enviroment = (string)navigationContext.Parameters["ENV"];
            managed = (string)navigationContext.Parameters["MANAGED"];
            SetToolbarTitle(Origin + "-" + managed + "-" +Enviroment+ " KEY GENERATION",Origin,managed);            
            InitKeyDataGen(Manufacturer, enviroment,ct,managed);
            if (Manufacturer == Converter.Manu(MANUFACTURER.SIX) == true)
            {
                CertifiedManufacturers = System.Windows.Visibility.Visible;
                ChangeStorePassword = System.Windows.Visibility.Hidden;
                SetSIXKeyStatus(Manufacturer, enviroment, managed);
            }
            else
            {
                CertifiedManufacturers = System.Windows.Visibility.Hidden;
                ChangeStorePassword = System.Windows.Visibility.Visible;
                SetManuKeyStatus(Manufacturer,Enviroment);
            }
            
            SetExpiry();
            LoadCertifiedManufacturer();
            SIXSoftwareSigningStatusBarViewModel status_bar = _container.Resolve<SIXSoftwareSigningStatusBarViewModel>();
            status_bar.Success("KEYMANAGEMENT", "Certificate management for QA signing keys");
        }
        public void LoadCertifiedManufacturer() 
        {
            SignerCertificateMapping cm = _container.Resolve<SignerCertificateMapping>();
            CertifiedManufactures = new ObservableCollection<string>(cm.CertifiedManufactures(Converter.ST(managed)));
        }
        public void SetManuKeyStatus(string manufacturer,string env)
        {
            KeyGenerationToolbarViewModel toolbar = _container.Resolve<KeyGenerationToolbarViewModel>();
            ENVIROMENT e = Converter.Env(env);
            StaticText_1 = "SIGNING KEYS";
            StaticText_2 = "REQUEST";
            StaticText_3 = "CERTIFICATE";
            string storetype = "MANU";
            KEYStoreStatus ks = ch.GetKeyStoreStatus(e, STORETYPE.UNMANAGED, Converter.CertType(storetype), Converter.Manu(manufacturer));
            CAKeyStatus = ks.StoreStatus;
            ShowCACertificate = false;
            ShowSigningQA = false;
            if (ks.StoreStatus == KEYStoreStatus.CREATED)
            {
                if (ChkAbort == true)
                    toolbar.ManuKeyGenEnabled = false;
                else
                    toolbar.ManuKeyGenEnabled = true;
            }
            else
                toolbar.ManuKeyGenEnabled = true;

            CAKeyCreation = ks.Creation;
            QAKeyStatus = ks.RequestStatus;
            QAKeyCreation = ks.RequestCreation;
            ATMKeyStatus = ks.CertificateStatus;
            if (ATMKeyStatus == KEYStoreStatus.CREATED)
                ShowSigningATMMANU = true;
            else
                ShowSigningATMMANU = false;
            ATMKeyCreation = ks.CertificateImport;
        }
        public void SetSIXKeyStatus(string manufacturer,string enviroment,string managed)
        {
            StaticText_1 = "SIX CA KEYS";
            StaticText_2 = "QA SIGNING KEYS";
            StaticText_3 = "ATM SIGNING KEYS";
            KeyGenerationToolbarViewModel toolbar = _container.Resolve<KeyGenerationToolbarViewModel>();
            string storetype = "CA";
            KEYStoreStatus ks = ch.GetKeyStoreStatus(Converter.Env(enviroment), Converter.ST(managed), Converter.CertType(storetype), Converter.Manu(manufacturer));
            CAKeyStatus = ks.StoreStatus;
            CAKeyCreation = ks.Creation;
            if (ks.StoreStatus == KEYStoreStatus.CREATED)
            {
                ShowCACertificate = true;
                if (ChkAbort == true)
                    toolbar.CAKeyGenEnabled = false;
                else
                    toolbar.CAKeyGenEnabled = true;
            }
            else
            {
                ShowCACertificate = false;
                toolbar.CAKeyGenEnabled = true;
            }
            storetype = "QA";
            ks = ch.GetKeyStoreStatus(Converter.Env(enviroment), Converter.ST(managed), Converter.CertType(storetype), Converter.Manu(manufacturer));
            QAKeyStatus = ks.StoreStatus;
            QAKeyCreation = ks.Creation;
            if (ks.StoreStatus == KEYStoreStatus.CREATED)
            {
                ShowSigningQA = true;
                if (ChkAbort == true)
                    toolbar.QAKeyGenEnabled = false;
                else
                    toolbar.QAKeyGenEnabled = true;
            }
            else
            {
                ShowSigningQA = false;
                toolbar.QAKeyGenEnabled = true;
            }
            storetype = "ATM";
            ks = ch.GetKeyStoreStatus(Converter.Env(enviroment), Converter.ST(managed), Converter.CertType(storetype), Converter.Manu(manufacturer));
            ATMKeyStatus = ks.StoreStatus;
            ATMKeyCreation = ks.Creation;
            if (ks.StoreStatus == KEYStoreStatus.CREATED)
            {
                ShowSigningATMMANU = true;
                if (ChkAbort == true)
                    toolbar.ATMKeyGenEnabled = false;
                else
                    toolbar.ATMKeyGenEnabled = true;
            }
            else
            {
                ShowSigningATMMANU = false;
                toolbar.ATMKeyGenEnabled = true;
            }
        }
        private void SetExpiry()
        {
            DateTime n = DateTime.Now;
            TimeSpan s = new TimeSpan(15 * 365, 0, 0, 0);
            ExpiryDate = n + s;
        }
        
        private void SetToolbarTitle(string title,string signer,string store)
        {
            if(Converter.Signer(signer)==SIGNER.MANU)
            {
                KeyGenerationToolbarViewModel toolbar = _container.Resolve<KeyGenerationToolbarViewModel>();
                toolbar.KeyGenTitle = title;
                return;
            }
            if(Converter.Signer(signer) == SIGNER.QA && store==Converter.ST(STORETYPE.UNMANAGED))
            {
                KeyGenerationToolbarViewModel toolbar = _container.Resolve<KeyGenerationToolbarViewModel>();
                toolbar.KeyGenTitle = title;
                return;
            }
            if (Converter.Signer(signer) == SIGNER.ATM && store == Converter.ST(STORETYPE.UNMANAGED))
            {
                KeyGenerationToolbarViewModel toolbar = _container.Resolve<KeyGenerationToolbarViewModel>();
                toolbar.KeyGenTitle = title;
                return;
            }
            if (Converter.Signer(signer) == SIGNER.QA && store == Converter.ST(STORETYPE.KMS))
            {
                KMSKeyGenerationToolbarViewModel toolbar = _container.Resolve<KMSKeyGenerationToolbarViewModel>();
                toolbar.KeyGenTitle = title;
                return;
            }
            if (Converter.Signer(signer) == SIGNER.ATM && store == Converter.ST(STORETYPE.KMS))
            {
                KMSKeyGenerationToolbarViewModel toolbar = _container.Resolve<KMSKeyGenerationToolbarViewModel>();
                toolbar.KeyGenTitle = title;
                return;
            }
        }
        private void EnableSIXKeyGeneration(MANUFACTURER m)
        {
            KMSKeyGenerationToolbarViewModel toolbar = _container.Resolve<KMSKeyGenerationToolbarViewModel>();
            DNSName.DNSEnabled = true;
            if (ChkAbort == true)
            {
                if (CAKeyStatus == KEYStoreStatus.CREATED)
                    toolbar.CreateCACertEnabled = false;
                else
                    toolbar.CreateCACertEnabled = true;
                if (ATMKeyStatus == KEYStoreStatus.CREATED)
                    toolbar.CreateATMCertEnabled = false;
                else
                    toolbar.CreateATMCertEnabled = true;
                if (QAKeyStatus == KEYStoreStatus.CREATED)
                    toolbar.CreateQACertEnabled = false;
                else
                    toolbar.CreateQACertEnabled = true;
            }
            else
            {
                toolbar.CreateCACertEnabled = true;
                toolbar.CreateATMCertEnabled = true;
                toolbar.CreateQACertEnabled = true;
            }
        }
        private void EnableManuKeyGeneration(MANUFACTURER m)
        {
            KeyGenerationToolbarViewModel toolbar = _container.Resolve<KeyGenerationToolbarViewModel>();
            DNSName.DNSEnabled = true; 
            if (ChkAbort == true)
            {
                string storetype = "MANU";
                KEYStoreStatus ks = ch.GetKeyStoreStatus(ENVIROMENT.PROD, STORETYPE.UNMANAGED, Converter.CertType(storetype), m);
                if (ks.StoreStatus == KEYStoreStatus.CREATED)
                    toolbar.ManuKeyGenEnabled = false;
                else
                    toolbar.ManuKeyGenEnabled = true;
                return;
            }
            else
                toolbar.ManuKeyGenEnabled = true;
        }
        public void BackupStore(CERTTYPE ct)
        {
            MANUFACTURER m=Converter.Manu(Manufacturer);
            ENVIROMENT e = Converter.Env(enviroment);
            string store_path= ch.GetKeyStorePath(m, STORETYPE.UNMANAGED, e);
            string sn=ch.GetStoreName(m,e,STORETYPE.UNMANAGED,ct);
            string fn=Path.Combine(store_path, sn+".p12");
            string bp = Path.Combine(store_path, sn + ".p12.back");
            try
            {
                File.Copy(fn, bp,true);
                if (m == MANUFACTURER.SIX)
                    return;
                foreach(var f in Directory.EnumerateFiles(store_path))
                {
                    if (f == bp)
                        continue;
                    File.Delete(f);
                }
            }
            catch { }
        }
    }
}
