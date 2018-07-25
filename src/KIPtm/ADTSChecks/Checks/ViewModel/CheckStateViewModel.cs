using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using ADTSChecks.ViewModel.Services;
using CheckFrame.ViewModel.Checks;
using KipTM.Model.Checks;

namespace ADTSChecks.Checks.ViewModel
{
    public class CheckStateViewModel : INotifyPropertyChanged
    {
        #region Members
        private string _titleSteps;
        private string _titleBtnNext;
        private bool _waitUserReaction = true;
        private string _note;
        protected ADTSViewModel _adtsViewModel;
        private bool _isUserChannel;
        private object _ethalonChannelViewModel;
        private IEnumerable<StepViewModel> _steps;
        private ObservableCollection<EventArgTestStepResult> _resultsLog;

        #endregion

        /// <summary>
        /// Initializes a new instance of the AdtsCheckStateViewModel class.
        /// </summary>
        public CheckStateViewModel()
        {
        }

        /// <summary>
        /// Название списка шагов
        /// </summary>
        public string TitleSteps
        {
            get { return _titleSteps; }
            set { _titleSteps = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Представление сосотояния ADTS
        /// </summary>
        public ADTSViewModel ADTS
        {
            get { return _adtsViewModel; }
            set { _adtsViewModel = value;
                OnPropertyChanged();
            }
        }

        public bool IsUserChannel
        {
            get { return _isUserChannel; }
            set
            {
                _isUserChannel = value;
                OnPropertyChanged();
                OnPropertyChanged("IsNotUserChannel");
            }
        }

        public bool IsNotUserChannel
        {
            get { return !_isUserChannel; }
        }

        /// <summary>
        /// Описатели шагов проверки
        /// </summary>
        public IEnumerable<StepViewModel> Steps
        {
            get { return _steps; }
            set { _steps = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Представление Эталонного канала
        /// </summary>
        public object EthalonChannelViewModel
        {
            get { return _ethalonChannelViewModel; }
            set { _ethalonChannelViewModel = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Название кнопки Старт/Далее
        /// </summary>
        public string TitleBtnNext
        {
            get { return _titleBtnNext; }
            set { _titleBtnNext = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Примечание к роверке
        /// </summary>
        public string Note
        {
            get { return _note; }
            set { _note = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Ожидать реакцию пользователя
        /// </summary>
        public bool WaitUserReaction
        {
            get { return _waitUserReaction; }
            set { _waitUserReaction = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Результаты
        /// </summary>
        public ObservableCollection<EventArgTestStepResult> ResultsLog
        {
            get { return _resultsLog; }
            set { _resultsLog = value;
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