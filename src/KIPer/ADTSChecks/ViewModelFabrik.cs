using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ADTSChecks.Checks.Data;
using ADTSChecks.Checks.ViewModel;
using ADTSChecks.Model.Checks;
using ADTSChecks.Model.Devices;
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
            var method = methodObj as CheckBase;
            if (method == null)
                return result;

            method.SetADTS(deviceManager.GetModel<ADTSModel>());
            method.ChannelType = checkDeviceTransport;
            method.SetEthalonChannel(ethalonChannel, ethalonTransport);

            if (method is Calibration)
            {
                var adtsMethodic = method as Calibration;
                result = new CalibrationViewModel(adtsMethodic, propertyPool,
                    deviceManager, testResult, customSettings as ADTSParameters);
            }
            else if (method is Test)
            {
                var adtsMethodic = method as Test;
                result = new TestViewModel(adtsMethodic, propertyPool,
                    deviceManager, testResult, customSettings as ADTSParameters);
            }
            if (result != null)
            {
                result.SetEthalonChannel(ethalonTypeKey, ethalonTransport);
            }

            return result;
        }
    }
}
