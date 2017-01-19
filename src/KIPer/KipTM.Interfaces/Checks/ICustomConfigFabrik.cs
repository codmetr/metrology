using CheckFrame.ViewModel.Checks;

namespace KipTM.Checks.ViewModel.Config
{
    /// <summary>
    /// Формирователь представления специализтрованной настройки
    /// </summary>
    public interface ICustomConfigFactory
    {
        ICustomSettingsViewModel GetCustomSettings(object customSettings);
    }
}