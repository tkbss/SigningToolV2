using Infrastructure;
using NavigationModule.Models;
using Prism.Mvvm;
using SoftwareSigning.ViewModels;
using Unity;

namespace SoftwareSigning.Model
{
    public class PackageDropModel : BindableBase
    {
        IUnityContainer _container;
        public string ErrorMessage { get; set; }
        public Dictionary<string, PackageInfoModel> PackageModels { get; set; }
        

        public PackageDropModel(IUnityContainer container)
        {
            _container = container;
            
        }
        string _exportTest;
        public string ExportTest
        {
            get { return _exportTest; }
            set
            {
                SetProperty(ref _exportTest, value);
            }
        }
        string _exportProd;
        public string ExportProd
        {
            get { return _exportProd; }
            set
            {
                SetProperty(ref _exportProd, value);
            }
        }
        string _exportFileName;
        public string ExportFile
        {
            get { return _exportFileName; }
            set
            {
                SetProperty(ref _exportFileName, value);
            }
        }
        
        string _manu;
        public string Manufacturer
        {
            get { return _manu; }
            set
            {
                SetProperty(ref _manu, value);
            }
        }
        string _v;
        public string Version
        {
            get { return _v; }
            set
            {
                SetProperty(ref _v, value);
            }
        }
        PackageInfoModel? _mpm;
        public PackageInfoModel? DropedPackage
        {
            get { return _mpm; }
            set
            {
                SetProperty(ref _mpm, value);
            }
        }
        //PackageInfoModel? _loadedPackage;
        //public PackageInfoModel? LoadedPackage
        //{
        //    get { return _loadedPackage; }
        //    set
        //    {
        //        SetProperty(ref _loadedPackage, value);
        //    }
        //}
        //public void SetDropedPackage() 
        //{
        //    SIXSoftwareSigningViewModel signing = _container.Resolve<SIXSoftwareSigningViewModel>();
        //    string key = BuildDictionaryKey(signing.Signer, signing.Enviroment);
        //    signing.LoadPackageInfo();
        //    LoadedPackage = signing.PI; 

        //} 
        public void ExportFileName()
        {
            SIXSoftwareSigningViewModel signingview = _container.Resolve<SIXSoftwareSigningViewModel>();
            SoftwareSigningToolbarViewModel toolbar = _container.Resolve<SoftwareSigningToolbarViewModel>();
            PackageProcessing pp = _container.Resolve<PackageProcessing>();
            SIGNER s = Converter.Signer(signingview.SignerType);
            MANUFACTURER m = Converter.Manu(signingview.PackageProvider);
            ENVIROMENT e = Converter.Env(signingview.Enviroment);
            STORETYPE st = Converter.ST(signingview.StoreType);
            ExportFile=pp.GetPackageFileName(signingview.PackageProvider, s, st, e, signingview.PI.Version, signingview.PackageName) + ".zip";
        }
        public async Task PackageDrop(string fn)
        {
            
            SIXSoftwareSigningStatusBarViewModel sbvm = _container.Resolve<SIXSoftwareSigningStatusBarViewModel>();
            SIXSoftwareSigningViewModel signing = _container.Resolve<SIXSoftwareSigningViewModel>();
            SoftwareSigningToolbarViewModel tbvm = _container.Resolve<SoftwareSigningToolbarViewModel>();
            tbvm.IsOperationInProgress = true;            
            PackageManagementModel pm = _container.Resolve<PackageManagementModel>();
            PackageInfo pi = new PackageInfo();
            ErrorMessage = string.Empty;
            bool r = await PackageExtraction(fn, signing, sbvm, pi);
            if (r == false)
            {
                DropedPackage = new PackageInfoModel();
                DropedPackage.DropStatus="DROPED PACKAGE EXTRACTION ERROR";
                sbvm.Error("PACKAGE DROP", ErrorMessage);
                tbvm.IsOperationInProgress = false;
                return;
            }
            signing.SetPackageProviderTitle();
            signing.PI = new PackageInfoModel(pi);
            DropedPackage = signing.PI;
            DropedPackage.DropStatus = "DROPED PACKAGE EXTRACTED SUCCESSFULL";
            //signing.PI.Vendor = Converter.Vendor(signing.PI.Vendor);
            if (signing.LoadPackageInfo() == true)
            {
                ErrorMessage = string.Empty;
                SecurityProcessingModel sp = new SecurityProcessingModel();
                sp.SignatureVerification(_container);
                if (signing.PackageVerification == false)
                {
                    tbvm.IsOperationInProgress = false;
                    return;
                }
                sp.DetermineSigningStatus(_container);
            }
            else
            {
                ErrorMessage = "Error in parsing setup info";
                sbvm.Error("PACKAGE DROP", ErrorMessage);
                tbvm.IsOperationInProgress = false;
                return;

            }
            string key= BuildDictionaryKey(signing.Signer, signing.Enviroment);
            
            if (PackageModels == null)
            {
                PackageModels = new Dictionary<string, PackageInfoModel>();
            }
            if (PackageModels.ContainsKey(key))
            {
                PackageModels[key] = signing.PI;
            }
            else
            {
                PackageModels.Add(key, signing.PI);
            }
            tbvm.IsOperationInProgress = false;
            sbvm.Success("PACKAGE DROP", "Package imported successfully.");

        }
        private string BuildDictionaryKey(string signer, string env)
        { 
            if(Converter.Signer(signer)==SIGNER.QA)
            {
                return signer;
            }
            return signer+"-"+ env;
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
                pp.RemovePackage(m, s, st, e, pi.Version, pi.Name);
                return false;
            }
            if (pp.CheckSetupInfo(pi) == false)
            {
                ErrorMessage = pp.Error;
                sbvm.Error("IMPORT PACKAGE", pp.Error);
                pp.RemovePackage(m, s, st, e, pi.Version, pi.Name);
                return false;
            }
            signing.PackageProvider = Converter.Vendor(pi.Vendor);
            signing.SelectedVersion = pi.Version;
            signing.PackageName = pi.Name;
            return true;
        }
    }
}
