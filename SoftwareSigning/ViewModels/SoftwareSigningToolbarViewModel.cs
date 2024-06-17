using Infrastructure;
using Infrastructure.Exceptions;
using Unity;
using Microsoft.Win32;
using Prism.Commands;
using Prism.Mvvm;
using SoftwareSigning.Model;
using System;
using System.IO;
using System.Windows.Input;
using TracingModule;

namespace SoftwareSigning.ViewModels
{
    public class SoftwareSigningToolbarViewModel : BindableBase
    {
        public ICommand ImportPackageCommand { get; private set; }
        public ICommand SignPackageCommand { get; private set; }

        public ICommand RemovePackageCommand { get; private set; }

        public ICommand ExportPackageCommand { get; private set; }
        public ICommand VerifyPackageCommand { get; private set; }
        IUnityContainer _container;
        public string ErrorMessage { get; set; }
        public SoftwareSigningToolbarViewModel(IUnityContainer container)
        {
            this.ImportPackageCommand = new DelegateCommand(this.OnImportPackage);
            this.SignPackageCommand = new DelegateCommand(this.OnSignPackage);
            this.ExportPackageCommand = new DelegateCommand(this.OnExportPackage);
            this.RemovePackageCommand = new DelegateCommand(this.OnRemovePackage);
            this.VerifyPackageCommand = new DelegateCommand(this.OnVerifyPackage);
            _container = container;
            SigningEnabled = true;
            ExportEnabled = true;
        }
        
