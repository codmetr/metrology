using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KIPer.Model
{
    public class TestModel
    {
        public TestModel()
        {
            
        }

        public IEnumerable<ICheckModel> Checks { get; private set; } 
    }
}
