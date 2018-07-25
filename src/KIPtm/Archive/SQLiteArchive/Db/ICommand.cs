namespace SQLiteArchive.Db
{
    public interface ICommand
    {
        void Execute(IDbContext context);
    }
}
