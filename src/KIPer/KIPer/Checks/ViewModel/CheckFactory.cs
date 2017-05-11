using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using ArchiveData.DTO;
using CheckFrame.Checks;
using CheckFrame.Model;
using CheckFrame.Model.Channels;
using CheckFrame.ViewModel.Checks.Channels;
using KipTM.Archive;
using KipTM.Checks;
using KipTM.Checks.ViewModel;
using KipTM.EventAggregator;
using KipTM.Interfaces.Checks;
using KipTM.Model;
using KipTM.Model.Channels;
using KipTM.Model.Checks;
using KipTM.Model.Devices;
using KipTM.Model.TransportChannels;
using ReportService;
using Tools;
using ITransportChannelType = KipTM.Model.TransportChannels.ITransportChannelType;

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
