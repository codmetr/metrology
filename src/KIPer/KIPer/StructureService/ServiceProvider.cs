using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KIPer.StructureService
{
    public class ServiceProvider : IServiceProvider
    {
        public object GetService(Type serviceType)
        {
            return new object();
        }
    }
}
