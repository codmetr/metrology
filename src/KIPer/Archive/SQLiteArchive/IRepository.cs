namespace SQLiteArchive
{
    interface IRepository
    {
        string GetByKey(string key);
        string SetByKey(string key, string value);
    }
}
