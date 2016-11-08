using ADTSChecks.Checks.Data;
using CheckFrame.ViewModel.Checks;

namespace KipTM.Checks.ViewModel.Config
{
    public class CustomConfigFabrik : ICustomConfigFabrik
    {
        public ICustomSettingsViewModel GetCustomSettings(object customSettings)
        {
            if (customSettings is ADTSParameters)
                return new ADTSChecks.Checks.ViewModel.CheckConfigViewModel(customSettings as ADTSParameters);
            return null;
        }
    }
}
