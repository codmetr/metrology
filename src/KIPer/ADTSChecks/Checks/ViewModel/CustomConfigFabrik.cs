using ADTSChecks.Checks.Data;
using CheckFrame.ViewModel.Checks;
using KipTM.Interfaces.Checks;

namespace KipTM.Checks.ViewModel.Config
{
    [CustomSettings(typeof(ADTSParameters))]
    public class CustomConfigFabrik : ICustomConfigFactory
    {
        public ICustomSettingsViewModel GetCustomSettings(object customSettings)
        {
            if (customSettings is ADTSParameters)
                return new ADTSChecks.Checks.ViewModel.AdtsCheckConfVm(customSettings as ADTSParameters);
            return null;
        }
    }
}
