using NavigationModule.ViewModels;
using NavigationModule.Views;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;
using Unity;

namespace NavigationModule
{
    public class Navigation : IModule
    {
        
        public void OnInitialized(IContainerProvider containerProvider)
        {
            var regionManager = containerProvider.Resolve<RegionManager>();
            regionManager.RegisterViewWithRegion("NavigationRegion", typeof(NavigationEmptyView));
            regionManager.RegisterViewWithRegion("ToolbarRegion", typeof(NavigationEmptyView));
            regionManager.RegisterViewWithRegion("StatusRegion", typeof(NavigationEmptyView));
            regionManager.RegisterViewWithRegion("NavigationRegion", typeof(NavigationSIXQAView));
            regionManager.RegisterViewWithRegion("NavigationRegion", typeof(NavigationMANUView));
            regionManager.RegisterViewWithRegion("NavigationRegion", typeof(NavigationSIXManagedView));
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterSingleton<NavigationViewModel>();
            

            containerRegistry.Register<NavigationSIXQAView>();
            containerRegistry.Register<NavigationEmptyView>();
            containerRegistry.Register<NavigationMANUView>();
            containerRegistry.Register<NavigationSIXManagedView>();
        }
        

    }
}
