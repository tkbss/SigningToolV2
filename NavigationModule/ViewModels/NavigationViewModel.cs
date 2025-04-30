using Infrastructure;
using NavigationModule.Models;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using SigningKeyManagment.ViewModels;
using SoftwareSigning.Model;
using SoftwareSigning.ViewModels;
using System.Windows;
using System.Windows.Input;
using Unity;

namespace NavigationModule.ViewModels
{
    public class NavigationViewModel : BindableBase, INavigationAware
    {
        string[] manufacturers = Converter.ManufacturesString;
        
        IUnityContainer _container;
        IRegionManager _manager;
        string signer;
        string enviroment;
        bool skipNavigate= false;
        
        public ICommand KeyGenerationCommand { get; private set; }
        public ICommand ImportPackageCommand { get; private set; }
        public ICommand ChangeSessionCommand { get; private set; }
        public ICommand QANavigationCommand { get; private set; }
        public ICommand NavigationATMTestCommand { get; private set; }
        public ICommand NavigationATMPRODCommand { get; private set; }
        public ICommand ShowTraceCommand { get; private set; }       

        public ICommand HSMStatusCommand{get;set;}
        public ICommand TreeNavigationCommand { get; set; }
        public PackageModel SelectedItem { get; set; }
        PackageManagementModel mpm;
        public PackageManagementModel ManufacturerPackageManagement
        {
            get { return mpm; }
            set { SetProperty(ref mpm, value); }
        }
        KeyGenerationViewModel _keyGenerationViewModel;
        public KeyGenerationViewModel KeyGenerationViewModel 
        { 
            get { return _keyGenerationViewModel; } 
            set { SetProperty(ref _keyGenerationViewModel, value); } 
        }   


        string Manufacturer { get; set; }
        
        PackageDropModel _pd;
        public PackageDropModel PackageDrop
        {
            get { return _pd; }
            set
            {
                SetProperty(ref _pd, value);
            }
        }
        string manu_packages;
        public string ManuPackages
        {
            get { return manu_packages; }
            set
            {
                SetProperty(ref manu_packages, value);
            }
        }
        Visibility kg_vis;
        public Visibility KGVisibility
        {
            get
            {
                return kg_vis;
            }
            set
            {
                SetProperty(ref kg_vis, value);
            }
        }

