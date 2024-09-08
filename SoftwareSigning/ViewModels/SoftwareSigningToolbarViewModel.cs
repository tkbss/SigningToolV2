using Infrastructure;
using Infrastructure.Exceptions;
using Microsoft.Win32;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using SoftwareSigning.Model;
using System.IO;
using System.Windows.Input;
using Unity;
using System.Windows;

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
        public string fn { get; set; }
        public SoftwareSigningToolbarViewModel(IUnityContainer container)
        {
            this.ImportPackageCommand = new DelegateCommand(async()=>
            {
                await OnImportPackage();
            });
            this.SignPackageCommand = new DelegateCommand(async () =>
            {
                await OnSignPackage();
            });

            this.ExportPackageCommand = new DelegateCommand(async () =>
            {
                await OnExportPackageAsync();
            });
            this.RemovePackageCommand = new DelegateCommand(this.OnRemovePackage);
            this.VerifyPackageCommand = new DelegateCommand(this.OnVerifyPackage);
            _container = container;
            SigningEnabled = true;
            ExportEnabled = true;
            IsOperationInProgress = false;
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
        bool _isOperationInProgress;
        public bool IsOperationInProgress
        {
            get { return _isOperationInProgress; }
            set
            {
                SetProperty(ref _isOperationInProgress, value);
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
            PackageDropModel pdm=_container.Resolve<PackageDropModel>();
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
                if (s == SIGNER.QA)
                    pdm.DropedPackage = null;
                if(s==SIGNER.ATM)
                    pdm.LoadedPackage = null;
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
        public async Task OnCopyPackageAsync() 
        {
            SIXSoftwareSigningStatusBarViewModel sbvm = _container.Resolve<SIXSoftwareSigningStatusBarViewModel>();
            SIXSoftwareSigningViewModel signing = _container.Resolve<SIXSoftwareSigningViewModel>();
            PackageProcessing pp = _container.Resolve<PackageProcessing>();
            SIGNER s = Converter.Signer(signing.SignerType);
            MANUFACTURER m = Converter.Manu(signing.PackageProvider);
            ENVIROMENT e = Converter.Env(signing.Enviroment);
            STORETYPE st = Converter.ST(signing.StoreType);
            if (s == SIGNER.ATM && e == ENVIROMENT.PROD)
                return;
            PackageInfo pi = signing.PI.GetPackageInfo();
            string fn = pp.GetPackageFileName(signing.PackageProvider, s, st, e, signing.PI.Version, signing.PackageName);            
            if (pp.PackageSigFileExists(m, e, s, st, signing.PI.Version, signing.PackageName) == false)
            {
                sbvm.Error("COPY PACKAGE", "No signature file exists. Package copy not possible");
                return;
                
            }
            ENVIROMENT targetENV=e;
            SIGNER targetS=s;
            if (s==SIGNER.QA)
            {
                targetENV = ENVIROMENT.TEST;
                targetS = SIGNER.ATM;
            }
            if(s==SIGNER.ATM)
            {
                targetS = SIGNER.ATM;
                targetENV = ENVIROMENT.PROD;
            }
            
            await pp.CopyPackageAsync(m,e,s,st, signing.PI.Version, signing.PackageName,targetENV,  targetS);
        }
        private async Task OnExportPackageAsync()
        {
            IsOperationInProgress = true;
            SIXSoftwareSigningStatusBarViewModel sbvm = _container.Resolve<SIXSoftwareSigningStatusBarViewModel>();
            sbvm.Success("EXPORT PACKAGE", "Start export operation");
            PackageProcessing pp = _container.Resolve<PackageProcessing>();
            SIXSoftwareSigningViewModel signingview = _container.Resolve<SIXSoftwareSigningViewModel>();
            PackageDropModel pdm = _container.Resolve<PackageDropModel>();
            try
            {               

                SIGNER s = Converter.Signer(signingview.SignerType);
                MANUFACTURER m = Converter.Manu(signingview.PackageProvider);
                ENVIROMENT e = Converter.Env(signingview.Enviroment);
                STORETYPE st = Converter.ST(signingview.StoreType);
                string targetPath = string.Empty;
                if (pp.PackageSigFileExists(m, e, s, st, signingview.PI.Version, signingview.PackageName) == false)
                {
                    sbvm.Error("EXPORT PACKAGE", "No signature file exists. Package export not possible");
                    IsOperationInProgress = false;
                    return;
                }
                if (s==SIGNER.MANU || s==SIGNER.QA)
                {
                    await FileDialogExportAsync(sbvm, signingview, pp);
                    IsOperationInProgress = false;
                    return;
                }
                if(e==ENVIROMENT.TEST && string.IsNullOrEmpty(pdm.ExportTest) && !Directory.Exists(pdm.ExportTest))
                {
                    await FileDialogExportAsync(sbvm, signingview, pp);
                    IsOperationInProgress = false;
                    return;
                }                
                if (e == ENVIROMENT.PROD && string.IsNullOrEmpty(pdm.ExportProd) && !Directory.Exists(pdm.ExportProd))
                {
                    await FileDialogExportAsync(sbvm, signingview, pp);
                    IsOperationInProgress = false;
                    return;
                }
                if(e == ENVIROMENT.TEST)
                {
                    targetPath = pdm.ExportTest;
                }
                else
                {
                    targetPath = pdm.ExportProd;
                }
                await pp.ExportPackage(m,e, s,st, signingview.PI.Version,signingview.PackageName, targetPath);
                ErrorMessage = string.Empty;                
                sbvm.Success("EXPORT PACKAGE", "Package successfull exported to:"+targetPath);
                Log(LogData.OPERATION.EXPORT, LogData.RESULT.EXPORT_SUCCESS, signingview);
                IsOperationInProgress = false;
            }
            catch(Exception e)
            {
                ErrorMessage = e.Message;
                Log(LogData.OPERATION.EXPORT, LogData.RESULT.EXPORT_ERROR, signingview);
                sbvm.Error("EXPORT PACKAGE", "Processing error: "+e.Message);
                IsOperationInProgress = false;
            }
        }
        private async Task FileDialogExportAsync(SIXSoftwareSigningStatusBarViewModel sbvm, SIXSoftwareSigningViewModel signingview, PackageProcessing pp) 
        {
            SIGNER s = Converter.Signer(signingview.SignerType);
            MANUFACTURER m = Converter.Manu(signingview.PackageProvider);
            ENVIROMENT e = Converter.Env(signingview.Enviroment);
            STORETYPE st = Converter.ST(signingview.StoreType);
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.Filter = "Zip (*.zip)|*.zip|All files (*.*)|*.*";
            dialog.FileName = pp.GetPackageFileName(signingview.PackageProvider, s, st, e, signingview.PI.Version, signingview.PackageName) + ".zip";
            if (dialog.ShowDialog() == false)
            {
                sbvm.Error("EXPORT PACKAGE", "Export dialog aborted. No package exported");
                return;
            }
            string tp = dialog.FileName;
            if (string.IsNullOrEmpty(Path.GetFileName(tp)) == false)
                tp = Path.GetDirectoryName(tp);
            await pp.ExportPackage(m, e, s, st, signingview.PI.Version, signingview.PackageName, tp);
            ErrorMessage = string.Empty;
            sbvm.Success("EXPORT PACKAGE", "Package successfull exported to: "+tp);
            Log(LogData.OPERATION.EXPORT, LogData.RESULT.EXPORT_SUCCESS, signingview);
        }
        public async Task OnSignPackage()
        {
            IsOperationInProgress = true;            
            SIXSoftwareSigningViewModel signing = _container.Resolve<SIXSoftwareSigningViewModel>();            
            SecurityProcessing sec = _container.Resolve<SecurityProcessing>();
            SIXSoftwareSigningStatusBarViewModel sbvm = _container.Resolve<SIXSoftwareSigningStatusBarViewModel>();
            sbvm.Success("PACKAGE SIGNING", "Start signing process.");
            UnmanagedCertificates uc = _container.Resolve<UnmanagedCertificates>();
            PackageProcessing pp = _container.Resolve<PackageProcessing>();
            //var navi=_container.Resolve<NavigationViewModel>();
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
                IsOperationInProgress = false;
                return;
            }
            signing.SecurityInfo.Clear();
            if(signing.LoadPackageInfo()==false)
                return;
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
                    IsOperationInProgress = false;
                    return;
                }
            }
            
            if (signing.PackageVerification == false) { IsOperationInProgress = false; return; }
                                  
            try
            {
                //create new setup info                
                ErrorMessage = string.Empty;                              
                pp.MakeSetupInfo(provider, s,st,env, signing.PI.Version,signing.PackageName);                         
                sec.GeneratePackageSignature(st, m, env, ct, pi,pwd);
                if(s==SIGNER.MANU)
                {
                    if (signing.LoadPackageInfo() == false)
                        return;
                }
                
                Log(LogData.OPERATION.SIGNING, LogData.RESULT.SIGNING_SUCCESS, signing);
                sbvm.Success("PACKAGE SIGNING", "Package signed successful");
                
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
                IsOperationInProgress = false;
            }
            catch(Exception)
            {
                ErrorMessage = "Error during package signing.";
                Log(LogData.OPERATION.SIGNING, LogData.RESULT.SIGNING_ERROR, signing);
                sbvm.Error("PACKAGE SIGNING", "Error during package signing.");
                ExportEnabled = false;
                IsOperationInProgress = false;
            }
            if(s==SIGNER.QA || s==SIGNER.ATM)
            {
                await OnCopyPackageAsync();
                var navigate = new NavigationModel(_container, _container.Resolve<IRegionManager>());
                if (s == SIGNER.QA && env == ENVIROMENT.TEST && ExportEnabled == true)
                {
                    navigate.Navigate(1);
                }
                if (s == SIGNER.ATM && env == ENVIROMENT.TEST && ExportEnabled == true)
                {
                    navigate.Navigate(2);
                }                
            }
            IsOperationInProgress = false;

        }
        public Task<string> ShowOpenFileDialogAsync()
        {
            var tcs = new TaskCompletionSource<string>();
            var openFileDialog = new OpenFileDialog
            {
                Filter = "Packages (*.zip)|*.zip|All files (*.*)|*.*"
            };

            Application.Current.Dispatcher.Invoke(() =>
            {
                if (openFileDialog.ShowDialog() == true)
                {
                    tcs.SetResult(openFileDialog.FileName);
                }
                else
                {
                    tcs.SetResult(null);
                }
            });

            return tcs.Task;
        }
        public async Task OnImportPackage()
        {
            SIXSoftwareSigningStatusBarViewModel sbvm = _container.Resolve<SIXSoftwareSigningStatusBarViewModel>();
            SIXSoftwareSigningViewModel signing = _container.Resolve<SIXSoftwareSigningViewModel>();
            SoftwareSigningToolbarViewModel tbvm = _container.Resolve<SoftwareSigningToolbarViewModel>();
            tbvm.IsOperationInProgress = true;
            SIGNER s = Converter.Signer(signing.Origin);
            MANUFACTURER m = Converter.Manu(Converter.SplitManuCertype(signing.Origin)[0]);
            string fn = await ShowOpenFileDialogAsync();
            if (string.IsNullOrEmpty(fn))
            {
                //write error to status bar and return
                sbvm.Error("IMPORT PACKAGE", "Import operation aborted by user.");
                tbvm.IsOperationInProgress = false;
                return;
            }
            PackageInfo pi = new PackageInfo();
            ErrorMessage = string.Empty;
            bool r= await PackageExtraction(fn, signing, sbvm, pi);
            if (r == false)
            {
                signing.PI = new PackageInfoModel(pi);
                Log(LogData.OPERATION.IMPORT, LogData.RESULT.IMPORT_ERROR, signing);
                tbvm.IsOperationInProgress = false;
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
            tbvm.IsOperationInProgress = false;
        }
        private async Task<bool> PackageExtraction(string fn, SIXSoftwareSigningViewModel signing, SIXSoftwareSigningStatusBarViewModel sbvm,PackageInfo pi)
        {
            
            PackageProcessing pp = _container.Resolve<PackageProcessing>();
            //PackageInfo pi;
            SIGNER s = Converter.Signer(signing.Origin);
            MANUFACTURER m = Converter.Manu(Converter.SplitManuCertype(signing.Origin)[0]);
            STORETYPE st = Converter.ST(signing.StoreType);
            ENVIROMENT e = Converter.Env(signing.Enviroment);
            if (pp.ValidateFileName(fn, pi) == false)
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
            bool r = await pp.ExtractionAsync(s, st, e, pi, fn);
            if (r == false)
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
            //LogData log = _container.Resolve<LogData>();
            
            //log.Operation = op;
            //log.OperationResult = res;
            //log.e = Converter.Env(signing.Enviroment);
            //if(op!= LogData.OPERATION.MANU_CERT_SIGN)                
            //    log.Signer = Converter.Manu(signing.Signer);
            //log.SignerType = Converter.Signer(signing.SignerType);
            //log.StoreType = Converter.ST(signing.StoreType);
            //log.ErrorMessage = ErrorMessage;           
            //int index=log.Log();
            //if (signing.PI != null)
            //{
            //    TracePackageData pd = new TracePackageData();
            //    pd.Date = signing.PI.PackageDate;
            //    pd.PackageName = signing.PI.Name;
            //    pd.Vendor = signing.PI.Vendor;
            //    pd.Version = signing.PI.Version;
            //    log.LogPackage(pd, index);
            //}
            //ErrorMessage = string.Empty;
        }
    }
}
