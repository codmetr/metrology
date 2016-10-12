using ADTSChecks.Model.Checks;
using ADTSChecks.ViewModel.Checks;
using CheckFrame.ViewModel.Checks;

namespace KipTM.ViewModel.Checks.Config
{
    public class CustomConfigFabrik
    {
        public ICustomSettingsViewModel GetCustomSettings(object customSettings)
        {
            if (customSettings is ADTSMethodParameters)
                return new ADTSCheckConfigViewModel(customSettings as ADTSMethodParameters);
            return null;
        }
    }
}
