namespace ArchiveData.Repo
{
    /// <summary>
    /// Преобразователь типа <see cref="T"/>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IRepo<T>
    {
        /// <summary>
        /// Загрузить сущности типа <see cref="T"/> из <paramref name="node"/>
        /// </summary>
        /// <param name="node">Корень для загрузки сущности</param>
        /// <returns>Загруженая сущность</returns>
        T Load(ITreeEntity node);

        /// <summary>
        /// Преобразовать сущности типа <see cref="T"/> в <see cref="ITreeEntity"/>
        /// </summary>
        /// <param name="entity">Объект преобразования</param>
        /// <returns></returns>
        ITreeEntity Save(T entity);

        /// <summary>
        /// Обновить значение сущности в узле
        /// </summary>
        /// <param name="node">Целевой узел</param>
        /// <param name="entity">Объект</param>
        /// <returns>Заполнненый узел</returns>
        ITreeEntity Update(ITreeEntity node, T entity);
    }
}