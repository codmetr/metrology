using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace CheckFrame.ViewModel.Archive
{
    public class ParameterViewModel : INotifyPropertyChanged, IParameterViewModel
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
            set { _nameParameter = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Единицы измерения параметра
        /// </summary>
        public string Unit
        {
            get { return _unit; }
            set { _unit = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Проверяемая величина параметра
        /// </summary>
        public string PointMeasuring
        {
            get { return _pointMeasuring; }
            set { _pointMeasuring = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Допуск параметра на заданной точке
        /// </summary>
        public string Tolerance
        {
            get { return _tolerance; }
            set
            {
                _tolerance = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}