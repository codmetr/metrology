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

        private DataPool(IDictionaryPool dictionaryPool, IDictionary<string, Type> resTypeDict)
        {
            _dictionaryPool = dictionaryPool;
            _resTypes = resTypeDict;
            Repairs = new Dictionary<TestResultID, object>();
        }

        /// <summary>
        /// Загрузить из БД результаты прошлых проверок
        /// </summary>
        /// <param name="dictionaryPool">справочники</param>
        /// <param name="resTypeDict">список типов реультатов</param>
        /// <returns></returns>
        public static DataPool Load(IDictionaryPool dictionaryPool, IDictionary<string, Type> resTypeDict, Action<string> trace = null)
        {
            var dp = new DataPool(dictionaryPool, resTypeDict);
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
            foreach (var repair in dp._dataSource.Repairs)
            {
                var res = dp.ResFromDto(repair);
                dp.Repairs.Add(dp.DescriptorFromDto(repair), res);
            }
            return dp;
            //return MakeFakeDataPool();
        }

        /// <summary>
        /// Добавить 
        /// </summary>
        /// <param name="check"></param>
        /// <param name="res"></param>
        public void AddRepair(TestResultID check, object res)
        {
            Repairs.Add(check, res);
            var checkDto = DescriptorToDto(check);
            var nodes = new List<Node>();
            if (res != null)
            {
                Node resDto = ResToDto(checkDto, res);
                nodes = NodeLiner.ToSetNodes(resDto);
            }
            _dataSource.Add(checkDto, nodes);
        }

        public void UpdateRepair(TestResultID check)
        {
            var repair = GetUpdatedRepair(check);
            _dataSource.Update(repair);
        }

        public void UpdateResult(TestResultID check, object res)
        {
            List<Node> newNodes;
            List<Node> changedNodes;
            List<Node> lessNodes;
            var isTreeNoChanged = AnilizeNodes(check, res, out newNodes, out changedNodes, out lessNodes);
            _dataSource.Update(newNodes, lessNodes, changedNodes);
        }

        public void UpdateConfig(TestResultID check, object res)
        {
            List<Node> newNodes;
            List<Node> changedNodes;
            List<Node> lessNodes;
            var isTreeNoChanged = AnilizeNodes(check, res, out newNodes, out changedNodes, out lessNodes);
            _dataSource.Update(newNodes, lessNodes, changedNodes);
        }

        private CheckDto GetUpdatedRepair(TestResultID check)
        {
            var checkDto = _dataSource.Repairs.FirstOrDefault(el => el.Id == check.Id);
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

        private bool AnilizeNodes(TestResultID check, object res, out List<Node> newNodes, out List<Node> changedNodes, out List<Node> lessNodes)
        {
            newNodes = new List<Node>();
            changedNodes = new List<Node>();
            var nodes = _dataSource.Nodes.Where(el => el.RepairId == check.Id);
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
            var isTreeNoChanged = TreeParser.UpdateTree(res, tree, new ItemDescriptor(), check.Id, newNodes, lessNodes,
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
        /// Конвертация DTO в результат
        /// </summary>
        /// <param name="check"></param>
        /// <returns></returns>
        private object ResFromDto(CheckDto check)
        {
            object res;
            var nodeList = _dataSource.Nodes.Where(el => el.RepairId == check.Id).ToList();
            if(!_resTypes.ContainsKey(check.MainDevice.DeviceType))
                throw new IndexOutOfRangeException(string.Format("For device \"{0}\" not found result type", check.MainDevice.DeviceType));
            var resTypes = _resTypes[check.MainDevice.DeviceType];
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
                        check.MainDevice.DeviceType, _resTypes[check.MainDevice.DeviceType]));
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
            var mainDeviceType =
                _dictionaryPool.DeviceTypes.FirstOrDefault(el => el.DeviceType == check.MainDevice.DeviceType);
            var resDevice = new TestResultID() {IsNewCheckType = true};
            resDevice.Id = (int)check.Id;
            resDevice.MainDevice = new DeviceInfo()
            {
                DeviceType = mainDeviceType,
                SerialNumber = check.MainDevice.SerialNumber,
                TSN = check.MainDevice.TSN,
                TSO = check.MainDevice.TSO,
                Metadate = check.MainDevice.Metadata.ToDictionary(el=>el.Key, elV=>elV.Value),
            };
            resDevice.Devices = check.Devices.Select(item =>
            {
                var devType = _dictionaryPool.DeviceTypes.FirstOrDefault(el => item.DeviceType == el.DeviceType);
                return new DeviceInfo()
                {
                    DeviceType = devType,
                    SerialNumber = item.SerialNumber,
                    TSN = item.TSN,
                    TSO = item.TSO,
                    Metadate = check.MainDevice.Metadata.ToDictionary(el => el.Key, elV => elV.Value),
                };
            });
            resDevice.AirType = _dictionaryPool.AirTypes.FirstOrDefault(el => el.Id == check.AirType.Id);
            if (resDevice.AirType == null)
                resDevice.AirType = new AirTypeDescriptor(check.AirType.Title) { Id = (int)check.AirType.Id };
            resDevice.Executor = _dictionaryPool.Users.FirstOrDefault(el => el.Id == check.Executor.Id);
            if (resDevice.Executor == null)
                resDevice.Executor = new UserDescriptor(check.Executor.Name)
                {
                    Id = (int)check.Executor.Id,
                    Surname = check.Executor.Surname,
                    Patronymic = check.Executor.Patronymic,
                    PersonnelNumber = check.Executor.PersonnelNumber,
                    Note = check.Executor.Note,
                    Post = null,//rnew PostDescriptor(check.Executor.Post.Title){Id = check.Executor.Post.Id},
                    //Photo = null,//check.Executor.PhotPath
                };
            resDevice.FormTO = _dictionaryPool.Forms.FirstOrDefault(el => el.Id == check.Id);
            if (resDevice.FormTO == null)
                resDevice.FormTO = new FormToDescriptor(check.FormTO.Title) { Id = (int)check.FormTO.Id };
            resDevice.AirMark = check.AirMark;
            resDevice.AirNumber = check.AirNumber;
            resDevice.Note = check.Note;
            resDevice.CommonResult = check.CommonResult;
            resDevice.TimeStamp = check.TimeStamp;
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
                Id = repair.Id,
                MainDevice = new DeviceDto()
                {
                    DeviceType = repair.MainDevice.DeviceType.DeviceType,
                    SerialNumber = repair.MainDevice.SerialNumber,
                    TSN = repair.MainDevice.TSN,
                    TSO = repair.MainDevice.TSO,
                    Metadata = repair.MainDevice.Metadate.Select(el=>new DeviceMetadataDto() {Key = el.Key,Value = el.Value}).ToList()
                },
                Devices = repair.Devices.Select(item => new DeviceDto()
                {
                    DeviceType = item.DeviceType.DeviceType,
                    SerialNumber = item.SerialNumber,
                    TSN = item.TSN,
                    TSO = item.TSO,
                    Metadata = item.Metadate.Select(el => new DeviceMetadataDto() { Key = el.Key, Value = el.Value }).ToList()
                }).ToList(),
                AirType = new AirTypeDto()
                {
                    RepairId = repair.Id,
                    Id = repair.AirType.Id,
                    Title = repair.AirType.Title,
                },
                Executor = new UserDto()
                {
                    RepairId = repair.Id,
                    Id = repair.Executor.Id,
                    Name = repair.Executor.Name,
                    Surname = repair.Executor.Surname,
                    Patronymic = repair.Executor.Patronymic,
                    PersonnelNumber = repair.Executor.PersonnelNumber,
                    PhotoPath = "", // TODO GetPath check.Executor.Photo
                    Note = repair.Executor.Note,
                    Post = new PostDto()
                    {
                        RepairId = repair.Id,
                        Id = repair.Executor.Post.Id,
                        Title = repair.Executor.Post.Title,
                    }
                },
                FormTO = new FormDto()
                {
                    RepairId = repair.Id,
                    Id = repair.FormTO.Id,
                    Title = repair.FormTO.Title,
                },
                AirMark = repair.AirMark,
                AirNumber = repair.AirNumber,
                Note = repair.Note,
                CommonResult = repair.CommonResult,
                TimeStamp = repair.TimeStamp,
            };
        }

        #endregion
    }
}
