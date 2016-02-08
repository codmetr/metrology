namespace SQLiteArchive
{
    public interface IArchive
    {
        void Save<T>(string key, T entity);
        T Load<T>(string key, T def);
    }
}