using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using KipTM.Archive;
using KipTM.Interfaces.Checks;
using KipTM.Model.Checks;

namespace PressureSensorCheck.Check
{
    public class PressureSensorCheck: ICheckMethod
    {
        public string Key { get; }
        public string Title { get; }
        public object GetCustomConfig(IPropertyPool propertyPool)
        {
            throw new NotImplementedException();
        }

        public bool Init(object customConf)
        {
            throw new NotImplementedException();
        }

        public bool Start(CancellationToken cancel)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<CheckStepConfig> Steps { get; }
    }
}
