using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using KipTM.Model.Checks;
using KipTM.ViewModel;

namespace KipTM.ViewModel.Checks
{
    /// <summary>
    /// This class contains properties that a View can data bind to.
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class ADTSCalibrationViewModel : ViewModelBase
    {
        private string _titleBtnNext;
        private ADTSCheckMethodic _methodic;

        /// <summary>
        /// Initializes a new instance of the ADTSCalibrationViewModel class.
        /// </summary>
        public ADTSCalibrationViewModel(ADTSCheckMethodic methodic)
        {
            _methodic = methodic;
        }

        public string TitleBtnNext
        {
            get { return _titleBtnNext; }
            set { Set(ref _titleBtnNext, value); }
        }

        public IEnumerable<PointCheckableViewModel> Points { get; private set; }

        public ICommand StepCommand { get{return new RelayCommand(()=>{});} }

        public ObservableCollection<IParameterResultViewModel> Results { get; set; }
    }
}