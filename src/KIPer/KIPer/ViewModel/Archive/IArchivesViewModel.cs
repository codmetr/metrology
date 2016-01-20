using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace KIPer.ViewModel
{
    public interface IArchivesViewModel
    {
        /// <summary>
        /// Загрузка базовой конфигурации набора тестов
        /// </summary>
        /// <param name="tests"></param>
        void LoadTests(IEnumerable<ITestResultViewModel> tests);

        /// <summary>
        /// Набор выполненных тестов (процедур поверок/калибровок/аттестаций и пр.) 
        /// </summary>
        ObservableCollection<ITestResultViewModel> TestsCollection { get; set; }
    }
}