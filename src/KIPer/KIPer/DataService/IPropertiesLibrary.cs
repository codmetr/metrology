using KipTM.Archive;

namespace KipTM.Model
{
    public interface IPropertiesLibrary
    {
        /// <summary>
        /// Список настроек хода проверок
        /// </summary>
        DataPool PropertyPool { get; }

        /// <summary>
        /// Список словарей (имена пользователей, организации и пр.)
        /// </summary>
        DictionariesPool DictionariesPool { get; }
    }
}