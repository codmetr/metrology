using System;
using KIPer.Model;

namespace KIPer.Design
{
    public class DesignDataService : IDataService
    {
        public void GetData(Action<PACE5000Model, Exception> callback)
        {
            // Use this to create design time data

            var item = new PACE5000Model("Welcome to MVVM Light [design]");
            callback(item, null);
        }
    }
}