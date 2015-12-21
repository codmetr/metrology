using System;

namespace KIPer.Model
{
    public class DataService : IDataService
    {
        MainLoop.Loops _loops = new MainLoop.Loops();

        public void GetData(Action<DataItem, Exception> callback)
        {
            // Use this to connect to the actual data service

            var item = new DataItem("Welcome to MVVM Light");
            callback(item, null);
        }

        public void InitDevices()
        {
            
        }
    }
}