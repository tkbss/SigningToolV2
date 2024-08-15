using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;
using SoftwareSigning.Model;
using SoftwareSigning.ViewModels;
using SoftwareSigning.Views;
using Unity;

namespace SoftwareSigning
{
    public class SoftwareSigningModule : IModule
    {
        
        public void OnInitialized(IContainerProvider containerProvider)
        {
            var regionManager = containerProvider.Resolve<RegionManager>();
            regionManager.RegisterViewWithRegion("StatusRegion", typeof(SIXSoftwareSigningStatusBarView));
            regionManager.RegisterViewWithRegion("MainRegion", typeof(SIXSoftwareSigningView));
            regionManager.RegisterViewWithRegion("MainRegion", typeof(ManuSoftwareSigningView));            
            regionManager.RegisterViewWithRegion("ToolbarRegion", typeof(SoftwareSigningToolbarView));
            regionManager.RegisterViewWithRegion("ToolbarRegion", typeof(ATMDeviceSoftwareSigningToolbarView));
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterSingleton<SIXSoftwareSigningViewModel>();
            containerRegistry.RegisterSingleton<SoftwareSigningToolbarViewModel>();
            containerRegistry.RegisterSingleton<SIXSoftwareSigningStatusBarViewModel>();
            containerRegistry.RegisterSingleton<PackageDropModel>();

            containerRegistry.Register<SIXSoftwareSigningStatusBarView>();
            containerRegistry.Register<ManuSoftwareSigningView>();
            containerRegistry.Register<SIXSoftwareSigningView>();
            containerRegistry.Register<SoftwareSigningToolbarView>();
            containerRegistry.Register<ATMDeviceSoftwareSigningToolbarView>();
            



        }
             
    }
}
