using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using KIPer.Interfaces;
using KIPer.ViewModel;
using KipTM.ViewModel.Checks;

namespace KipTM.ViewModel
{
    /// <summary>
    /// This class contains properties that a View can data bind to.
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class CheckViewModel : ViewModelBase
    {
        private IDataService _dataService;
        private object _check;
        /// <summary>
        /// Initializes a new instance of the CheckViewModel class.
        /// </summary>
        public CheckViewModel(IDataService dataService, object check)
        {
            _dataService = dataService;
            _check = check;
        }

        public ObservableCollection<IParameterResultViewModel> Parameters { get; set; }

        public IEnumerable<string> DeviceTypes { get; set; }
 
        public IEnumerable<string> CheckTypes { get; set; }
 
        public string SelectedDeviceType { get; set; }

        public string SelectedCheckType { get; set; }

        public string SerialNumber { get; set; }

        public DateTime PreviousCheckTime { get; set; }

        public ICommand Save { get; set; }

        public ICommand Report { get; set; }
        
        public object Check { get { return _check; } }
    }
}