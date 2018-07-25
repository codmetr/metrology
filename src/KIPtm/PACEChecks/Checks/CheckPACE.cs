using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using KipTM.Archive;
using KipTM.Interfaces.Checks;
using KipTM.Model.Checks;

namespace PACEChecks
{
    public class CheckPACE : ICheckMethod
    {
        private string _key = "PACECheck";

        #region ICheckMethod implimentation

        public string Key
        {
            get { return _key; }
        }

        public string Title { get { return "Поверка PACE"; } }

        public object GetCustomConfig(IPropertyPool propertyPool)
        {
            return null;
        }

        public bool Init(object customConf)
        {
            throw new NotImplementedException();
        }

        public bool Start(CancellationToken cancel)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<CheckStepConfig> Steps { get; private set; }

        #endregion

    }
}
