using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KipTM.Model.Checks
{
    public interface ITestStep
    {
        string Name { get;}

        bool Run();

        bool Stop();

        event EventHandler<EventArgCheckProgress> ProgressChanged;

        event EventHandler<EventArgError> Error;
    }
}
