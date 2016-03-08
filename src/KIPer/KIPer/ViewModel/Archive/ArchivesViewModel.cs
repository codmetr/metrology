using System.Collections.Generic;
using System.Collections.ObjectModel;
using GalaSoft.MvvmLight;
using KipTM.Model.Archive;

namespace KipTM.ViewModel
{
    /// <summary>
    /// This class contains properties that a View can data bind to.
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class ArchivesViewModel : ViewModelBase, IArchivesViewModel
    {
        private string _meashuredParameter;

        /// <summary>
        /// Initializes a new instance of the TestsViewModel class.
        /// </summary>
        public ArchivesViewModel()
        {

        }

        /// <summary>
        /// Загрузка базовой конфигурации набора тестов
        /// </summary>
        /// <param name="tests"></param>
        public void LoadTests(List<TestResult> tests)
        {
            var testsSet = new List<ITestResultViewModel>();
            foreach (var test in tests)
            {
                var testResVM = new TestResultViewModel();
            }
            TestsCollection = new ObservableCollection<ITestResultViewModel>(tests);
        }

        /// <summary>
        /// Набор выполненных тестов (процедур поверок/калибровок/аттестаций и пр.) 
        /// </summary>
        public ObservableCollection<ITestResultViewModel> TestsCollection { get; set; }

        
    }
}