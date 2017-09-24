using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using DPI620Genii;
using MahApps.Metro.Controls;
using Microsoft.Research.DynamicDataDisplay;
using Microsoft.Research.DynamicDataDisplay.DataSources;
using Moq;

namespace Dpi620Test
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        public MainWindow()
        {
            InitializeComponent();

            var dpi = new DPI620DriverUsb();
            var moq = GetMoq();

            DataContext = new MainViewModel(moq, new SettingsViewModel()
            {
                PeriodAutoread = TimeSpan.FromSeconds(1)
            }, this.Dispatcher);
        }

        private static IDPI620Driver GetMoq()
        {
            var moq = new Moq.Mock<IDPI620Driver>();

            moq.Setup(drv => drv.Open()).Callback(() => { Dpi620StateMoq.Instance.Start(); });
            moq.Setup(drv => drv.Close()).Callback(() => { Dpi620StateMoq.Instance.Stop(); });

            moq.Setup(drv => drv.SetUnits(It.IsAny<int>(), It.IsAny<string>())).Callback(
                (int slotId, string unitCode) => { Dpi620StateMoq.Instance.SetUnit(slotId, unitCode); });

            moq.Setup(drv => drv.GetValue(It.IsAny<int>(), It.IsAny<string>()))
                .Returns((int slotId, string unitCode) => Dpi620StateMoq.Instance.GetValue(slotId, unitCode));
            return moq.Object;
        }

        private void FrameworkElement_OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var chart = sender as ChartPlotter;
            var prop = e.NewValue as SlotViewModel;
            var source = new ObservableDataSource<Point>();
            source.AppendMany(prop.ReadedPoints.Select(el=>new Point(el.TimeStamp.Ticks/ 10000000000.0, el.Val)));
            prop.ReadedPoints.CollectionChanged += (sen, args) =>
            {
                if (args.Action == NotifyCollectionChangedAction.Add)
                {
                    for (int i = 0; i < args.NewItems.Count; i++)
                    {
                        var el = args.NewItems[i] as OnePointViewModel;
                        if (el != null)
                            source.AppendAsync(this.Dispatcher, new Point(el.TimeStamp.Ticks / 10000000000.0, el.Val));
                    }
                }
                else if (args.Action == NotifyCollectionChangedAction.Reset)
                {
                    source.Collection.Clear();
                }
            };
            var line = chart.AddLineGraph(source, Colors.Brown, 1, prop.Name);
        }


        internal class Dpi620StateMoq
        {
            internal static Dpi620StateMoq Instance { get; } = new Dpi620StateMoq();

            Random _rnd = new Random();
            private double slot1Max = 1000.0;
            private double slot2Max = 1000.0;
            private bool _isOpened = false;

            private Dpi620StateMoq(){}

            public double GetValue(int slot, string unit)
            {
                if (!_isOpened)
                    return 0.0;
                var val = _rnd.NextDouble() * (slot == 1 ? slot1Max : slot2Max);
                return val;
            }

            public void SetUnit(int slot, string unit)
            {
            }

            public void Start()
            {
                _isOpened = true;
            }

            public void Stop()
            {
                _isOpened = false;
            }
        }
    }
}
