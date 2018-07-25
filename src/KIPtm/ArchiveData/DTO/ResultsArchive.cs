using System;
using System.Collections.Generic;

namespace ArchiveData.DTO
{
    public class ResultsArchive
    {
        /// <summary>
        /// Репозиторий результатов проверки
        /// </summary>
        public ResultsArchive()
        {
            Results = new List<TestResultID>();
        }

        public bool Load()
        {
            throw new NotImplementedException();
        }

        public bool Save()
        {
            throw new NotImplementedException();
        }

        public void AddResult(TestResultID result)
        {
            Results.Add(result);
        }

        public List<TestResultID> Results { get; private set; }
    }
}
