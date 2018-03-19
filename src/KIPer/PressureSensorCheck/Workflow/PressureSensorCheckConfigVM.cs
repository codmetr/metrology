using System.ComponentModel;
using ArchiveData.DTO;
using KipTM.Checks.ViewModel.Config;
using KipTM.Interfaces;
using PressureSensorData;

namespace PressureSensorCheck.Workflow
{
    /// <summary>
    /// Конфигурация проверки
    /// </summary>
    public class PressureSensorCheckConfigVm : INotifyPropertyChanged
    {
        public PressureSensorCheckConfigVm(TestResultID identificator, PressureSensorConfig configData, DPI620GeniiConfig dpiConf)
        {
            Data = configData;
            Identificator = identificator;
            Config = new CheckPressureLogicConfigVm(configData);
            DpiConfig = dpiConf;
        }

        /// <summary>
        /// Идентифитатор проверки
        /// </summary>
        public TestResultID Identificator { get; }

        /// <summary>
        /// Фактические данные конфигурации
        /// </summary>
        /// <remarks>
        /// Использовать на разметке экрана только в случае единственного места изменения, так как без INotifyPropertyChanged
        /// </remarks>
        public PressureSensorConfig Data { get; }

        /// <summary>
        /// Единицы измерения давления
        /// </summary>
        public Units PressUnit { get { return Data.Unit; }
            set
            {
                if(value == Data.Unit)
                    return;
                Data.Unit = value;
                foreach (var point in Config.Points)
                {
                    point.Unit = value;
                }
                OnPropertyChanged("PressUnit");
            } }

        /// <summary>
        /// Конфигурация логики проверки
        /// </summary>
        public CheckPressureLogicConfigVm Config { get; set; }

        /// <summary>
        /// Конфигурация DPI620
        /// </summary>
        public DPI620GeniiConfig DpiConfig { get; set; }

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}
