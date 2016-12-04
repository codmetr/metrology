using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ArchiveData.DTO;
using CheckFrame.Checks;
using CheckFrame.ViewModel.Checks.Channels;
using KipTM.Checks;
using KipTM.Model.TransportChannels;

namespace KipTM.Interfaces.Checks
{

    public interface ICheckViewModelFactory
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="method"></param>
        /// <param name="checkConfig"></param>
        /// <param name="customSettings"></param>
        /// <param name="resultSet"></param>
        /// <param name="checkDeviceChanel"></param>
        /// <param name="ethalonChanel"></param>
        /// <returns></returns>
        IMethodViewModel GetViewModel(object method, CheckConfigData checkConfig, object customSettings,
            TestResult resultSet, ITransportChannelType checkDeviceChanel, ITransportChannelType ethalonChanel);
    }
}
