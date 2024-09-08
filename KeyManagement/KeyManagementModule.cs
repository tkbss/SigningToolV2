using Unity;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;
using SigningKeyManagment.ViewModels;
using SigningKeyManagment.Views;
using SoftwareSigning.Views;
using SoftwareSigning;
using System.Windows;



namespace SigningKeyManagment
{
    public class KeyManagementModule : IModule
    {
        
        public void OnInitialized(IContainerProvider containerProvider)
        {
            var regionManager = containerProvider.Resolve<RegionManager>();            
            regionManager.RegisterViewWithRegion("MainRegion", typeof(HSMStatusView));           
            regionManager.RegisterViewWithRegion("MainRegion", typeof(KeyGenerationView));
            regionManager.RegisterViewWithRegion("ToolbarRegion", typeof(ATMKeyGenerationToolbarView));
            regionManager.RegisterViewWithRegion("ToolbarRegion", typeof(QAKeyGenerationToolbarView));
            regionManager.RegisterViewWithRegion("ToolbarRegion", typeof(MANUKeyGenerationToolbarView));
            regionManager.RegisterViewWithRegion("ToolbarRegion", typeof(KMSQAKeyGenerationToolbarView));
            regionManager.RegisterViewWithRegion("ToolbarRegion", typeof(KMSATMKeyGenerationToolbarView));
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterSingleton<KeyGenerationViewModel>();
            containerRegistry.RegisterSingleton<KeyGenerationToolbarViewModel>();
            containerRegistry.RegisterSingleton<HSMStatusViewModel>();
            containerRegistry.RegisterSingleton<KMSKeyGenerationToolbarViewModel>();

            containerRegistry.Register<KeyGenerationView>();
            containerRegistry.Register<ATMKeyGenerationToolbarView>();
            containerRegistry.Register<QAKeyGenerationToolbarView>();
            containerRegistry.Register<MANUKeyGenerationToolbarView>();
            containerRegistry.Register<KMSQAKeyGenerationToolbarView>();
            containerRegistry.Register<KMSATMKeyGenerationToolbarView>();
            containerRegistry.Register<HSMStatusView>();
        }   

        
    }
}
