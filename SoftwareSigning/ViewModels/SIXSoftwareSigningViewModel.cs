using Infrastructure;
using Infrastructure.Exceptions;
using Unity;
using Microsoft.Win32;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using SoftwareSigning.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using TracingModule;
using Infrastructure.Certificates;

namespace SoftwareSigning.ViewModels
{
   
    public partial class SIXSoftwareSigningViewModel : BindableBase, INavigationAware
    {
        ObservableCollection<PackageSecurityInfoModel> security_info;
        IUnityContainer _container;
        public ICommand ShowSenderCertificateCommand { get; private set; }
        public ICommand ShowSignerCertificateCommand { get; private set; }
        public ICommand SignForATMCommand { get; private set; }
        public ICommand ExportATMPackageCommand { get; private set; }
        public ICommand SignPackage { get; private set; }
        public ObservableCollection<PackageSecurityInfoModel> SecurityInfo
        {
            get { return security_info; }

        }
        public Action<string,string,string> LoadAllVersions = null;
        public Action<string,string,string,string> SelectVersion = null;
        public System.Security.Cryptography.X509Certificates.X509Certificate VerificationCertificate { get; set; }
        public System.Security.Cryptography.X509Certificates.X509Certificate SigningCertificate { get; set; }
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

        public string Signer { get; set; }
        public string PackageProvider { get; set; }
        public string SignerType { get; set; }
        public string PackageName { get; set; }
        public string ErrorMessage { get; set; }
        public bool PackageVerification
        {
            get;
            set;
        }
        KeyStatusModel ks;
        public KeyStatusModel KeyStatus
        {
            get { return ks; }
            set
            {
                SetProperty(ref ks, value);
            }
        }
        Visibility atm_signing;
        public Visibility ATMSigning
        {
            get { return atm_signing; }
            set
            {
                SetProperty(ref atm_signing, value);
            }
        }

        public string SelectedVersion { get; set; }
        //public bool ManuCertAvailable { get; set; }

        public SIXSoftwareSigningViewModel(IUnityContainer container)
        {
            security_info = new ObservableCollection<PackageSecurityInfoModel>();
            _container = container;
            ToolbarViewModel = _container.Resolve<SoftwareSigningToolbarViewModel>();
            PackageVerification = false;
            ShowSenderCertificateCommand = new DelegateCommand(this.OnShowSenderCertificate);
            ShowSignerCertificateCommand = new DelegateCommand(this.OnShowSignerCertificate);
            SignForATMCommand=new DelegateCommand(this.OnSignForATM);
            ExportATMPackageCommand = new DelegateCommand(this.OnExportATMPackage);
            SignPackage = new DelegateCommand(this.OnSignPackage);
            KeyStatus = new KeyStatusModel();
        }
        

