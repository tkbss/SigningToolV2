using Infrastructure;
using NavigationModule.Models;
using Prism.Regions;
using SoftwareSigning.ViewModels;
using Unity;

namespace SoftwareSigning.Model
{
    public class NavigationModel
    {
        
        public NavigationModel(IUnityContainer container,IRegionManager manager)
        {
            _manager = manager;
            _container = container;
            ManufacturerPackageManagement=_container.Resolve<PackageManagementModel>();
        }
        IRegionManager _manager;
        IUnityContainer _container;
        NavigationParameters parameters;
        //string StorePassword;
        Uri navigationURI;       
        public MANUFACTURER Manu { get; set; }
        public PackageManagementModel ManufacturerPackageManagement
        {
            get; set;
        }
        public void SetATMEnviroment(int SIATM)
        {
           
            Manu = MANUFACTURER.SIX;
            
            SIXSoftwareSigningViewModel _signing = _container.Resolve<SIXSoftwareSigningViewModel>();
            //StorePasswordSafe PwdManagement = _container.Resolve<StorePasswordSafe>();
            parameters = new NavigationParameters();
            parameters.Add("MANU", "SIX-ATM");
            parameters.Add("VERSION",_signing.SelectedVersion);

            navigationURI = new Uri("NavigationSIXQAView", UriKind.Relative);
            _signing.Signer = "SIX";
            _signing.Origin = "SIX-ATM";
            _signing.SignerType = "ATM";
            
            switch (SIATM)
            {
                case 0:
                    navigationURI = new Uri("NavigationSIXQAView", UriKind.Relative);
                    _signing.StoreType = "UNMANAGED";
                    _signing.Enviroment = "TEST";
                    parameters.Add("ENV", "TEST");
                    
                    //StorePassword = PwdManagement.DefaultStorePwd;
                    break;
                case 1:
                    navigationURI = new Uri("NavigationATMView", UriKind.Relative);
                    _signing.StoreType = "KMS";
                    _signing.Enviroment = "TEST";
                    parameters.Add("ENV", "TEST");
                   
                    //StorePassword = PwdManagement.GetSIXPassword(STORETYPE.KMS, ENVIROMENT.TEST, CERTTYPE.ATM);
                    break;
                case 2:
                    navigationURI = new Uri("NavigationATMView", UriKind.Relative);
                    _signing.StoreType = "KMS";
                    _signing.Enviroment = "PROD";
                    parameters.Add("ENV", "PROD");
                    
                    //StorePassword = PwdManagement.GetSIXPassword(STORETYPE.KMS, ENVIROMENT.PROD, CERTTYPE.ATM);
                    break;
            }
            

        }
        public void Navigate(int SIATM)
        {
            SetATMEnviroment(SIATM);
            SIXSoftwareSigningViewModel _signing = _container.Resolve<SIXSoftwareSigningViewModel>();
            LoadAllVersions(_signing.Signer, _signing.StoreType, _signing.Enviroment);
            parameters=MakeParameters(_signing.SelectedVersion, _signing.PackageProvider, _signing.Enviroment, _signing.PackageName);
            _manager.RequestNavigate("NavigationRegion", navigationURI, parameters);
            _manager.RequestNavigate("MainRegion", new Uri("SIXSoftwareSigningView", UriKind.Relative), parameters);            
            _manager.RequestNavigate("ToolbarRegion", new Uri("SoftwareSigningToolbarView", UriKind.Relative));

        }
        private NavigationParameters MakeParameters(string v, string m, string e, string pn)
        {
            NavigationParameters parameters = new NavigationParameters();
            parameters.Add("VERSION", v);
            parameters.Add("PACKAGE_PROVIDER", m);
            parameters.Add("ENV", e);
            parameters.Add("PACKAGE_NAME", pn);
            return parameters;
        }
        public void LoadAllVersions(string signer, string store_type, string env)
        {
            STORETYPE st = Converter.ST(store_type);
            ENVIROMENT e = Converter.Env(env);
            ManufacturerPackageManagement.LoadAllPackages(signer, st, e);
            

        }
    }

}
