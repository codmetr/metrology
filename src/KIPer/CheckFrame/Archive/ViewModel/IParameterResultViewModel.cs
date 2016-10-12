
namespace CheckFrame.ViewModel.Archive
{
    public interface IParameterResultViewModel : IParameterViewModel
    {
        /// <summary>
        /// Реальная погрешность
        /// </summary>
        string Error { get; set; }
    }
}