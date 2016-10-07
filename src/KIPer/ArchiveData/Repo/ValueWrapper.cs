using System.Linq;

namespace ArchiveData.Repo
{
    public class ValueWrapper
    {
        private readonly TreeEntity _entity;

        public ValueWrapper(TreeEntity entity)
        {
            _entity = entity;
        }

        public string this[string key]
        {
            get
            {
                return _entity[key].Value;
            }
            set
            {
                if (_entity.Childs.Any(el => el.Key == key))
                {
                    _entity[key].Value = value;
                    return;
                }
                _entity[key] = new TreeEntity(){Value = value};
            }
        }
    }
}
