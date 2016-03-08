using GalaSoft.MvvmLight;

namespace KipTM.ViewModel.Checks
{
    /// <summary>
    /// This class contains properties that a View can data bind to.
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class PointCheckableViewModel : ViewModelBase
    {
        private readonly string _name;
        private bool _isChecked;

        /// <summary>
        /// Initializes a new instance of the PointChekableViewModel class.
        /// </summary>
        public PointCheckableViewModel(string name)
        {
            _name = name;
            _isChecked = true;
        }

        public bool IsChecked
        {
            get { return _isChecked; }
            set { Set(ref _isChecked, value); }
        }

        public string Name
        {
            get { return _name; }
        }
    }
}