using System.Collections.Generic;
using System.Collections.ObjectModel;
using ADTSChecks.ViewModel.Services;
using GalaSoft.MvvmLight;
using KipTM.Model.Checks;
using KipTM.ViewModel.Checks;

namespace ADTSChecks.ViewModel.Checks
{
    /// <summary>
    /// This class contains properties that a View can data bind to.
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class AdtsCheckStateViewModel : ViewModelBase
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
        public AdtsCheckStateViewModel()
        {
        }

        /// <summary>
        /// Название списка шагов
        /// </summary>
        public string TitleSteps
        {
            get { return _titleSteps; }
            set { Set(ref _titleSteps, value); }
        }

        /// <summary>
        /// Представление сосотояния ADTS
        /// </summary>
        public ADTSViewModel ADTS
        {
            get { return _adtsViewModel; }
            set { Set(ref _adtsViewModel, value); }
        }

        public bool IsUserChannel
        {
            get { return _isUserChannel; }
            set
            {
                Set(ref _isUserChannel, value);
                RaisePropertyChanged("IsNotUserChannel");
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
            set { Set(ref _steps, value); }
        }

        /// <summary>
        /// Представление Эталонного канала
        /// </summary>
        public object EthalonChannelViewModel
        {
            get { return _ethalonChannelViewModel; }
            set { Set(ref _ethalonChannelViewModel, value); }
        }

        /// <summary>
        /// Название кнопки Старт/Далее
        /// </summary>
        public string TitleBtnNext
        {
            get { return _titleBtnNext; }
            set { Set(ref _titleBtnNext, value); }
        }

        /// <summary>
        /// Примечание к роверке
        /// </summary>
        public string Note
        {
            get { return _note; }
            set { Set(ref _note, value); }
        }

        /// <summary>
        /// Ожидать реакцию пользователя
        /// </summary>
        public bool WaitUserReaction
        {
            get { return _waitUserReaction; }
            set { Set(ref _waitUserReaction, value); }
        }

        /// <summary>
        /// Результаты
        /// </summary>
        public ObservableCollection<EventArgTestStepResult> ResultsLog
        {
            get { return _resultsLog; }
            set { Set(ref _resultsLog, value); }
        }
    }
}