        public NavigationViewModel(IUnityContainer container, IRegionManager manager)
        {
            _manager = manager;
            _container = container;
            mpm = _container.Resolve<PackageManagementModel>();
            mpm.ManuSoftwareSigning = OnManuSoftwareSigning;
            mpm.SetManufacturers(manufacturers);
            KeyGenerationViewModel = _container.Resolve<KeyGenerationViewModel>();


            KeyGenerationCommand = new DelegateCommand(this.OnKeyGeneration);
            ImportPackageCommand = new DelegateCommand(this.OnImportPackage);
            ChangeSessionCommand = new DelegateCommand(this.OnChangeSession);
            HSMStatusCommand = new DelegateCommand(this.OnHSMStatus);
            QANavigationCommand = new DelegateCommand(OnQANavigation);
            NavigationATMTestCommand = new DelegateCommand(OnATMTestNavigation);
            NavigationATMPRODCommand = new DelegateCommand(OnATMProdNavigation);
            TreeNavigationCommand = new DelegateCommand(OnPackageTree);



        }
        private void OnATMTestNavigation() 
        {
            SIXSoftwareSigningViewModel signing = _container.Resolve<SIXSoftwareSigningViewModel>();
            signing.Origin = "SIX-ATM";
            //signing.StoreType = "KMS";
            signing.Enviroment = "TEST";
            signing.Signer = "SIX";
            signing.SignerType = "ATM";
            var packagedrop = _container.Resolve<PackageDropModel>();
            ENVIROMENT e = ENVIROMENT.TEST;
            packagedrop.LoadedPackage=packagedrop.DropedPackage;
            if (packagedrop.LoadedPackage != null)
            {
                NavigationParameters parameters = MakeParameters(packagedrop.LoadedPackage.Version, Converter.Vendor(packagedrop.LoadedPackage.Vendor), Converter.Env(e), packagedrop.LoadedPackage.Name);
                _manager.RequestNavigate("NavigationRegion", new Uri("NavigationATMView", UriKind.Relative), parameters);
            }
            else
            {
                NavigationParameters parameters = MakeParameters(string.Empty, string.Empty, Converter.Env(e), string.Empty);
                _manager.RequestNavigate("NavigationRegion", new Uri("NavigationATMView", UriKind.Relative), parameters);
            }

        }
        private void OnATMProdNavigation()
        {
            SIXSoftwareSigningViewModel signing = _container.Resolve<SIXSoftwareSigningViewModel>();
            signing.Origin = "SIX-ATM";
            //signing.StoreType = "KMS";
            signing.Enviroment = "PROD";
            signing.Signer = "SIX";
            signing.SignerType = "ATM";
            var packagedrop = _container.Resolve<PackageDropModel>();
            ENVIROMENT e = ENVIROMENT.PROD;
            packagedrop.LoadedPackage = packagedrop.DropedPackage;
            if (packagedrop.LoadedPackage != null)
            {
                NavigationParameters parameters = MakeParameters(packagedrop.LoadedPackage.Version, Converter.Vendor(packagedrop.LoadedPackage.Vendor), Converter.Env(e), packagedrop.LoadedPackage.Name);
                _manager.RequestNavigate("NavigationRegion", new Uri("NavigationATMView", UriKind.Relative), parameters);
            }
            else
            {
                NavigationParameters parameters = MakeParameters(string.Empty, string.Empty, Converter.Env(e), string.Empty);
                _manager.RequestNavigate("NavigationRegion", new Uri("NavigationATMView", UriKind.Relative), parameters);
            }

        }
        private void OnQANavigation()
        {
            SIXSoftwareSigningViewModel signing = _container.Resolve<SIXSoftwareSigningViewModel>();
            signing.Origin = "SIX-QA";
            //signing.StoreType = "KMS";
            signing.Enviroment = "TEST";
            signing.Signer = "SIX";
            signing.SignerType = "QA";
            var packagedrop = _container.Resolve<PackageDropModel>();
            ENVIROMENT e = ENVIROMENT.TEST;
            NavigationParameters parameters = null;
            if (packagedrop.DropedPackage == null)
            {
                if (SelectedItem == null)
                    parameters = MakeParameters(string.Empty, string.Empty, Converter.Env(e), string.Empty);
                else
                {
                    parameters = MakeParameters(SelectedItem.Version, SelectedItem.Manu, enviroment, SelectedItem.PackageName);
                    PackageDrop.ExportFile = SelectedItem.PackageName + ".zip";
                }
                _manager.RequestNavigate("NavigationRegion", new Uri("NavigationQAView", UriKind.Relative), parameters);
            }
            else 
            { 
                parameters = MakeParameters(packagedrop.DropedPackage.Version, Converter.Vendor(packagedrop.DropedPackage.Vendor), Converter.Env(e), packagedrop.DropedPackage.Name);
                _manager.RequestNavigate("NavigationRegion", new Uri("NavigationQAView", UriKind.Relative), parameters);
            }
        }
        private void Navigate(string v,string manu)
        {
            NavigationParameters parameters = MakeParameters(v, manu, enviroment, string.Empty);
           
            _manager.RequestNavigate("StatusRegion", NavigationURI.SIXSoftwareSigningStatusBarView);
            _manager.RequestNavigate("MainRegion", NavigationURI.SIXsigningViewUri, parameters);
            _manager.RequestNavigate("ToolbarRegion", NavigationURI.signingToolbarViewUri);
        }
        

