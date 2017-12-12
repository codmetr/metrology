using System;
using System.Collections.ObjectModel;
using ArchiveData.DTO;
using CheckFrame.ViewModel.Archive;
using KipTM.ViewModel;
using PressureSensorCheck.Workflow;

namespace KipTM.Checks.ViewModel.Config
{
    public class PressureResult: ITestResultViewModel
    {
        private readonly PressureSensorResultVM _result;
        public PressureResult(PressureSensorResultVM result)
        {
            _result = result;
            var devType = new DeviceTypeDescriptor(/*_result.Config.SensorModel, _result.Config.*/);
            Device = new DeviceViewModel(new DeviceDescriptor(devType)) {DeviceType = devType};
        }

        /// <summary>
        /// Тип проводимого теста (Поверка/Калибровка)
        /// </summary>
        public string TestType { get; set; }
        public string User { get; set; }
        public DateTime Time { get; set; }
        public IDeviceViewModel Device { get; set; }
        public ObservableCollection<IParameterResultViewModel> Parameters { get; set; }
        public ObservableCollection<IDeviceViewModel> Etalons { get; set; }
    }
}