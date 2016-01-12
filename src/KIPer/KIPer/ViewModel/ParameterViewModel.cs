using GalaSoft.MvvmLight;

namespace KIPer.ViewModel
{
    /// <summary>
    /// This class contains properties that a View can data bind to.
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class ParameterViewModel : ViewModelBase, IParameterViewModel
    {
        private string _unit;
        private string _pointMeashuring;
        private string _error;
        private string _tolerance;

        /// <summary>
        /// Initializes a new instance of the ParameterViewModel class.
        /// </summary>
        public ParameterViewModel()
        {
        }

        /// <summary>
        /// Единицы измерения параметра
        /// </summary>
        public string Unit
        {
            get { return _unit; }
            set { Set(ref _unit, value); }
        }

        /// <summary>
        /// Проверяемая величина параметра
        /// </summary>
        public string PointMeashuring
        {
            get { return _pointMeashuring; }
            set { Set(ref _pointMeashuring, value); }
        }

        /// <summary>
        /// Погрешность
        /// </summary>
        public string Error
        {
            get { return _error; }
            set { Set(ref _error, value); }
        }

        /// <summary>
        /// Допуск параметра на заданной точке
        /// </summary>
        public string Tolerance
        {
            get { return _tolerance; }
            set { Set(ref _tolerance, value); }
        }
    }
}