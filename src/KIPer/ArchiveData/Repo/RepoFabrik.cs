using System;
using System.Collections.Generic;

namespace ArchiveData.Repo
{
    public class RepoFabrik
    {
        private static readonly Dictionary<Type, object> RepoDic = new Dictionary<Type, object>();

        public static void Registrate<T>(IRepo<T> repo)
        {
            if (RepoDic.ContainsKey(typeof (T)))
                RepoDic[typeof (T)] = repo;
            else
                RepoDic.Add(typeof (T), repo);
        }

        public static IRepo<T> Get<T>()
        {
            return RepoDic[typeof (T)] as IRepo<T>;
        }
    }
}
