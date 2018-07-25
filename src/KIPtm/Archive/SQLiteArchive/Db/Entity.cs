using Dapper.Contrib.Extensions;

namespace SQLiteArchive.Db
{
    public abstract class Entity
    {
        private bool _isNew = true;

        [Write(false)]
        public bool IsNew
        {
            get { return _isNew; }
            set { _isNew = value; }
        }

        /// <summary>
        /// Идентификатор
        /// </summary>
        public long Id { get; set; }
    }
}
