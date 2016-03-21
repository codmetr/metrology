using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace KipTM.Model.Checks
{
    public interface ITestStep
    {
        string Name { get;}

        void Start(EventWaitHandle whEnd);

        bool Stop();

        event EventHandler<EventArgTestResult> ResultUpdated;

        event EventHandler<EventArgProgress> ProgressChanged;

        event EventHandler<EventArgError> Error;
    }
}
