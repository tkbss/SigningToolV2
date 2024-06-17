using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;
using TracingModule.View;
using TracingModule.ViewModel;
using Unity;

namespace TracingModule
{
    public class TracingModule : IModule
    {
        IRegionManager _manager;
        IUnityContainer _container;
        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterSingleton<TraceDataViewModel>();
            containerRegistry.RegisterSingleton<TraceDataToolbarViewModel>();
            containerRegistry.RegisterSingleton<LogData>();

            containerRegistry.Register<TraceDataView>();
            containerRegistry.Register<TraceDataToolbarView>();
            
        }

        public void OnInitialized(IContainerProvider containerProvider)
        {
            var regionManager = containerProvider.Resolve<RegionManager>();
            regionManager.RegisterViewWithRegion("MainRegion", typeof(TraceDataView));
            regionManager.RegisterViewWithRegion("ToolbarRegion", typeof(TraceDataToolbarView));
        }
        

        
    }
}
