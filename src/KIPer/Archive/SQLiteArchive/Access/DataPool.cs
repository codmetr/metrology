using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ArchiveData;
using ArchiveData.DTO;
using SQLiteArchive.Tree;


namespace SQLiteArchive
{
    /// <summary>
    /// Связывание записей о проверке с DTO
    /// </summary>
    public class DataPool
    {
        private DataSource _dataSource;
        private IDictionaryPool _dictionaryPool;

        /// <summary>
        /// Список всех поддерживаемых типов устройств
        /// </summary>
        public IEnumerable<DeviceTypeDescriptor> DeviceTypes { get { return _dictionaryPool.DeviceTypes; } }
        /// <summary>
        /// Справочник всех результатов проверок по ключу
        /// </summary>
        public IDictionary<TestResultID, object> Repairs { get; internal set; }
        /// <summary>
        /// Справочник всех конфигураций проверок по ключу
        /// </summary>
        public IDictionary<TestResultID, object> Configs { get; internal set; }

        private IDictionary<string, Type> _resTypes;
        private IDictionary<string, Type> _confTypes;

        private DataPool(IDictionaryPool dictionaryPool, IDictionary<string, Type> resTypeDict, IDictionary<string, Type> confTypeDict)
        {
            _dictionaryPool = dictionaryPool;
            _resTypes = resTypeDict;
            _confTypes = confTypeDict;
            Repairs = new Dictionary<TestResultID, object>();
        }

        /// <summary>
        /// Загрузить из БД результаты прошлых проверок
        /// </summary>
        /// <param name="dictionaryPool">справочники</param>
        /// <param name="resTypeDict">список типов реультатов</param>
        /// <returns></returns>
        public static DataPool Load(IDictionaryPool dictionaryPool, IDictionary<string, Type> resTypeDict, IDictionary<string, Type> confTypeDict, Action<string> trace = null)
        {
            var dp = new DataPool(dictionaryPool, resTypeDict, confTypeDict);
            return Load(dp, trace);
        }