        public object NavigationURI { get; private set; }
        public void PackageDropf(string fn) 
        {
            var packagedrop = _container.Resolve<SoftwareSigningToolbarViewModel>();
            packagedrop.fn = fn;
            packagedrop.OnImportPackage();
        }
        public void OnSignPackage() 
        {
            SoftwareSigningToolbarViewModel tbvm = _container.Resolve<SoftwareSigningToolbarViewModel>();
            tbvm.OnSignPackage();
        }
        public void OnExportATMPackage()
        {
            PackageProcessing pp = _container.Resolve<PackageProcessing>();
            SoftwareSigningToolbarViewModel tbvm = _container.Resolve<SoftwareSigningToolbarViewModel>();
            SIXSoftwareSigningStatusBarViewModel sbvm = _container.Resolve<SIXSoftwareSigningStatusBarViewModel>();
            SIGNER s = Converter.Signer(SignerType);
            MANUFACTURER m = Converter.Manu(PackageProvider);
            ENVIROMENT e = Converter.Env(Enviroment);
            STORETYPE st = Converter.ST(StoreType);
            if(PI==null || string.IsNullOrEmpty(PI.Version)==true || string.IsNullOrEmpty(PackageName)==true)
            {
                sbvm.Error("EXPORT PACKAGE", "No Package selected.");
                return;
            }
            SaveFileDialog dialog = new SaveFileDialog();
            
            dialog.Filter = "Zip (*.zip)|*.zip|All files (*.*)|*.*";
            dialog.FileName = pp.GetPackageFileName(PackageProvider, s, st,e, PI.Version, PackageName) + ".zip";
            if (dialog.ShowDialog() == false)
                return;
            string tp = dialog.FileName;
            if (string.IsNullOrEmpty(Path.GetFileName(tp)) == false)
                tp = Path.GetDirectoryName(tp);
            pp.ExportATMPackage(m, e, s, st, PI.Version, PackageName, tp);
            tbvm.ErrorMessage = string.Empty;
            sbvm.Success("EXPORT PACKAGE", "Package successfull exported.");
            tbvm.Log(LogData.OPERATION.EXPORT, LogData.RESULT.EXPORT_SUCCESS, this);
        }
        public void OnSignForATM()
        {
            SoftwareSigningToolbarViewModel tbvm = _container.Resolve<SoftwareSigningToolbarViewModel>();
            SIXSoftwareSigningStatusBarViewModel sbvm = _container.Resolve<SIXSoftwareSigningStatusBarViewModel>();
            if (string.IsNullOrEmpty(SelectedVersion)==true || string.IsNullOrEmpty(PackageName)==true)
            {
                tbvm.ErrorMessage = "NO PACKAGE SELECTED";
                ErrorMessage= "NO PACKAGE SELECTED";                
                sbvm.Error("PACKAGE PARSING", ErrorMessage);
                return;
            }
            PackageProcessing pp = _container.Resolve<PackageProcessing>();          
            SIGNER s = Converter.Signer(Origin);
            STORETYPE st = Converter.ST(StoreType);
            ENVIROMENT e = Converter.Env(Enviroment);
            string ep = pp.GetVersionExtractionPath(PackageProvider, s, st,e, SelectedVersion, PackageName);
            PackageInfo pi;
            try
            {
                pi = pp.ReadPackageInfo(ep, PackageProvider);

            }
            catch (Exception ex)
            {
                tbvm.ErrorMessage = ex.Message;
                tbvm.Log(LogData.OPERATION.SIGNING, LogData.RESULT.SIGNING_ERROR, this);
                sbvm.Error("PACKAGE PARSING", ErrorMessage);                
                return;
            }
            PI = new PackageInfoModel(pi);
            ENVIROMENT env = ENVIROMENT.TEST;            
            CERTTYPE ct = CERTTYPE.ATM;            
            MANUFACTURER provider = Converter.Manu(PackageProvider);
            MANUFACTURER m = MANUFACTURER.SIX;
            string pwd = "1234";
            SecurityProcessing sec = _container.Resolve<SecurityProcessing>();
            try
            {
                //create new setup info
                tbvm.ErrorMessage = string.Empty;
                pp.MakeSetupInfo(provider, s, st,env, PI.Version, PackageName);
                sec.GeneratePackageSignature(st, m, env, ct, pi, pwd);
                tbvm.Log(LogData.OPERATION.SIGNING, LogData.RESULT.SIGNING_SUCCESS, this);
                sbvm.Success("PACKAGE SIGNING", "Package signed successful");
                SecurityProcessingModel sp = new SecurityProcessingModel();                
                sp.DetermineSigningStatus(_container);
                //DetermineSigningStatus();
                //ExportEnabled = true;
            }
            catch (StorePasswordExceptions)
            {
                tbvm.ErrorMessage = "Unmanged keystore for ATM does not exist";
                tbvm.Log(LogData.OPERATION.SIGNING, LogData.RESULT.SIGNING_ERROR, this);
                sbvm.Error("PACKAGE SIGNING", tbvm.ErrorMessage);
                StorePasswordSafe pwds = _container.Resolve<StorePasswordSafe>();
                pwds.DeleteStorePassword(m, env);
                //ExportEnabled = false;
            }
            catch (Exception)
            {
                tbvm.ErrorMessage = "Error during package signing.";
                tbvm.Log(LogData.OPERATION.SIGNING, LogData.RESULT.SIGNING_ERROR, this);
                sbvm.Error("PACKAGE SIGNING", "Error during package signing.");
                //ExportEnabled = false;
            }
        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            PI = null;
            PackageDrop = _container.Resolve<PackageDropModel>();
            SecurityInfo.Clear();            
            SelectedVersion = (string)navigationContext.Parameters["VERSION"];
            PackageProvider=(string)navigationContext.Parameters["PACKAGE_PROVIDER"];
            Enviroment=(string)navigationContext.Parameters["ENV"];
            if (Enviroment == "TEST")
                ATMSigning = Visibility.Visible;
            else
                ATMSigning = Visibility.Hidden;
            PackageName = (string)navigationContext.Parameters["PACKAGE_NAME"];
            VerificationCertificateOwner = "UNKNOWN";
            SetPackageProviderTitle();
            if (Origin == "SIX-ATM-DEVICE")
                PSSVisibility = Visibility.Hidden;
            else
                PSSVisibility = Visibility.Visible;
            SetKeyStatus();
            CertifiedCertificates();
            SecurityProcessingModel sp = new SecurityProcessingModel();
            if (string.IsNullOrEmpty(SelectedVersion) == false)
            {
                LoadPackageInfo();
                sp.SignatureVerification(_container);
            }
            sp.DetermineSigningStatus(_container);            
            if (Origin == "SIX-ATM-DEVICE" && PackageVerification==true)
            {
                SIXSoftwareSigningStatusBarViewModel sbvm = _container.Resolve<SIXSoftwareSigningStatusBarViewModel>();
                sbvm.Success("PACKAGE SIGNATURE VERIFICATION", "Packet successfull verified and ready for installation on ATM.");
            }
        }
        private void CertifiedCertificates() 
        {
            SignerCertificateMapping cm = _container.Resolve<SignerCertificateMapping>();
            CertifiedManufactures = new ObservableCollection<string>(cm.CertifiedManufactures(STORETYPE.KMS));
        }
        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
           
        }
        public void SetPackageProviderTitle()
        {
            SoftwareSigningToolbarViewModel toolbar = _container.Resolve<SoftwareSigningToolbarViewModel>();
            PackageProviderTitle = "PACKAGE ORIGIN " + PackageProvider;
            MANUFACTURER m = Converter.Manu(Signer);
            if(m==MANUFACTURER.SIX)
            {
                if (Origin == "SIX-ATM-DEVICE")
                    toolbar.ToolbarTitle = Origin + "-" + Enviroment;
                else if (Origin=="SIX-QA")
                    toolbar.ToolbarTitle = Origin+"-"+StoreType+"-SOFTWARE SIGNING";
                else
                    toolbar.ToolbarTitle = Origin + "-" + Enviroment + "-SOFTWARE SIGNING";
            }
            else
            {
                toolbar.ToolbarTitle = Signer+"-"+Enviroment+ "-SOFTWARE SIGNING";
            }
        }
        public bool LoadPackageInfo()
        {
            ErrorMessage = string.Empty;
            PackageVerification = false;            
            PackageProcessing pp = _container.Resolve<PackageProcessing>();
            var packagedrop=_container.Resolve<PackageDropModel>();
            SoftwareSigningToolbarViewModel tbvm = _container.Resolve<SoftwareSigningToolbarViewModel>();
            SIXSoftwareSigningStatusBarViewModel sbvm = _container.Resolve<SIXSoftwareSigningStatusBarViewModel>();

            SIGNER s = Converter.Signer(Origin);
            STORETYPE st = Converter.ST(StoreType);
            ENVIROMENT e = Converter.Env(Enviroment);
            string ep=pp.GetVersionExtractionPath(PackageProvider, s,st,e, SelectedVersion,PackageName);
            PackageInfo pi;
            try
            {
                pi = pp.ReadPackageInfo(ep,PackageProvider);
                
            }
            catch(Exception ex)
            {                
                tbvm.ErrorMessage = ex.Message;
                tbvm.Log(LogData.OPERATION.IMPORT, LogData.RESULT.IMPORT_ERROR, this);
                sbvm.Error("PACKAGE PARSING", ErrorMessage);
                PackageVerification = false;
                return false;
            }
            PI = new PackageInfoModel(pi); 
            packagedrop.LoadedPackage = PI;
            packagedrop.LoadedPackage.DropStatus = "PACKAGE LOADED SUCCESSFULL FOR "+Origin+ " "+Enviroment;
            return true;
            
        }
        //public void DetermineSigningStatus()
        //{
        //    ExportSignatureExists = "False";
        //    SecurityProcessingModel sp = new SecurityProcessingModel();
        //    sp.PackageSigningStatus(this, _container);
        //    sp.ATMSigningStatus(this, _container);
        //}
        private void OnShowSignerCertificate()
        {
            if (SigningCertificate == null)
                return;
            X509Certificate2 c = new X509Certificate2(SigningCertificate);
            IntPtr handle = Process.GetCurrentProcess().MainWindowHandle;
            X509Certificate2UI.DisplayCertificate(c,handle);
        }

        private void OnShowSenderCertificate()
        {
            if (VerificationCertificate == null)
                return;
            X509Certificate2 c = new X509Certificate2(VerificationCertificate);
            IntPtr handle = Process.GetCurrentProcess().MainWindowHandle;
            X509Certificate2UI.DisplayCertificate(c,handle);
        }
        private void SetKeyStatus()
        {
            IKeyStatusInfo ks = KeyStatusInfoFactory.Create(KeyStatus, StoreType);
            ks.SetKeyStatus(Signer, Enviroment, StoreType, Origin, _container);            
        }


    }
}
