namespace KIPer.ViewModel
{
    public interface IParameterViewModel
    {
        /// <summary>
        ///  Имя параметра
        /// </summary>
        string NameParameter { get; set; }

        /// <summary>
        /// Единицы измерения параметра
        /// </summary>
        string Unit { get; set; }

        /// <summary>
        /// Проверяемая величина параметра
        /// </summary>
        string PointMeashuring { get; set; }

        /// <summary>
        /// Допуск параметра на заданной точке
        /// </summary>
        string Tolerance { get; set; }
    }
}