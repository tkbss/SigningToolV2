using NavigationModule.Views;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;
using SigningTool.ViewModels;
using SigningTool.Views;
using Unity;

namespace SigningTool
{
    public class StartupModule : IModule
    {
        

        public void OnInitialized(IContainerProvider containerProvider)
        {
            var regionManager = containerProvider.Resolve<RegionManager>();
            regionManager.RegisterViewWithRegion("StatusRegion", typeof(LoginStatusBarView));
            regionManager.RegisterViewWithRegion("MainRegion", typeof(LoginView));
            regionManager.RegisterViewWithRegion("ToolbarRegion", typeof(LoginToolbarView));
            
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterSingleton<LoginStatusBarViewModel>();
            containerRegistry.RegisterSingleton<LoginStatusBarView>();
            containerRegistry.RegisterSingleton<LoginViewModel>();
            


            containerRegistry.RegisterSingleton<LoginView>();
            containerRegistry.Register<LoginToolbarView>();            
            containerRegistry.Register<NavigationSIXManagedView>();
        }
    }
}
