namespace SQLiteArchive.Db
{
    public interface IQuery<out T>
    {
        T Execute(IDbContext context);
    }
}
