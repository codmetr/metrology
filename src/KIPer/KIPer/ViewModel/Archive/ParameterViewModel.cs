using GalaSoft.MvvmLight;

namespace KIPer.ViewModel
{
    public class ParameterViewModel : ViewModelBase, IParameterViewModel
    {
        private string _unit;
        private string _pointMeashuring;
        private string _tolerance;
        private string _nameParameter;

        /// <summary>
        ///  Имя параметра
        /// </summary>
        public string NameParameter
        {
            get { return _nameParameter; }
            set { Set(ref _nameParameter, value); }
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
        /// Допуск параметра на заданной точке
        /// </summary>
        public string Tolerance
        {
            get { return _tolerance; }
            set { Set(ref _tolerance, value); }
        }
    }
}