        string toolbar_title;
        public string ToolbarTitle
        {
            get { return toolbar_title; }
            set
            {
                SetProperty(ref toolbar_title, value);
            }
        }
        bool export_enabled;
        public bool ExportEnabled
        {
            get { return export_enabled; }
            set
            {
                SetProperty(ref export_enabled, value);
            }
        }
        bool signing_enabled;
        public bool SigningEnabled
        {
            get { return signing_enabled; }
            set
            {
                SetProperty(ref signing_enabled, value);
            }
        }
        private void OnVerifyPackage()
        {
            SIXSoftwareSigningViewModel viewdata = _container.Resolve<SIXSoftwareSigningViewModel>();
            SIXSoftwareSigningStatusBarViewModel sbvm = _container.Resolve<SIXSoftwareSigningStatusBarViewModel>();

            PackageProcessing pp = _container.Resolve<PackageProcessing>();
            SIGNER s = Converter.Signer(viewdata.Origin);
            STORETYPE st = Converter.ST(viewdata.StoreType);
            ENVIROMENT e = Converter.Env(viewdata.Enviroment);
            string ep = pp.GetVersionExtractionPath(viewdata.PackageProvider, s,st,e, viewdata.SelectedVersion, viewdata.PackageName);
            viewdata.SecurityInfo.Clear();
            PackageInfo pi;
            try
            {
                pi = pp.ReadPackageInfo(ep, viewdata.PackageProvider);

            }
            catch (Exception ex)
            {                
                sbvm.Error("PACKAGE PARSING", ex.Message);                
                return;
            }
            
            SecurityProcessingModel sp = new SecurityProcessingModel();
            sp.SignatureVerification(_container);
            if(viewdata.PackageVerification==true)
            {
                sbvm.Success("PACKAGE SIGNATURE VERIFICATION", "Packet successfull verified and ready for installation on ATM.");
            }
        }
        private void OnRemovePackage()
        {
            SIXSoftwareSigningViewModel viewdata = _container.Resolve<SIXSoftwareSigningViewModel>();
            SIXSoftwareSigningStatusBarViewModel sbvm = _container.Resolve<SIXSoftwareSigningStatusBarViewModel>();
            if (string.IsNullOrEmpty(viewdata.SelectedVersion)==true)
            {
                sbvm.Error("REMOVE PACAKGE", "No version selected. Cannot remove package.");
                return;
            }
            PackageProcessing pp = _container.Resolve<PackageProcessing>();
            try
            {
                SIGNER s = Converter.Signer(viewdata.SignerType);
                MANUFACTURER m = Converter.Manu(viewdata.PackageProvider);
                STORETYPE st = Converter.ST(viewdata.StoreType);
                ENVIROMENT e = Converter.Env(viewdata.Enviroment);
                pp.RemovePackage(m, s,st,e, viewdata.SelectedVersion,viewdata.PackageName);
                Log(LogData.OPERATION.REMOVE, LogData.RESULT.REMOVE_SUCCESS, viewdata);
                sbvm.Success("REMOVE PACKAGE", "Package successfull removed.");
                viewdata.SecurityInfo.Clear();
                viewdata.PI = null;
                SecurityProcessingModel sp = new SecurityProcessingModel();
                sp.PackageSigningStatus(viewdata, _container);
                viewdata.LoadAllVersions(viewdata.Origin,viewdata.StoreType, viewdata.Enviroment);
            }
            catch(Exception)
            {
                sbvm.Error("REMOVE PACKAGE", "Error during remove operation.");
            }
           

        }
        private void OnExportPackage()
        {
            SIXSoftwareSigningStatusBarViewModel sbvm = _container.Resolve<SIXSoftwareSigningStatusBarViewModel>();
            PackageProcessing pp = _container.Resolve<PackageProcessing>();
            SIXSoftwareSigningViewModel signingview = _container.Resolve<SIXSoftwareSigningViewModel>();
            try
            {               

                SIGNER s = Converter.Signer(signingview.SignerType);
                MANUFACTURER m = Converter.Manu(signingview.PackageProvider);
                ENVIROMENT e = Converter.Env(signingview.Enviroment);
                STORETYPE st = Converter.ST(signingview.StoreType);
                SaveFileDialog dialog = new SaveFileDialog();
                if (pp.PackageSigFileExists(m,e, s,st, signingview.PI.Version,signingview.PackageName) == false)
                {
                    sbvm.Error("EXPORT PACKAGE", "No signature file exists. Package export not possible");
                    return;
                }
                dialog.Filter = "Zip (*.zip)|*.zip|All files (*.*)|*.*";
                dialog.FileName = pp.GetPackageFileName(signingview.PackageProvider, s,st,e, signingview.PI.Version,signingview.PackageName) + ".zip";
                if (dialog.ShowDialog() == false)
                    return;
                string tp = dialog.FileName;
                if (string.IsNullOrEmpty(Path.GetFileName(tp)) == false)
                    tp = Path.GetDirectoryName(tp);
                pp.ExportPackage(m,e, s,st, signingview.PI.Version,signingview.PackageName, tp);
                ErrorMessage = string.Empty;                
                sbvm.Success("EXPORT PACKAGE", "Package successfull exported.");
                Log(LogData.OPERATION.EXPORT, LogData.RESULT.EXPORT_SUCCESS, signingview);
            }
            catch(Exception e)
            {
                ErrorMessage = e.Message;
                Log(LogData.OPERATION.EXPORT, LogData.RESULT.EXPORT_ERROR, signingview);
                sbvm.Error("EXPORT PACKAGE", "Processing error: "+e.Message);
            }
        }
        public void OnSignPackage()
        {
            SIXSoftwareSigningViewModel signing = _container.Resolve<SIXSoftwareSigningViewModel>();            
            SecurityProcessing sec = _container.Resolve<SecurityProcessing>();
            SIXSoftwareSigningStatusBarViewModel sbvm = _container.Resolve<SIXSoftwareSigningStatusBarViewModel>();
            UnmanagedCertificates uc = _container.Resolve<UnmanagedCertificates>();
            PackageProcessing pp = _container.Resolve<PackageProcessing>();
            ErrorMessage = string.Empty;

            string[] manu_ct = Converter.SplitManuCertype(signing.Origin);
            string manu = manu_ct[0];
            string ct_s = manu_ct[1];
            MANUFACTURER m = Converter.Manu(manu);
            ENVIROMENT e = Converter.Env(signing.Enviroment);
            
            string pwd = LoadStorePassword(m,e);
            if (string.IsNullOrEmpty(pwd) == true)
            {
                ErrorMessage = "Password for key store not set";
                Log(LogData.OPERATION.SIGNING, LogData.RESULT.SIGNING_ERROR, signing);
                sbvm.Error("PACKAGE SIGNING", ErrorMessage);                
                return;
            }
            signing.SecurityInfo.Clear();
            signing.LoadPackageInfo();
            SecurityProcessingModel sp = new SecurityProcessingModel();
            sp.SignatureVerification(_container);
            sp.DetermineSigningStatus(_container);

            ENVIROMENT env = Converter.Env(signing.Enviroment);
            PackageInfo pi = signing.PI.GetPackageInfo();
            STORETYPE st = Converter.ST(signing.StoreType);
            CERTTYPE ct = Converter.CertType(ct_s);
            SIGNER s = Converter.Signer(signing.SignerType);
            MANUFACTURER provider = Converter.Manu(signing.PackageProvider);
            if (m != MANUFACTURER.SIX)
            {
                if (signing.KeyStatus.ManuCertAvailable == false)
                {
                    ErrorMessage = "No certificate for this manufacturer imported. Cannot sign package";
                    Log(LogData.OPERATION.SIGNING, LogData.RESULT.SIGNING_ERROR, signing);
                    sbvm.Error("PACKAGE SIGNING", ErrorMessage);
                    SigningEnabled = false;
                    ExportEnabled = false;
                    return;
                }
            }
            //pp.RemovePackageSignature(pi);
            if (signing.PackageVerification == false)
                return;                    
            try
            {
                //create new setup info                
                ErrorMessage = string.Empty;                              
                pp.MakeSetupInfo(provider, s,st,env, signing.PI.Version,signing.PackageName);                         
                sec.GeneratePackageSignature(st, m, env, ct, pi,pwd);
                Log(LogData.OPERATION.SIGNING, LogData.RESULT.SIGNING_SUCCESS, signing);
                sbvm.Success("PACKAGE SIGNING", "Package signed successful");
                //signing.DetermineSigningStatus();
                sp.DetermineSigningStatus(_container);
                ExportEnabled = true;
            }
            catch(StorePasswordExceptions)
            {
                ErrorMessage = "Password for key store not correct.";
                Log(LogData.OPERATION.SIGNING, LogData.RESULT.SIGNING_ERROR, signing);
                sbvm.Error("PACKAGE SIGNING", ErrorMessage);
                StorePasswordSafe pwds = _container.Resolve<StorePasswordSafe>();
                pwds.DeleteStorePassword(m,env);                
                ExportEnabled = false;
            }
            catch(Exception)
            {
                ErrorMessage = "Error during package signing.";
                Log(LogData.OPERATION.SIGNING, LogData.RESULT.SIGNING_ERROR, signing);
                sbvm.Error("PACKAGE SIGNING", "Error during package signing.");
                ExportEnabled = false;
            }
        }
        public void OnImportPackage()
        {
            SIXSoftwareSigningStatusBarViewModel sbvm = _container.Resolve<SIXSoftwareSigningStatusBarViewModel>();
            //SIGNER s = Converter.Signer(signing.Origin);
            //MANUFACTURER m = Converter.Manu(Converter.SplitManuCertype(signing.Origin)[0]);
            OpenFileDialog openFileDialog = new OpenFileDialog();
            SIXSoftwareSigningViewModel signing = _container.Resolve<SIXSoftwareSigningViewModel>();           
            openFileDialog.Filter = "Packages (*.zip)|*.zip|All files (*.*)|*.*";
            string fn;
            if (openFileDialog.ShowDialog() == true)
            {
                fn = openFileDialog.FileName;
            }
            else
            {
                //write error to status bar and return
                sbvm.Error("IMPORT PACAKGE", "Import operation aborted by user.");
                return;
            }
            PackageInfo pi = null;
            ErrorMessage = string.Empty;
            if (PackageExtraction(fn, signing, sbvm,out pi) == false)
            {
                signing.PI = new PackageInfoModel(pi);
                Log(LogData.OPERATION.IMPORT, LogData.RESULT.IMPORT_ERROR, signing);
                return;
            }
            signing.PI = new PackageInfoModel(pi);
            if(signing.LoadPackageInfo()==true)
            {                
                ErrorMessage = string.Empty;
                Log(LogData.OPERATION.IMPORT, LogData.RESULT.IMPORT_SUCCESS, signing);
                SecurityProcessingModel sp = new SecurityProcessingModel();
                sp.SignatureVerification(_container);
                sp.DetermineSigningStatus(_container);
            }
            else
            {
                ErrorMessage = "Error in parsing setup info";
                Log(LogData.OPERATION.IMPORT, LogData.RESULT.IMPORT_ERROR, signing);
            }                      
            signing.LoadAllVersions(signing.SignerType,signing.StoreType,signing.Enviroment);
            signing.SelectVersion(signing.PackageProvider, signing.SelectedVersion,signing.Origin,signing.PackageName);                         
        }
        private bool PackageExtraction(string fn, SIXSoftwareSigningViewModel signing, SIXSoftwareSigningStatusBarViewModel sbvm,out PackageInfo pi)
        {
            
            PackageProcessing pp = _container.Resolve<PackageProcessing>();
            //PackageInfo pi;
            SIGNER s = Converter.Signer(signing.Origin);
            MANUFACTURER m = Converter.Manu(Converter.SplitManuCertype(signing.Origin)[0]);
            STORETYPE st = Converter.ST(signing.StoreType);
            ENVIROMENT e = Converter.Env(signing.Enviroment);
            if (pp.ValidateFileName(fn, out pi) == false)
            {
                //write error to status bar and return 
                ErrorMessage = "Package name incorrect. Package not imported.";
                sbvm.Error("IMPORT PACKAGE", ErrorMessage);
                return false;
            }
            if (pp.MapPackageName(pi, m) == false)
            {
                ErrorMessage = "Package not for this manufacturer.";
                sbvm.Error("IMPORT PACKAGE", ErrorMessage);
                return false;
            }

            if (pp.Extraction(s,st,e, pi, fn) == false)
            {
                //write error to status bar
                ErrorMessage = pp.Error;
                sbvm.Error("IMPORT PACKAGE", pp.Error);
                pp.RemovePackage(m, s,st,e, pi.Version,pi.Name);
                return false;
            }
            if(pp.CheckSetupInfo(pi)==false)
            {
                ErrorMessage = pp.Error;
                sbvm.Error("IMPORT PACKAGE", pp.Error);
                pp.RemovePackage(m, s,st,e, pi.Version, pi.Name);
                return false;
            }
            signing.PackageProvider = Converter.Vendor(pi.Vendor);
            signing.SelectedVersion = pi.Version;
            signing.PackageName=pi.Name;
            return true;
        }
        private string LoadStorePassword(MANUFACTURER m,ENVIROMENT e)
        {
            StorePasswordSafe pwds = _container.Resolve<StorePasswordSafe>();            
            if (m==MANUFACTURER.SIX)
            {
                pwds.SetStorePassword(m,e,"1234");
                return "1234";
            }
            else
                return pwds.GetStorePassword(m, e);            
        }
        public void Log(LogData.OPERATION op,LogData.RESULT res, SIXSoftwareSigningViewModel signing)
        {
            LogData log = _container.Resolve<LogData>();
            
            log.Operation = op;
            log.OperationResult = res;
            log.e = Converter.Env(signing.Enviroment);
            if(op!= LogData.OPERATION.MANU_CERT_SIGN)                
                log.Signer = Converter.Manu(signing.Signer);
            log.SignerType = Converter.Signer(signing.SignerType);
            log.StoreType = Converter.ST(signing.StoreType);
            log.ErrorMessage = ErrorMessage;           
            int index=log.Log();
            if (signing.PI != null)
            {
                TracePackageData pd = new TracePackageData();
                pd.Date = signing.PI.PackageDate;
                pd.PackageName = signing.PI.Name;
                pd.Vendor = signing.PI.Vendor;
                pd.Version = signing.PI.Version;
                log.LogPackage(pd, index);
            }
            //ErrorMessage = string.Empty;
        }
    }
}
