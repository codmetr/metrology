using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KIPer.Model
{
    public interface ICheckModel
    {
        string Point { get; }
        string Error { get; }

        bool CheckPoint();
    }
}
