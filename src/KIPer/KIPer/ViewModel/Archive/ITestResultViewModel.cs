using System;
using System.Collections.ObjectModel;

namespace KIPer.ViewModel
{
    public interface ITestResultViewModel
    {
        /// <summary>
        /// Тип проводимого теста (Поверка/Калибровка)
        /// </summary>
        string TestType { get; set; }

        /// <summary>
        /// Пользователь проводивший тест
        /// </summary>
        string User { get; set; }

        /// <summary>
        /// Дата и время теста
        /// </summary>
        DateTime Time { get; set; }

        /// <summary>
        /// Проверяемое устройство
        /// </summary>
        IDeviceViewModel Device { get; set; }

        /// <summary>
        /// Список проверяемых параметров
        /// </summary>
        ObservableCollection<IParameterResultViewModel> Parameters { get; set; }

        /// <summary>
        /// Список используемых для проверки эталонов
        /// </summary>
        ObservableCollection<IDeviceViewModel> Etalons { get; set; }
    }
}