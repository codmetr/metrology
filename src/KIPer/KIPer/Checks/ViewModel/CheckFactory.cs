using ArchiveData.DTO;
using CheckFrame.ViewModel.Checks.Channels;
using KipTM.Checks;
using KipTM.Checks.ViewModel;
using KipTM.EventAggregator;

namespace KipTM.ViewModel.Checks
{
    public class CheckFactory : ICheckFactory
    {
        private CheckPool _checkPool;
        private CheckConfig _checkConfig;
        private TestResult _result;
        private SelectChannelViewModel _chTargetDev;
        private SelectChannelViewModel _chEthalonDev;
        private IEventAggregator _eventAggregator;


        public CheckFactory(CheckPool checkPool, CheckConfig checkConfig, TestResult result, SelectChannelViewModel chTargetDev, SelectChannelViewModel chEthalonDev, IEventAggregator eventAggregator)
        {
            _checkPool = checkPool;
            _checkConfig = checkConfig;
            _result = result;
            _chTargetDev = chTargetDev;
            _chEthalonDev = chEthalonDev;
            _eventAggregator = eventAggregator;
        }

        /// <summary>
        /// Фабрика модели представления методики
        /// </summary>
        /// <returns></returns>
        public IMethodViewModel GetViewModelFor()
        {
            IMethodViewModel result = null;
            var targetType = _checkConfig.SelectedMethod.GetType();
            var factory = _checkPool.GetFactory(targetType);

            if (factory != null)
            {
                result = factory.GetViewModel(_checkConfig.SelectedMethod, _checkConfig.Data, _checkConfig.CustomSettings, _result,
                    _chTargetDev.SelectedChannel, _chEthalonDev.SelectedChannel);
                if (result!=null)
                    result.SetAggregator(_eventAggregator);
            }

            return result;
        }
    }
}
