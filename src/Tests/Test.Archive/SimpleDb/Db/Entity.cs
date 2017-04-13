using Dapper.Contrib.Extensions;

namespace SimpleDb.Db
{
    public abstract class Entity
    {
        [Write(false)]
	    public bool IsNew
	    {
			get
			{
				return Id == 0;
			}
	    }

        public long Id { get; set; }
    }
}
