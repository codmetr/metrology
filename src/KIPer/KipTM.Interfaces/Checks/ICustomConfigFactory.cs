using CheckFrame.ViewModel.Checks;

namespace KipTM.Checks.ViewModel.Config
{
    /// <summary>
    /// Формирователь представления специализтрованной настройки
    /// </summary>
    public interface ICustomConfigFactory
    {
        /// <summary>
        /// Сформировать визуальную модель специализтрованной настройки
        /// </summary>
        /// <param name="customSettings">Специализтрованая настройка</param>
        /// <returns>Визуальную модель специализтрованной настройки</returns>
        ICustomSettingsViewModel GetCustomSettings(object customSettings);
    }
}