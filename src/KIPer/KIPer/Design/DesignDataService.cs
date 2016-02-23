using System;
using KipTM.Interfaces;
using KipTM.Model;

namespace KipTM.Design
{
    public class DesignDataService : IDataService
    {
        public PACE5000Model Pace5000 { get{return new PACE5000Model("DesineTime PASE500", null, "", null);} }
        public void LoadSettings()
        {
            //throw new NotImplementedException();
        }

        public void SaveSettings()
        {
            throw new NotImplementedException();
        }

        public void InitDevices()
        {
            //throw new NotImplementedException();
        }
    }
}