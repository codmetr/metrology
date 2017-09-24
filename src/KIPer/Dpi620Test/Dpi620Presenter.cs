using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DPI620Genii;

namespace Dpi620Test
{
    internal class Dpi620Presenter
    {
        private readonly IDPI620Driver _dpi620;

        public Dpi620Presenter(IDPI620Driver dpi620)
        {
            _dpi620 = dpi620;
        }

        public void Open()
        {
            _dpi620.Open();
        }

        public void Close()
        {
            _dpi620.Close();
        }

        public IEnumerable<string> UnitsSlot1 { get; } = new[] {"mbar", "mhg"};

        public IEnumerable<string> UnitsSlot2 { get; } = new[] { "V", "mV" };
    }
}
