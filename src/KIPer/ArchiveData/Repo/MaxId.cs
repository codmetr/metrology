namespace ArchiveData.Repo
{
    /// <summary>
    /// Мешанизм формирования очередного индекса
    /// </summary>
    internal class MaxId
    {
        private static int _maxIn = 0;

        /// <summary>
        /// Получить следующий свободный индекс
        /// </summary>
        public static int Next { get { return _maxIn++; } }
    }
}
