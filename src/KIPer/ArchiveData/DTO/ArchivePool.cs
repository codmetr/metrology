using System;
using System.Collections.Generic;

namespace ArchiveData.DTO
{
    public class ResultsArchive
    {
        public ResultsArchive()
        {
            Results = new List<TestResult>();
        }

        public bool Load()
        {
            throw new NotImplementedException();
        }

        public List<TestResult> Results { get; private set; }
    }
}
