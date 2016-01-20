namespace KIPer.ViewModel
{
    public interface IParameterResultViewModel : IParameterViewModel
    {
        /// <summary>
        /// Реальная погрешность
        /// </summary>
        string Error { get; set; }
    }
}