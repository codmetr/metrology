using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Graphic
{

    public class LinesInOutViewModel:INotifyPropertyChanged
    {
        private List<LineDescriptor> _lines;
        private ObservableCollection<PointData> _lineIn = new ObservableCollection<PointData>();
        private ObservableCollection<PointData> _lineOut = new ObservableCollection<PointData>();

        public LinesInOutViewModel(
            string l1Title, string l1Asix, Color l1Color, int l1With, TimeSpan l1Period,
            string l2Title, string l2Asix, Color l2Color, int l2With, TimeSpan l2Period)
        {
            _lines = new List<LineDescriptor>() {new LineDescriptor()
                {
                    Title = l1Title,
                    AzixTitle = l1Asix,
                    LineColor = l1Color,
                    LimitForLine = l1Period,
                    Source = _lineIn,
                    Width = l1With,
                },
                new LineDescriptor()
                {
                    Title = l2Title,
                    AzixTitle = l2Asix,
                    LineColor = l2Color,
                    LimitForLine = l2Period,
                    Source = _lineOut,
                    Width = l2With,
                },
            };
        }

        public void CLearAllLines()
        {
            _lineIn.Clear();
            _lineOut.Clear();
        }

        public void AddPoint(TimeSpan time, double inVal, double outVal)
        {
            _lineIn.Add(new PointData()
            {
                Time = time,
                Value = inVal
            });
            _lineOut.Add(new PointData()
            {
                Time = time,
                Value = outVal
            });
        }


        /// <summary>
        /// Линии на графике
        /// </summary>
        public IEnumerable<LineDescriptor> Lines { get { return _lines; } }


        public CleanerAct LineCleaner { get; private set; }


        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}
