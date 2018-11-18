using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using ArchiveData;
using ArchiveData.DTO;
using ReportService;

namespace KipTM.ViewModel
{
    public class ArchivesViewModel : IArchivesViewModel, INotifyPropertyChanged
    {
        private readonly IDataPool _dataPool;
        private TestResultID _selectedTest;
        private readonly Dictionary<string, IReportFactory> _reportFactories;
        private object _result;
        private bool _isBuzy;

        /// <summary>
        /// Initializes a new instance of the TestsViewModel class.
        /// </summary>
        /// <param name="dataPool"></param>
        /// <param name="reportFactories"></param>
        public ArchivesViewModel(IDataPool dataPool, Dictionary<string, IReportFactory> reportFactories)
        {
            _dataPool = dataPool;
            _reportFactories = reportFactories;
            TestsCollection = new ObservableCollection<TestResultID>(dataPool.Repairs.Keys);
        }

        ///// <summary>
        ///// Загрузка базовой конфигурации набора тестов
        ///// </summary>
        ///// <param name="results"></param>
        //public void LoadTests(ResultsArchive results)
        //{
            // TODO продумать получение готового результата
            //TestsCollection = new ObservableCollection<ITestResultViewModel>(results.Results.Select(el => new TestResultViewModel(el, null, null, null)));

        //}

        /// <summary>
        /// Набор выполненных тестов (процедур поверок/калибровок/аттестаций и пр.) 
        /// </summary>
        //todo: вернуться на более сложную модель, когда будет реализован конвертер настроек в IDeviceViewModel и результатов в набор IParameterResultViewModel
        //public ObservableCollection<ITestResultViewModel> TestsCollection { get; set; }
        public ObservableCollection<TestResultID> TestsCollection { get; set; }

        /// <summary>
        /// Выбранный тест
        /// </summary>
        public TestResultID SelectedTest
        {
            get { return _selectedTest; }
            set
            {
                _selectedTest = value;
                OnPropertyChanged("SelectedTest");
                UpdateResult(_selectedTest);
            }
        }

        /// <summary>
        /// Представление результата
        /// </summary>
        public object Result
        {
            get { return _result; }
            set
            {
                _result = value;
                OnPropertyChanged("Result");
            }
        }

        /// <summary>
        /// Выполняетс длительная операция
        /// </summary>
        public bool IsBuzy
        {
            get { return _isBuzy; }
            set
            {
                _isBuzy = value;
                OnPropertyChanged("IsBuzy");
            }
        }

        /// <summary>
        /// Обновить состояние результата по заданному тесту
        /// </summary>
        /// <param name="id"></param>
        private void UpdateResult(TestResultID id)
        {
            var factory = _reportFactories[id.TargetDeviceKey];
            var result = _dataPool.Repairs[id];
            var config = _dataPool.Configs[id];
            Result = factory.GetReporter(id, config, result);
        }

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}