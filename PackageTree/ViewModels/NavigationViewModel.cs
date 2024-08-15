using Infrastructure;
using NavigationModule.Models;
using PackageTree;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
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
        
        public ICommand KeyGenerationCommand { get; private set; }
        public ICommand ImportPackageCommand { get; private set; }
        
        

        public ICommand HSMStatusCommand{get;set;}

        PackageManagementModel mpm;
        public PackageManagementModel ManufacturerPackageManagement
        {
            get { return mpm; }
            set 
            {                 
                SetProperty(ref mpm, value);
            }
        }

        

        
        string Manufacturer { get; set; }
        
        
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
            ManufacturerPackageManagement = new PackageManagementModel(_container, OnManuSoftwareSigning, manufacturers);
            
            KeyGenerationCommand = new DelegateCommand(this.OnKeyGeneration);
            ImportPackageCommand = new DelegateCommand(this.OnImportPackage);
            
            HSMStatusCommand = new DelegateCommand(this.OnHSMStatus);
            
           
        }
        
        
        

        private void OnManuSoftwareSigning(PackageModel pm)
        {
            NavigationParameters parameters = MakeParameters(pm.Version, pm.Manu, enviroment, pm.PackageName);
            
            SIGNER s = Converter.Signer(signer);
            if(s==SIGNER.ATM_DEVICE)
                _manager.RequestNavigate("ToolbarRegion", NavigationURI.signingToolbarATMDeviceViewUri);
            else
                _manager.RequestNavigate("ToolbarRegion", NavigationURI.signingToolbarViewUri);

            if (s==SIGNER.MANU)
                _manager.RequestNavigate("MainRegion", NavigationURI.ManuSoftwareSigningViewUri, parameters);
            else
                _manager.RequestNavigate("MainRegion", NavigationURI.SIXsigningViewUri, parameters);

        }
        
        private void OnImportPackage()
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
            vm.OnImportPackage();         
        }
        public void OnHSMStatus()
        {
            
            NavigationParameters parameters = MakeParameters(string.Empty, string.Empty, enviroment, string.Empty);
            parameters.Add("MANAGED", "KMS");
            parameters.Add("MANU", signer);
            _manager.RequestNavigate("MainRegion", new Uri("HSMStatusView", UriKind.Relative), parameters);
            if (signer == "SIX-QA")
                _manager.RequestNavigate("ToolbarRegion", NavigationURI.KMSQAKeyGenerationToolbarViewUri);
            if (signer == "SIX-ATM")
                _manager.RequestNavigate("ToolbarRegion", NavigationURI.KMSATMKeyGenerationToolbarViewUri);
        }
        public void OnKeyGeneration()
        {
            
            NavigationParameters parameters = MakeParameters(string.Empty, string.Empty, enviroment, string.Empty);
            parameters.Add("MANAGED", "UNMANAGED");
            parameters.Add("MANU", signer);
            if (signer=="SIX-QA")
                _manager.RequestNavigate("ToolbarRegion", NavigationURI.QAkeyGenerationToolbvarViewUri);
            else if(signer=="SIX-ATM")
                _manager.RequestNavigate("ToolbarRegion", NavigationURI.ATMkeyGenerationToolbvarViewUri);
            else
                _manager.RequestNavigate("ToolbarRegion", NavigationURI.MANUKeyGenerationToolbarViewUri);

            _manager.RequestNavigate("MainRegion", NavigationURI.keyGenerationViewUri, parameters);
            _manager.RequestNavigate("StatusRegion", NavigationURI.SIXSoftwareSigningStatusBarView);
            
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
            SIXSoftwareSigningViewModel vm = _container.Resolve<SIXSoftwareSigningViewModel>();
            vm.LoadAllVersions = LoadAllVersions;
            vm.SelectVersion = SelectVersion;            
            enviroment = vm.Enviroment;
            signer = vm.Origin;            
            string[] manu_ct = Converter.SplitManuCertype(vm.Origin);
            Manufacturer = manu_ct[0];
            ManuPackages = Manufacturer + " PACKAGES";
            LoadAllVersions(signer,vm.StoreType,enviroment);
            LoadOtherRegions(enviroment, signer);
        }
        
        private void LoadOtherRegions(string env,string signer)
        {

            SIGNER s = Converter.Signer(signer);
            _manager.RequestNavigate("StatusRegion", NavigationURI.SIXSoftwareSigningStatusBarView);
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
                NavigationParameters parameters = MakeParameters(string.Empty, string.Empty, enviroment, string.Empty);
                _manager.RequestNavigate("MainRegion", NavigationURI.SIXsigningViewUri, parameters);             
                
            }

            
        }
        private NavigationParameters MakeParameters(string v,string m,string e,string pn)
        {
            NavigationParameters parameters = new NavigationParameters();
            parameters.Add("VERSION", v);
            parameters.Add("PACKAGE_PROVIDER", m);
            parameters.Add("ENV", e);
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
