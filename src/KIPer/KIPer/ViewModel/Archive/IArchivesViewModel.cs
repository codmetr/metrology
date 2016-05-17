using System.Collections.Generic;
using System.Collections.ObjectModel;
using ArchiveData.DTO;
using KipTM.Archive.DTO;

namespace KipTM.ViewModel
{
    public interface IArchivesViewModel
    {
        /// <summary>
        /// Загрузка базовой конфигурации набора тестов
        /// </summary>
        /// <param name="results"></param>
        void LoadTests(ResultsArchive results);

        /// <summary>
        /// Набор выполненных тестов (процедур поверок/калибровок/аттестаций и пр.) 
        /// </summary>
        ObservableCollection<ITestResultViewModel> TestsCollection { get; set; }
    }
}