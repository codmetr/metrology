using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using ArchiveData.DTO;
using Core.Archive.DataTypes;
using PressureSensorData;
using Tools.View;

namespace PressureSensorCheck.Workflow
{
    /// <summary>
    /// Визуальная модель результата поверки датчика давления
    /// </summary>
    public class PressureSensorResultVM:INotifyPropertyChanged
    {
        /// <summary>
        /// Хранилище результата для конкретной проверки
        /// </summary>
        private readonly IDataAccessor _accessor;

        public PressureSensorResultVM(TestResultID checkResId, IDataAccessor accessor, PressureSensorResult result = null)
        {
            Identificator = checkResId;
            _accessor = accessor;
            PointResults = new ObservableCollection<PointViewModel>();
            Data = result;
        }

        /// <summary>
        /// Текущий результат проверки
        /// </summary>
        public PressureSensorResult Data { get; set; }

        /// <summary>
        /// Идентифитатор проверки
        /// </summary>
        public TestResultID Identificator { get; }

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

        /// <summary>
        /// Сохранить
        /// </summary>
        public ICommand Save{ get { return new CommandWrapper(OnSave); } }

        /// <summary>
        /// Фактическое выполнение сохранение
        /// </summary>
        private void OnSave()
        {
            _accessor.Save(Identificator, Data);
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
}


