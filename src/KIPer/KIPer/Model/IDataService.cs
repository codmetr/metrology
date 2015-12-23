using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KIPer.Model
{
    public interface IDataService
    {
        void GetData(Action<PACE5000Model, Exception> callback);
    }
}
