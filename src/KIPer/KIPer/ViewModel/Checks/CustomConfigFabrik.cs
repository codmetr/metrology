using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KipTM.Model.Checks;

namespace KipTM.ViewModel.Checks
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
