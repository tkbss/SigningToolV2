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
            regionManager.RegisterViewWithRegion("MainRegion", typeof(LoginView));
            regionManager.RegisterViewWithRegion("ToolbarRegion", typeof(LoginToolbarView));
            regionManager.RegisterViewWithRegion("StatusRegion", typeof(LoginStatusBarView));
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterSingleton<LoginViewModel>();
            containerRegistry.RegisterSingleton<LoginStatusBarViewModel>();


            containerRegistry.RegisterSingleton<LoginView>();
            containerRegistry.Register<LoginToolbarView>();
            containerRegistry.Register<LoginStatusBarView>();
            containerRegistry.Register<NavigationSIXManagedView>();
        }
    }
}
