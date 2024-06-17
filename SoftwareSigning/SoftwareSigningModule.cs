using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prism.Modularity;
using Prism.Regions;
using SoftwareSigning.ViewModels;
using Unity;
using SoftwareSigning.Views;
using Prism.Ioc;

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

            containerRegistry.RegisterSingleton<SIXSoftwareSigningStatusBarView>();
            containerRegistry.RegisterSingleton<ManuSoftwareSigningView>();
            containerRegistry.RegisterSingleton<SIXSoftwareSigningView>();
            containerRegistry.RegisterSingleton<SoftwareSigningToolbarView>();
            containerRegistry.RegisterSingleton<ATMDeviceSoftwareSigningToolbarView>();

            

        }
             
    }
}
