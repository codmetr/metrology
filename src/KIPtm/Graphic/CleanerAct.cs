using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Graphic
{
    public class CleanerAct
    {
        public event Action Clear;

        public void OnClear()
        {
            Clear?.Invoke();
        }
    }
}
