using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
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
using KipTM.Checks.ViewModel.Config;
using Microsoft.Research.DynamicDataDisplay;
using Microsoft.Research.DynamicDataDisplay.DataSources;
using PressureSensorCheck.Workflow;
using Tools.View;

namespace KipTM.Checks.View
{
    /// <summary>
    /// Interaction logic for PressureSensorRunView.xaml
    /// </summary>
    [View(typeof(PressureSensorRunVm))]
    public partial class PressureSensorRunView : UserControl
    {
        public PressureSensorRunView()
        {
            InitializeComponent();
        }

        private void Plotter_OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var chart = sender as ChartPlotter;
            var prop = e.NewValue as ObservableCollection<MeasuringPoint>;
            if(prop == null)
                return;
            var source = new ObservableDataSource<Point>();
            source.AppendMany(prop.Select(el => new Point(el.TimeStamp.Ticks / 10000000000.0, el.I)));
            prop.CollectionChanged += (sen, args) =>
            {
                if (args.Action == NotifyCollectionChangedAction.Add)
                {
                    for (int i = 0; i < args.NewItems.Count; i++)
                    {
                        var el = args.NewItems[i] as MeasuringPoint;
                        if (el != null)
                            source.AppendAsync(this.Dispatcher, new Point(el.TimeStamp.Ticks / 10000000000.0, el.I));
                    }
                }
                else if (args.Action == NotifyCollectionChangedAction.Reset)
                {
                    source.Collection.Clear();
                }
            };
            var line = chart.AddLineGraph(source, Colors.Brown, 1, "I");
        }
    }
}
