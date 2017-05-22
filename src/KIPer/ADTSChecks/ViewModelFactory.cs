using ADTSChecks.Checks.Data;
using ADTSChecks.Checks.ViewModel;
using ADTSChecks.Model.Checks;
using ADTSChecks.Model.Devices;
using ArchiveData.DTO;
using CheckFrame.Model;
using CheckFrame.Model.Channels;
using CheckFrame.ViewModel.Checks.Channels;
using KipTM.Archive;
using KipTM.Interfaces.Checks;
using KipTM.Model;
using KipTM.Model.Channels;
using KipTM.Model.Checks;
using KipTM.Model.TransportChannels;

namespace ADTSChecks
{
    public class ViewModelFactory : ADTSChecks.IViewModelFactory
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
