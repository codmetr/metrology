using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KipTM.Archive;
using KipTM.Model;
using KipTM.Model.Checks;
using KipTM.Model.Devices;

namespace KipTM.ViewModel.Checks
{
    public class CheckFabrik : ICheckFabrik
    {
        private readonly IDeviceManager _deviceManager;
        private readonly IPropertyPool _propertyPool;

        public CheckFabrik(IDeviceManager deviceManager, IPropertyPool propertyPool)
        {
            _deviceManager = deviceManager;
            _propertyPool = propertyPool;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IMethodViewModel GetViewModelFor(CheckConfig checkConfig)
        {
            var method = checkConfig.SelectedCheckType;
            if (method is ADTSCheckMethod)
            {
                var adtsMethodic = method as ADTSCheckMethod;
                adtsMethodic.SetADTS(_deviceManager.GetModel<ADTSModel>());
                return new ADTSCalibrationViewModel(adtsMethodic, _propertyPool.ByKey(checkConfig.SelectedDeviceTypeKey), _deviceManager, checkConfig.Result);
            }
            else if (method is ADTSTestMethod)
            {
                var adtsMethodic = method as ADTSTestMethod;
                adtsMethodic.SetADTS(_deviceManager.GetModel<ADTSModel>());
                return new ADTSTestViewModel(adtsMethodic, _propertyPool.ByKey(checkConfig.SelectedDeviceTypeKey), _deviceManager, checkConfig.Result);
            }
            return null;
        }
    }
}
