namespace SimpleDb.Db
{
    public interface ICommand
    {
        void Execute(IDbContext context);
    }
}
