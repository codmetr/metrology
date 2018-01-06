using System.Collections.Generic;
using System.Collections.ObjectModel;
using ArchiveData.DTO;

namespace KipTM.ViewModel
{
    public interface IArchivesViewModel
    {
        /// <summary>
        /// Загрузка базовой конфигурации набора тестов
        /// </summary>
        /// <param name="results"></param>
        ///void LoadTests(ResultsArchive results);

        /// <summary>
        /// Набор выполненных тестов (процедур поверок/калибровок/аттестаций и пр.) 
        /// </summary>
        //todo: вернуться на более сложную модель, когда будет реализован конвертер настроек в IDeviceViewModel и результатов в набор IParameterResultViewModel
        //ObservableCollection<ITestResultViewModel> TestsCollection { get; set; }
        ObservableCollection<TestResultID> TestsCollection { get; set; }

        /// <summary>
        /// Выбранный тест
        /// </summary>
        TestResultID SelectedTest { get; set; }

        /// <summary>
        /// Представление результата
        /// </summary>
        object Result { get; }
    }
}