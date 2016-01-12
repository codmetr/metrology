namespace KIPer.ViewModel
{
    public interface IParameterViewModel
    {
        /// <summary>
        /// Единицы измерения параметра
        /// </summary>
        string Unit { get; set; }

        /// <summary>
        /// Проверяемая величина параметра
        /// </summary>
        string PointMeashuring { get; set; }

        /// <summary>
        /// Погрешность
        /// </summary>
        string Error { get; set; }

        /// <summary>
        /// Допуск параметра на заданной точке
        /// </summary>
        string Tolerance { get; set; }
    }
}