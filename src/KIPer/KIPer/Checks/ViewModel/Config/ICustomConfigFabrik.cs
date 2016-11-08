using CheckFrame.ViewModel.Checks;

namespace KipTM.Checks.ViewModel.Config
{
    public interface ICustomConfigFabrik
    {
        ICustomSettingsViewModel GetCustomSettings(object customSettings);
    }
}