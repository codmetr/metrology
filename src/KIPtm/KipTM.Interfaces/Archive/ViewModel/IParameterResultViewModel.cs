
namespace CheckFrame.ViewModel.Archive
{
    /// <summary>
    /// Описатель отдельного результата
    /// </summary>
    public interface IParameterResultViewModel : IParameterViewModel
    {
        /// <summary>
        /// Реальная погрешность
        /// </summary>
        string Error { get; set; }
    }
}