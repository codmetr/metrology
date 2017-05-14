using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArchiveData.DTO;
using CheckFrame.Checks;
using SQLiteArchive;

namespace KipTM.Archive
{
    public class DataAccessor:IDataAccessor
    {
        private IObjectiveArchive _archive;
        private TestResult _result;
        CheckConfigData _config;
        private object _customConfig;

        public DataAccessor(TestResult result, CheckConfigData config, object customConfig, IObjectiveArchive archive)
        {
            _result = result;
            _config = config;
            _customConfig = customConfig;
            _archive = archive;
        }

        public void Save()
        {
            if (_result.ResultId == null)
                _result.ResultId = _archive.CreateNewRepair(DateTime.Now);
            _archive.SaveResult(_result.ResultId.Value, _result);
            _archive.SaveParameters(_result.ResultId.Value, _customConfig);
        }
    }
}
