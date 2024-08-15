using NavigationModule.ViewModels;
using NavigationModule.Views;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;
using Unity;

namespace PackageTree
{
    public class PackageTreeModule : IModule
    {
        
        public void OnInitialized(IContainerProvider containerProvider)
        {
            var regionManager = containerProvider.Resolve<RegionManager>();
            regionManager.RegisterViewWithRegion("TreeRegion", typeof(PackageTreeView));
            
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterSingleton<NavigationViewModel>();         

            containerRegistry.Register<PackageTreeView>();
            
        }
        

    }
}
