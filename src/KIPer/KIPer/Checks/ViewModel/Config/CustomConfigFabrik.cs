using System;
using System.Collections.Generic;
using CheckFrame.ViewModel.Checks;

namespace KipTM.Checks.ViewModel.Config
{
    public class CustomConfigFactory : ICustomConfigFactory
    {
        private Dictionary<Type, ICustomConfigFactory> _factories;

        public CustomConfigFactory(Dictionary<Type, ICustomConfigFactory> factories)
        {
            _factories = new Dictionary<Type, ICustomConfigFactory>(factories);
        }

        public ICustomSettingsViewModel GetCustomSettings(object customSettings)
        {
            if (_factories.ContainsKey(customSettings.GetType()))
                return _factories[customSettings.GetType()].GetCustomSettings(customSettings);
            //if (customSettings is ADTSParameters)
            //    return new ADTSChecks.Checks.ViewModel.CheckConfigViewModel(customSettings as ADTSParameters);
            return null;
        }
    }
}
