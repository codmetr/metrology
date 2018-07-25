namespace Tools.View.Busy
{
    /// <summary>
    /// Пользовательский интерфейс умеющий говорить "Я занят"
    /// </summary>
    public interface IBusy
    {
        /// <summary>
        /// Состояние "Я занят"
        /// </summary>
        bool IsBusy { get; set; }
    }
}