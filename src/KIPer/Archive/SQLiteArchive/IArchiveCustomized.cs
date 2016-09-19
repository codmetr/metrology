namespace SQLiteArchive
{
    public interface IArchiveCustomized<T>
    {
        void Save(string key, T entity);
        T Load(string key, T def);
    }
}