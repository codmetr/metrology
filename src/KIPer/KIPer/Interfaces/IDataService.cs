using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KIPer.Model;

namespace KIPer.Interfaces
{
    public interface IDataService
    {
        PACE5000Model Pace5000 { get; }
        void LoadSettings();
        void InitDevices();
    }
}
