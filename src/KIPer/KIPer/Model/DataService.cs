using System;

namespace KIPer.Model
{
    public class DataService : IDataService
    {
        MainLoop.Loops _loops = new MainLoop.Loops();

        public void GetData(Action<PACE5000Model, Exception> callback)
        {
            // Use this to connect to the actual data service

            var item = new PACE5000Model("Welcome to MVVM Light");
            callback(item, null);
        }

        public void InitDevices()
        {
            
        }
    }
}