        /// <summary>
        /// Загрузить из БД результаты прошлых проверок
        /// </summary>
        /// <param name="target"></param>
        /// <param name="trace"></param>
        /// <returns></returns>
        public static DataPool Load(DataPool target, Action<string> trace = null)
        {
            var dp = target;
            var dbPath = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.CommonDocuments),
                "KipTM\\db", Properties.Settings.Default.DBName);
            dp._dataSource = new DataSource(dbPath, trace);
            dp._dataSource.Load();
            foreach (var repair in dp._dataSource.Checks)
            {
                var res = dp.ResFromDto(repair);
                dp.Repairs.Add(dp.DescriptorFromDto(repair), res);
            }
            return dp;
            //return MakeFakeDataPool();
        }

        /// <summary>
        /// Добавить результат
        /// </summary>
        /// <param name="check"></param>
        /// <param name="res"></param>
        public void AddResult(TestResultID check, object res)
        {
            List<Node> nodes;
            Repairs.Add(check, res);
            var checkDto = Add(check, res, out nodes);
            _dataSource.AddResult(checkDto, nodes);
        }

        /// <summary>
        /// Добавить конфигурацию
        /// </summary>
        /// <param name="check"></param>
        /// <param name="conf"></param>
        public void AddConfig(TestResultID check, object conf)
        {
            List<Node> nodes;
            Configs.Add(check, conf);
            var checkDto = Add(check, conf, out nodes);
            _dataSource.AddConfig(checkDto, nodes);
        }

        /// <summary>
        /// Добавить проверку и
        /// </summary>
        /// <param name="check"></param>
        /// <param name="res"></param>
        /// <param name="nodes"></param>
        /// <returns></returns>
        private CheckDto Add(TestResultID check, object res, out List<Node> nodes)
        {
            var checkDto = DescriptorToDto(check);
            nodes = new List<Node>();
            if (res != null)
            {
                Node resDto = ResToDto(checkDto, res);
                nodes = NodeLiner.ToSetNodes(resDto);
            }
            return checkDto;
        }

        /// <summary>
        /// Обновить описание проверки
        /// </summary>
        /// <param name="check"></param>
        public void UpdateRepair(TestResultID check)
        {
            var repair = GetUpdatedRepair(check);
            _dataSource.Update(repair);
        }

        /// <summary>
        /// Обновить описание проверки и результат
        /// </summary>
        /// <param name="check"></param>
        /// <param name="res"></param>
        public void UpdateResult(TestResultID check, object res)
        {
            var chDto = GetUpdatedRepair(check);
            List<Node> newNodes;
            List<Node> changedNodes;
            List<Node> lessNodes;
            var isTreeNoChanged = AnilizeNodes(chDto, res, _dataSource.NodesConf, out newNodes, out changedNodes, out lessNodes);
            _dataSource.UpdateRes(chDto, newNodes, lessNodes, changedNodes);
        }

        /// <summary>
        /// Обновить описание проверки и конфигурацию
        /// </summary>
        /// <param name="check"></param>
        /// <param name="conf"></param>
        public void UpdateConfig(TestResultID check, object conf)
        {
            var chDto = GetUpdatedRepair(check);
            List<Node> newNodes;
            List<Node> changedNodes;
            List<Node> lessNodes;
            var isTreeNoChanged = AnilizeNodes(chDto, conf, _dataSource.NodesConf, out newNodes, out changedNodes, out lessNodes);
            _dataSource.UpdateConf(chDto, newNodes, lessNodes, changedNodes);
        }

        /// <summary>
        /// Получить обновленную DTO проверки или null
        /// </summary>
        /// <param name="check"></param>
        /// <returns></returns>
        private CheckDto GetUpdatedRepair(TestResultID check)
        {
            var checkDto = _dataSource.Checks.FirstOrDefault(el => el.Id == check.Id);
            if (checkDto == null)
                return null;
            checkDto.CommonResult = check.CommonResult;
            checkDto.CreateTime = check.CreateTime;
            checkDto.DeviceType= check.DeviceType;
            checkDto.SerialNumber = check.SerialNumber;
            checkDto.TargetDeviceKey = check.TargetDeviceKey;
            checkDto.Timestamp = check.Timestamp;
            checkDto.Note = check.Note;
            return checkDto;
        }

        /// <summary>
        /// Проанализировать объект на изменение в древовидном представлении
        /// </summary>
        /// <param name="check"></param>
        /// <param name="res"></param>
        /// <param name="tNodes"></param>
        /// <param name="newNodes"></param>
        /// <param name="changedNodes"></param>
        /// <param name="lessNodes"></param>
        /// <returns></returns>
        private bool AnilizeNodes(CheckDto check, object res, List<Node> tNodes, out List<Node> newNodes, out List<Node> changedNodes, out List<Node> lessNodes)
        {
            newNodes = new List<Node>();
            changedNodes = new List<Node>();
            var nodes = tNodes.Where(el => el.RepairId == check.Id);
            if (!nodes.Any())
            {
                var root = new Node() { RepairId = check.Id, Name = "root" };
                TreeParser.Convert(res, root, new ItemDescriptor(), check.Id);
                newNodes = NodeLiner.ToSetNodes(root);
                lessNodes = new List<Node>();
                return true;
            }
            var tree = NodeLiner.ToTree(nodes);
            lessNodes = NodeLiner.ToSetNodes(tree);
            var isTreeNoChanged = TreeParser.UpdateTree(res, tree, new ItemDescriptor(), (int) check.Id, newNodes, lessNodes,
                changedNodes);
            return isTreeNoChanged;
        }

        #region Конвертаци в DTO и обратно

        /// <summary>
        /// Конвертация результата в DTO
        /// </summary>
        /// <param name="check"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        private Node ResToDto(CheckDto check, object result)
        {
            return TreeParser.Convert(result, new Node() { RepairId = (int)check.Id, Name = "root" },
                new ItemDescriptor(), check.Id);
        }

        /// <summary>
        /// Получить результат по DTO описателя проверки
        /// </summary>
        /// <param name="check"></param>
        /// <returns></returns>
        private object ResFromDto(CheckDto check)
        {
            object res;
            var nodeList = _dataSource.NodesRes.Where(el => el.RepairId == check.Id).ToList();
            if(!_resTypes.ContainsKey(check.DeviceType))
                throw new IndexOutOfRangeException(string.Format("For device \"{0}\" not found result type", check.DeviceType));
            var resTypes = _resTypes[check.DeviceType];
            if (!nodeList.Any())
            {
                if (resTypes == null)
                    return null;
                res = resTypes.Assembly.CreateInstance(resTypes.FullName);
                return res;
            }
            var tree = NodeLiner.ToTree(nodeList);
            if (!TreeParser.TryParse(tree, out res, resTypes, new ItemDescriptor()))
                throw new InvalidCastException(
                    string.Format("Can not parce result for device type {0} from tree to type {1}",
                        check.DeviceType, _resTypes[check.DeviceType]));
            if (res == null)
                res = resTypes.Assembly.CreateInstance(resTypes.FullName);
            return res;
        }

        /// <summary>
        /// Получить конфигурацию по DTO описателя проверки
        /// </summary>
        /// <param name="check"></param>
        /// <returns></returns>
        private object ConfFromDto(CheckDto check)
        {
            object res;
            var nodeList = _dataSource.NodesRes.Where(el => el.RepairId == check.Id).ToList();
            if(!_resTypes.ContainsKey(check.DeviceType))
                throw new IndexOutOfRangeException(string.Format("For device \"{0}\" not found result type", check.DeviceType));
            var resTypes = _resTypes[check.DeviceType];
            if (!nodeList.Any())
            {
                if (resTypes == null)
                    return null;
                res = resTypes.Assembly.CreateInstance(resTypes.FullName);
                return res;
            }
            var tree = NodeLiner.ToTree(nodeList);
            if (!TreeParser.TryParse(tree, out res, resTypes, new ItemDescriptor()))
                throw new InvalidCastException(
                    string.Format("Can not parce result for device type {0} from tree to type {1}",
                        check.DeviceType, _resTypes[check.DeviceType]));
            if (res == null)
                res = resTypes.Assembly.CreateInstance(resTypes.FullName);
            return res;
        }

        /// <summary>
        /// Конвертация DTO в запись о проверке
        /// </summary>
        /// <param name="check"></param>
        /// <returns></returns>
        private TestResultID DescriptorFromDto(CheckDto check)
        {
            var resDevice = new TestResultID()
            {
                Id = (int) check.Id,
                SerialNumber = check.SerialNumber,
                CommonResult = check.CommonResult,
                DeviceType = check.DeviceType,
                TargetDeviceKey = check.TargetDeviceKey,
                CreateTime = check.CreateTime,
                Timestamp = check.Timestamp,
                Note = check.Note
            };
            return resDevice;
        }

        /// <summary>
        /// Конвертация записи о проверке в DTO
        /// </summary>
        /// <param name="repair"></param>
        /// <returns></returns>
        private CheckDto DescriptorToDto(TestResultID repair)
        {
            return new CheckDto()
            {
                Id = repair.Id.Value,
                SerialNumber = repair.SerialNumber,
                DeviceType = repair.DeviceType,
                TargetDeviceKey = repair.TargetDeviceKey,
                CreateTime = repair.CreateTime,
                Timestamp = repair.Timestamp,
                CommonResult = repair.CommonResult,
                Note = repair.Note
            };
        }

        #endregion
    }
}
