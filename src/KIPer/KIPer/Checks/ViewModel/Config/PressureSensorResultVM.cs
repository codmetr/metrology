using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using ArchiveData.DTO;
using CheckFrame.Checks;
using CheckFrame.ViewModel.Archive;
using KipTM.Archive;
using KipTM.ViewModel;
using Tools.View;

namespace KipTM.Checks.ViewModel.Config
{
    /// <summary>
    /// Визуальная модель результата поверки датчика давления
    /// </summary>
    public class PressureSensorResultVM:INotifyPropertyChanged
    {
        private ITestResultViewModel _result = null;
        private ArchivesViewModel _archive;
        private PressureSensorCheckConfigVm _config;

        public PressureSensorResultVM(ArchivesViewModel archive, PressureSensorCheckConfigVm config)
        {
            _archive = archive;
            _config = config;
            PointResults = new ObservableCollection<PointViewModel>();
        }

        /// <summary>
        /// Результат опробирования
        /// </summary>
        public string Assay { get; set; }

        /// <summary>
        /// Результат проверки на герметичность
        /// </summary>
        public string Leak { get; set; }

        /// <summary>
        /// Общий результат поверки
        /// </summary>
        public string CommonResult { get; set; }

        /// <summary>
        /// Результат визуального осмотра
        /// </summary>
        public string VisualCheckResult { get; set; }

        public ObservableCollection<PointViewModel> PointResults { get; set; }

        /// <summary>
        /// Дата протокола
        /// </summary>
        public DateTime? TimeStamp { get; set; }

        public ICommand Save{ get { return new CommandWrapper(OnSave); } }

        private void OnSave()
        {
            if(_result!=null)
                return;
            _result = new PressureResult(this);
        }

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName = null)
        {

            if (PropertyChanged != null)
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }

    public class PressureResult: ITestResultViewModel
    {
        private readonly PressureSensorResultVM _result;
        public PressureResult(PressureSensorResultVM result)
        {
            _result = result;
            var devType = new DeviceTypeDescriptor(_result.);
            Device = new DeviceViewModel(new DeviceDescriptor(devType)) {DeviceType = devType};
            
        }

        public string TestType { get; set; }
        public string User { get; set; }
        public DateTime Time { get; set; }
        public IDeviceViewModel Device { get; set; }
        public ObservableCollection<IParameterResultViewModel> Parameters { get; set; }
        public ObservableCollection<IDeviceViewModel> Etalons { get; set; }
    }
}


