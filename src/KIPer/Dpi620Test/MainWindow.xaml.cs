using System;
using System.Collections.Specialized;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using DPI620Genii;
using MahApps.Metro.Controls;
using Microsoft.Research.DynamicDataDisplay;
using Microsoft.Research.DynamicDataDisplay.DataSources;
using Moq;
using Tools;

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

            var dpiLog = NLog.LogManager.GetLogger("Dpi620");
            var dpiCom = new DPI620DriverCom().Setlog((msg)=>dpiLog.Trace(msg));
            var moq = GetMoq();

            var dpi = AppVersionHelper.CurrentAppVersionType == AppVersionHelper.AppVersionType.Emulation
                ? moq : dpiCom;

            var ports = System.IO.Ports.SerialPort.GetPortNames();
            var selectedPort = ports.FirstOrDefault();
            if (ports.Contains(Properties.Settings.Default.Port))
            {
                selectedPort = Properties.Settings.Default.Port;
            }
            else if(ports.Length!=0)
            {
                Properties.Settings.Default.Port = selectedPort;
                Properties.Settings.Default.Save();
            }
            var settings = new SettingsViewModel()
            {
                SelectedPort = selectedPort,
                Ports = ports,
                PeriodAutoread = TimeSpan.FromMilliseconds(100)
            };

            Action prepare = () =>
            {
                if (AppVersionHelper.CurrentAppVersionType != AppVersionHelper.AppVersionType.Emulation)
                    dpiCom.SetPort(settings.SelectedPort);
            };
            DataContext = new MainViewModel(dpi, settings, this.Dispatcher, prepare);
        }

        private static IDPI620Driver GetMoq()
        {
            var moq = new Moq.Mock<IDPI620Driver>();

            moq.Setup(drv => drv.Open()).Callback(() => { Dpi620StateMoq.Instance.Start(); });
            moq.Setup(drv => drv.Close()).Callback(() => { Dpi620StateMoq.Instance.Stop(); });

            //moq.Setup(drv => drv.SetUnits(It.IsAny<int>(), It.IsAny<string>())).Callback(
            //    (int slotId, string unitCode) => { Dpi620StateMoq.Instance.SetUnit(slotId, unitCode); });

            moq.Setup(drv => drv.GetValue(It.IsAny<int>()))
                .Returns((int slotId) => Dpi620StateMoq.Instance.GetValue(slotId));
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


        protected class Dpi620StateMoq
        {
            internal static Dpi620StateMoq Instance { get; } = new Dpi620StateMoq();

            Random _rnd = new Random();
            private double slot1Max = 1000.0;
            private double slot2Max = 1000.0;
            private bool _isOpened = false;

            private Dpi620StateMoq(){}

            public double GetValue(int slot)
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
