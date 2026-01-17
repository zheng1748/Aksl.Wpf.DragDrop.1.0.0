using Prism.Ioc;
using Prism.Modularity;
using Prism.Mvvm;

using Unity;

using Aksl.Modules.Miscs.OrgChart.ViewModels;
using Aksl.Modules.Miscs.OrgChart.Views;

namespace Aksl.Modules.Miscs
{
    public class MiscsModule : IModule
    {
        #region Members
        private readonly IUnityContainer _container;
        #endregion

        #region Constructors
        public MiscsModule(IUnityContainer container)
        {
            this._container = container;
        }
        #endregion

        #region IModule 成员
        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<OrgChartView>();

        }

        public void OnInitialized(IContainerProvider containerProvider)
        {
            ViewModelLocationProvider.Register(typeof(OrgChartView).ToString(),
                                               () => this._container.Resolve<OrgChartViewModel>());
        }
        #endregion
    }
}
