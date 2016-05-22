using GalaSoft.MvvmLight;
using KipTM.Model.Channels;

namespace KipTM.ViewModel.Channels
{
    /// <summary>
    /// This class contains properties that a View can data bind to.
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class PACEEchalonChannelViewModel : ViewModelBase
    {
        private PACEEchalonChannel _model;

        /// <summary>
        /// Initializes a new instance of the PACEEchalonChannelViewModel class.
        /// </summary>
        public PACEEchalonChannelViewModel(PACEEchalonChannel model)
        {
            _model = model;
        }


    }
}