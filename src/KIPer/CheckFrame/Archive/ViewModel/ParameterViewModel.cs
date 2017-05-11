using GalaSoft.MvvmLight;

namespace CheckFrame.ViewModel.Archive
{
    public class ParameterViewModel : ViewModelBase, IParameterViewModel
    {
        private string _unit;
        private string _pointMeasuring;
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
        public string PointMeasuring
        {
            get { return _pointMeasuring; }
            set { Set(ref _pointMeasuring, value); }
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