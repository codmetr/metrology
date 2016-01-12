using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace KIPer.ViewModel
{
    public interface ITestsViewModel
    {
        /// <summary>
        /// Загрузка базовой конфигурации набора тестов
        /// </summary>
        /// <param name="tests"></param>
        void LoadTests(IEnumerable<ITestViewModel> tests);

        /// <summary>
        /// Набор выполненных тестов (процедур поверок/калибровок/аттестаций и пр.) 
        /// </summary>
        ObservableCollection<ITestViewModel> TestsCollection { get; set; }
    }
}