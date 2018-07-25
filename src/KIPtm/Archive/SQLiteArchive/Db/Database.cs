namespace SQLiteArchive.Db
{
    public class Database : IDatabase
    {
        private readonly IDbContext _context;

        public Database(IDbContext context)
        {
            _context = context; 
        }

        public T Query<T>(IQuery<T> query)
        {
            var result = query.Execute(_context);
            return result;
        }

        public void Execute(ICommand command)
        {
            command.Execute(_context);
        }
    }
}
