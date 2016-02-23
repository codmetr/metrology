using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KipTM.Model;

namespace KipTM.Interfaces
{
    public interface IDataService
    {
        PACE5000Model Pace5000 { get; }
        void LoadSettings();
        void SaveSettings();
        void InitDevices();
    }
}
