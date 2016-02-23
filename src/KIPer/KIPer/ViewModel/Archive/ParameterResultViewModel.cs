namespace KipTM.ViewModel
{
    /// <summary>
    /// This class contains properties that a View can data bind to.
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class ParameterResultViewModel : ParameterViewModel, IParameterResultViewModel
    {
        private string _error;

        /// <summary>
        /// Initializes a new instance of the ParameterViewModel class.
        /// </summary>
        public ParameterResultViewModel()
        {
        }

        /// <summary>
        /// Погрешность
        /// </summary>
        public string Error
        {
            get { return _error; }
            set { Set(ref _error, value); }
        }
    }
}