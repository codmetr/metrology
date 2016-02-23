using System.Collections.ObjectModel;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using KipTM.ViewModel;

namespace KipTM.ViewModel.Checks
{
    /// <summary>
    /// This class contains properties that a View can data bind to.
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class MechanicalManometerViewModel : ViewModelBase
    {
        /// <summary>
        /// Initializes a new instance of the MechanicalManometerViewModel class.
        /// </summary>
        public MechanicalManometerViewModel()
        {
        }

        public double PointValue { get; set; }

        public double RealValue { get; set; }

        public bool EnabledNext { get; set; }

        public ICommand IncrementCommand { get; set; }

        public ICommand DecrementCommand { get; set; }

        public ICommand NextCommand { get; set; }

        public ObservableCollection<IParameterResultViewModel> Results { get; set; } 
    }
}