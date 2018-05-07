
namespace CheckFrame.ViewModel.Archive
{
    public class ParameterResultViewModel : ParameterViewModel, IParameterResultViewModel
    {
        private string _error;
        private string _errorUnit;

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
            set { _error = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Единицы измерения параметра
        /// </summary>
        public string ErrorUnit
        {
            get { return _errorUnit; }
            set { _errorUnit = value;
                OnPropertyChanged();
            }
        }
    }
}