        private void OnManuSoftwareSigning(PackageModel pm)
        {
            NavigationParameters parameters = MakeParameters(pm.Version, pm.Manu, enviroment, pm.PackageName);
            PackageDrop.ExportFile=pm.PackageName + ".zip";
            SIGNER s = Converter.Signer(signer);
            if(s==SIGNER.ATM_DEVICE)
                _manager.RequestNavigate("ToolbarRegion", NavigationURI.signingToolbarATMDeviceViewUri);
            else
                _manager.RequestNavigate("ToolbarRegion", NavigationURI.signingToolbarViewUri);

            if (s == SIGNER.MANU)
                _manager.RequestNavigate("MainRegion", NavigationURI.ManuSoftwareSigningViewUri, parameters);
            else
            {
                SIXSoftwareSigningViewModel signing = _container.Resolve<SIXSoftwareSigningViewModel>();
                SoftwareSigningToolbarViewModel toolbar = _container.Resolve<SoftwareSigningToolbarViewModel>();
                toolbar.IsOperationInProgress = true;
                signing.SelectedVersion = pm.Version;
                signing.PackageProvider = pm.Manu;
                signing.Enviroment = enviroment;
                signing.PackageName = pm.PackageName;
                signing.LoadPackageInfo();
                toolbar.IsOperationInProgress = false;
                //_manager.RequestNavigate("MainRegion", NavigationURI.SIXsigningViewUri, parameters);
            }

        }
        private void OnChangeSession()
        {
            
            _manager.RequestNavigate("MainRegion", NavigationURI.LoginViewUri);
            _manager.RequestNavigate("NavigationRegion", NavigationURI.NavigationEmptyViewUri);
            _manager.RequestNavigate("ToolbarRegion", NavigationURI.LoginToolbarViewUri);
            _manager.RequestNavigate("StatusRegion", NavigationURI.LoginStatusBarViewUri);
        }
        private async void OnImportPackage()
        {
            
            SoftwareSigningToolbarViewModel vm= _container.Resolve<SoftwareSigningToolbarViewModel>();
            NavigationParameters parameters = MakeParameters(string.Empty, Manufacturer, enviroment, string.Empty);
            
            SIGNER s = Converter.Signer(signer);
            if(s==SIGNER.MANU)
                _manager.RequestNavigate("MainRegion", NavigationURI.ManuSoftwareSigningViewUri, parameters);
            else
                _manager.RequestNavigate("MainRegion", NavigationURI.SIXsigningViewUri, parameters);
            _manager.RequestNavigate("ToolbarRegion", NavigationURI.signingToolbarViewUri);
            _manager.RequestNavigate("StatusRegion", NavigationURI.SIXSoftwareSigningStatusBarView);
            await vm.OnImportPackage();         
        }
        public void OnPackageTree() 
        {
            NavigationParameters parameters = MakeParameters(string.Empty, string.Empty, enviroment, string.Empty);
            parameters.Add("MANAGED", "KMS");
            parameters.Add("MANU", signer);
            _manager.RequestNavigate("NavigationRegion", new Uri("NavigationSIXManagedView", UriKind.Relative), parameters);
            _manager.RequestNavigate("MainRegion", NavigationURI.SIXsigningViewUri, parameters);
        }
        public void OnHSMStatus()
        {
            
            NavigationParameters parameters = MakeParameters(string.Empty, string.Empty, enviroment, string.Empty);
            parameters.Add("MANAGED", "KMS");
            parameters.Add("MANU", signer);          
            _manager.RequestNavigate("NavigationRegion", new Uri("NavigationHSMTreeView", UriKind.Relative), parameters);            
            if (signer == "SIX-QA") {
                _manager.RequestNavigate("MainRegion", new Uri("KeyGenerationView", UriKind.Relative), parameters);
                _manager.RequestNavigate("ToolbarRegion", NavigationURI.KMSQAKeyGenerationToolbarViewUri);
            }
            if (signer == "SIX-ATM")
            {
                _manager.RequestNavigate("MainRegion", new Uri("HSMStatusView", UriKind.Relative), parameters);
                _manager.RequestNavigate("ToolbarRegion", NavigationURI.KMSATMKeyGenerationToolbarViewUri);
            }
        }
        public void OnKeyGeneration()
        {
            skipNavigate = true;
            SIXSoftwareSigningViewModel vm = _container.Resolve<SIXSoftwareSigningViewModel>();
            NavigationParameters parameters = MakeParameters(string.Empty, string.Empty, enviroment, string.Empty);
            if (signer == Converter.Signer(SIGNER.MANU))
                parameters.Add("MANAGED", "UNMANAGED");
            else
                parameters.Add("MANAGED", vm.StoreType);
            parameters.Add("MANU", signer);
            if (signer == "SIX-QA")
            {
                if (vm.StoreType == Converter.ST(STORETYPE.KMS))
                    _manager.RequestNavigate("ToolbarRegion", NavigationURI.KMSQAKeyGenerationToolbarViewUri);
                else
                    _manager.RequestNavigate("ToolbarRegion", NavigationURI.QAkeyGenerationToolbvarViewUri);
                _manager.RequestNavigate("NavigationRegion", new Uri("NavigationHSMKMView", UriKind.Relative));
                _manager.RequestNavigate("MainRegion", NavigationURI.keyGenerationViewUri, parameters);
            }
            else if (signer == "SIX-ATM")
            {
                if (vm.StoreType == Converter.ST(STORETYPE.KMS))
                    _manager.RequestNavigate("ToolbarRegion", NavigationURI.KMSATMKeyGenerationToolbarViewUri);
                else
                    _manager.RequestNavigate("ToolbarRegion", NavigationURI.ATMkeyGenerationToolbvarViewUri);
                _manager.RequestNavigate("NavigationRegion", new Uri("NavigationHSMKMView", UriKind.Relative));
                _manager.RequestNavigate("MainRegion", NavigationURI.HSMStatusViewUri, parameters);
            }
            else
            {
                _manager.RequestNavigate("ToolbarRegion", NavigationURI.MANUKeyGenerationToolbarViewUri);
                _manager.RequestNavigate("MainRegion", NavigationURI.keyGenerationViewUri, parameters);
            }            
            _manager.RequestNavigate("StatusRegion", NavigationURI.SIXSoftwareSigningStatusBarView);
            skipNavigate = false;
        }
        
