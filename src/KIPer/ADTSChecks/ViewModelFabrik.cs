using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ADTSChecks.Model.Checks;
using ADTSChecks.Model.Devices;
using ADTSChecks.ViewModel.Checks;
using ArchiveData.DTO;
using CheckFrame.Archive;
using CheckFrame.Model;
using CheckFrame.Model.Channels;
using CheckFrame.Model.Checks;
using CheckFrame.Model.TransportChannels;
using CheckFrame.ViewModel.Checks.Channels;

namespace ADTSChecks
{
    public class ViewModelFabrik : ADTSChecks.IViewModelFabrik
    {
        /// <summary>
        /// получить презентор типа проверки
        /// </summary>
        /// <returns></returns>
        public IMethodViewModel GetViewModelFor(ICheckMethod methodObj, IEthalonChannel ethalonChannel,
            string ethalonTypeKey, ITransportChannelType checkDeviceTransport, ITransportChannelType ethalonTransport,
            IDeviceManager deviceManager, IPropertyPool propertyPool, TestResult testResult, object customSettings)
        {
            IMethodViewModel result = null;
            var method = methodObj as ADTSMethodBase;
            if (method != null)
            {
                method.SetADTS(deviceManager.GetModel<ADTSModel>());
                method.ChannelType = checkDeviceTransport;
                method.SetEthalonChannel(ethalonChannel, ethalonTransport);

                if (method is AdtsCheckMethod)
                {
                    var adtsMethodic = method as AdtsCheckMethod;
                    result = new ADTSCalibrationViewModel(adtsMethodic, propertyPool,
                        deviceManager, testResult, customSettings as ADTSMethodParameters);
                }
                else if (method is ADTSTestMethod)
                {
                    var adtsMethodic = method as ADTSTestMethod;
                    result = new ADTSTestViewModel(adtsMethodic, propertyPool,
                        deviceManager, testResult, customSettings as ADTSMethodParameters);
                }
                if (result != null)
                {
                    result.SetEthalonChannel(ethalonTypeKey, ethalonTransport);
                }
            }

            return result;
        }
    }
}
