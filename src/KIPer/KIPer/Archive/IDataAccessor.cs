using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArchiveData.DTO;

namespace KipTM.Archive
{
    public interface IDataAccessor
    {
        void Save(TestResult result);
    }
}