        public void LoadAllVersions(string signer,string store_type,string env)
        {
            STORETYPE st = Converter.ST(store_type);
            ENVIROMENT e = Converter.Env(env);
            ManufacturerPackageManagement.LoadAllPackages(signer,st,e);
            ManufacturerPackageManagement.SetManuPackages(Converter.Manu(Manufacturer));            

        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            if (skipNavigate==true)
            {
                skipNavigate = false;
                return;
            }
            SIXSoftwareSigningViewModel vm = _container.Resolve<SIXSoftwareSigningViewModel>();
            PackageDrop = _container.Resolve<PackageDropModel>();
            PackageDrop.DropedPackage = null;
            vm.LoadAllVersions = LoadAllVersions;
            vm.SelectVersion = SelectVersion;            
            enviroment = vm.Enviroment;
            signer = vm.Origin;            
            string[] manu_ct = Converter.SplitManuCertype(vm.Origin);
            Manufacturer = manu_ct[0];
            ManuPackages = Manufacturer + " PACKAGES";
            ManufacturerPackageManagement = _container.Resolve<PackageManagementModel>();
            SelectedItem = ManufacturerPackageManagement.SeletectedItem;
            LoadAllVersions(signer,vm.StoreType,enviroment);
            LoadOtherRegions(enviroment, signer,  navigationContext);
        }
        
        private void LoadOtherRegions(string env,string signer, NavigationContext navigationContext)
        {

            SIGNER s = Converter.Signer(signer);
            _manager.RequestNavigate("StatusRegion", NavigationURI.SIXSoftwareSigningStatusBarView);
            //_manager.RequestNavigate("TreeRegion", NavigationURI.PacakageTreeViewUri);
            if (s == SIGNER.ATM_DEVICE)
            {
                _manager.RequestNavigate("ToolbarRegion", NavigationURI.signingToolbarATMDeviceViewUri);
                KGVisibility = Visibility.Hidden;
            }
            else
            {
                _manager.RequestNavigate("ToolbarRegion", NavigationURI.signingToolbarViewUri);
                KGVisibility = Visibility.Visible;
            }

            if (s == SIGNER.MANU)
            {
                NavigationParameters parameters = MakeParameters(string.Empty, Manufacturer, enviroment, string.Empty);
                _manager.RequestNavigate("MainRegion", NavigationURI.ManuSoftwareSigningViewUri, parameters);
            }
            else
            {
                NavigationParameters parameters = null;
                if (SelectedItem != null)
                    parameters = MakeParameters(SelectedItem.Version, SelectedItem.Manu, enviroment, SelectedItem.PackageName);
                else
                    parameters = navigationContext.Parameters;
                _manager.RequestNavigate("MainRegion", NavigationURI.SIXsigningViewUri, parameters);             
                
            }

            
        }
        private NavigationParameters MakeParameters(string v,string m,string e,string pn)
        {
            NavigationParameters parameters = new NavigationParameters();
            if (string.IsNullOrEmpty(v))
                v = "";
            parameters.Add("VERSION", v);
            if (string.IsNullOrEmpty(m))
                m = "";
            parameters.Add("PACKAGE_PROVIDER", m);
            if (string.IsNullOrEmpty(e))
                e = "";
            parameters.Add("ENV", e);
            if (string.IsNullOrEmpty(pn))
                pn = "";
            parameters.Add("PACKAGE_NAME", pn);
            return parameters;
        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
            
        }
        public void SelectVersion(string PackageProvider, string v, string signer,string packageName)
        {
            //go through every entry set IsSelcted property to false except for the entry uniquely defined by the parameters
            foreach(var mp in ManufacturerPackageManagement.ManuPackages)
            {
                foreach(var vl in mp.VersionList)
                {
                    foreach(var pnl in vl.PackageNameList)
                    {
                        if (pnl.Manu == PackageProvider && pnl.Version == v && pnl.PackageName == packageName)
                            pnl.IsSelected = true;
                        else
                            pnl.IsSelected = false;
                    }
                }
            }
        }
        
    }
}
