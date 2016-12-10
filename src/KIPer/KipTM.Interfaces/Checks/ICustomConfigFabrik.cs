using CheckFrame.ViewModel.Checks;

namespace KipTM.Checks.ViewModel.Config
{
    public interface ICustomConfigFactory
    {
        ICustomSettingsViewModel GetCustomSettings(object customSettings);
    }
}