namespace CheckFrame.Model.Checks.Steps
{
    /// <summary>
    /// Шаг поддерживает установку текущей точки как ключевой
    /// </summary>
    public interface IStoppedOnPoint
    {
        /// <summary>
        /// Установить текущую точку как ключевую
        /// </summary>
        void SetCurrentValueAsPoint();
    }
}
