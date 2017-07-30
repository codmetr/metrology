using ArchiveData.DTO;
using CheckFrame.ViewModel.Checks.Channels;
using KipTM.EventAggregator;
using KipTM.ViewModel.Checks;

namespace KipTM.Checks.ViewModel
{
    /// <summary>
    /// Фабрика визуальной модели для проверки
    /// </summary>
    public class CheckFactory : ICheckFactory
    {
        private readonly CheckPool _checkPool;
        private readonly CheckConfig _checkConfig;
        private readonly TestResult _result;
        private readonly IEventAggregator _eventAggregator;

        /// <summary>
        /// Фабрика визуальной модели для проверки
        /// </summary>
        /// <param name="checkPool"></param>
        /// <param name="checkConfig"></param>
        /// <param name="result"></param>
        /// <param name="eventAggregator"></param>
        public CheckFactory(CheckPool checkPool, CheckConfig checkConfig, TestResult result, IEventAggregator eventAggregator)
        {
            _checkPool = checkPool;
            _checkConfig = checkConfig;
            _result = result;
            _eventAggregator = eventAggregator;
        }

        /// <summary>
        /// Фабрика модели представления методики
        /// </summary>
        /// <returns></returns>
        public IMethodViewModel GetViewModelFor()
        {
            IMethodViewModel check = null;
            var targetType = _checkConfig.SelectedMethod.GetType();
            var factory = _checkPool.GetFactory(targetType);

            if (factory != null)
            {
                check = factory.GetViewModel(_checkConfig.SelectedMethod, _checkConfig.Data, _checkConfig.CustomSettings, _result,
                    _checkConfig.TargetTransportChannel, _checkConfig.EthalonTransportChannel);
                if (check!=null)
                    check.SetAggregator(_eventAggregator);
            }

            return check;
        }
    }
}
