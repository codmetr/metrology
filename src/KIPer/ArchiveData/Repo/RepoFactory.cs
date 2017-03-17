using System;
using System.Collections.Generic;

namespace ArchiveData.Repo
{
    /// <summary>
    /// Локатор по работе с репозиторием
    /// </summary>
    public class RepoFactory
    {
        private static readonly Dictionary<Type, object> RepoDic = new Dictionary<Type, object>();

        /// <summary>
        /// Зарегистрировать преобразователь типа <see cref="T"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="repo"></param>
        public static void Registrate<T>(IRepo<T> repo)
        {
            if (RepoDic.ContainsKey(typeof (T)))
                RepoDic[typeof (T)] = repo;
            else
                RepoDic.Add(typeof (T), repo);
        }

        /// <summary>
        /// Получить преобразователь типа <see cref="T"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static IRepo<T> Get<T>()
        {
            return RepoDic[typeof (T)] as IRepo<T>;
        }
    }
}
