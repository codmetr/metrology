using GalaSoft.MvvmLight;
using KIPer.Interfaces;
using KIPer.Model;

namespace KIPer.ViewModel
{
    /// <summary>
    /// This class contains properties that a View can data bind to.
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class Pace5000ViewModel : ViewModelBase, IService
    {
        private readonly PACE5000Model _model;
        /// <summary>
        /// Initializes a new instance of the Pace5000ViewModel class.
        /// </summary>
        public Pace5000ViewModel(PACE5000Model model)
        {
            _model = model;
        }

        public string Title { get { return _model.Title; } }
    }
}