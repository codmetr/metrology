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

        public IEnumerable<DeviceTypeDescriptor> DeviceTypes { get { return _dictionaryPool.DeviceTypes; } }
        public IDictionary<TestResultID, object> Repairs { get; internal set; }

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

        private RepairDto GetUpdatedRepair(TestResultID check)
        {
            var repair = _dataSource.Repairs.FirstOrDefault(el => el.Id == check.Id);
            if (repair == null)
                return null;
            repair.AirMark = check.AirMark;
            repair.AirNumber = check.AirNumber;
            repair.AirType.Title = check.AirType.Title;
            repair.CommonResult = check.CommonResult;
            if (repair.Executor.Post == null)
                repair.Executor.Post = new PostDto();
            if (check.Executor != null)
            {
                repair.Executor.Name = check.Executor.Name;
                repair.Executor.Surname = check.Executor.Surname;
                repair.Executor.Patronymic = check.Executor.Patronymic;
                repair.Executor.PersonnelNumber = check.Executor.PersonnelNumber;
                repair.Executor.Note = check.Executor.Note;
                repair.Executor.PhotoPath = ""; //TODO: check.Executor.Photo;
                if (check.Executor.Post == null)
                    repair.Executor.Post.Title = "";
                else
                {
                    repair.Executor.Post.Title = check.Executor.Post.Title;
                    repair.Executor.Post.Id = check.Executor.Post.Id;
                }
            }
            else
            {
                repair.Executor.Name = "";
                repair.Executor.Surname = "";
                repair.Executor.Patronymic = "";
                repair.Executor.PersonnelNumber = "";
                repair.Executor.Note = "";
                repair.Executor.PhotoPath = ""; //TODO: check.Executor.Photo;
                repair.Executor.Post.Title = "";
            }
            repair.Note = check.Note;
            repair.TimeStamp = check.TimeStamp;
            if(repair.FormTO == null)
                repair.FormTO = new FormDto();
            repair.FormTO.Title = check.FormTO.Title;
            repair.MainDevice.DeviceType = check.MainDevice.DeviceType.DeviceType;
            repair.MainDevice.SerialNumber = check.MainDevice.SerialNumber;
            repair.MainDevice.TSN = check.MainDevice.TSN;
            repair.MainDevice.TSO = check.MainDevice.TSO;
            UpdateMetadata(repair.MainDevice, check.MainDevice);
            foreach (var deviceDto in repair.Devices)
            {
                var dev = check.Devices.FirstOrDefault(el => el.DeviceType.DeviceType == deviceDto.DeviceType);
                deviceDto.DeviceType = dev.DeviceType.DeviceType;
                deviceDto.SerialNumber = dev.SerialNumber;
                deviceDto.TSN = dev.TSN;
                deviceDto.TSO = dev.TSO;
                UpdateMetadata(deviceDto, dev);
            }
            return repair;
        }

        private void UpdateMetadata(DeviceDto deviceDto, DeviceInfo dev)
        {
            if (dev.Metadate != null)
            {
                foreach (var key in dev.Metadate.Keys)
                {
                    var md = deviceDto.Metadata.FirstOrDefault(el => el.Key == key);
                    if (md != null)
                        md.Value = dev.Metadate[key];
                    else
                        deviceDto.Metadata.Add(new DeviceMetadataDto()
                        {
                            Key = key,
                            Value = dev.Metadate[key],
                        });
                }
                var lessData = deviceDto.Metadata.Where(el => !dev.Metadate.ContainsKey(el.Key)).ToList();
                foreach (var data in lessData)
                    deviceDto.Metadata.Remove(data);
            }
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
        /// <param name="repair"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        private Node ResToDto(RepairDto repair, object result)
        {
            return TreeParser.Convert(result, new Node() { RepairId = (int)repair.Id, Name = "root" },
                new ItemDescriptor(), repair.Id);
        }

        /// <summary>
        /// Конвертация DTO в результат
        /// </summary>
        /// <param name="repair"></param>
        /// <returns></returns>
        private object ResFromDto(RepairDto repair)
        {
            object res;
            var nodeList = _dataSource.Nodes.Where(el => el.RepairId == repair.Id).ToList();
            if(!_resTypes.ContainsKey(repair.MainDevice.DeviceType))
                throw new IndexOutOfRangeException(string.Format("For device \"{0}\" not found result type", repair.MainDevice.DeviceType));
            var resTypes = _resTypes[repair.MainDevice.DeviceType];
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
                        repair.MainDevice.DeviceType, _resTypes[repair.MainDevice.DeviceType]));
            if (res == null)
                res = resTypes.Assembly.CreateInstance(resTypes.FullName);
            return res;
        }

        /// <summary>
        /// Конвертация DTO в запись о проверке
        /// </summary>
        /// <param name="repair"></param>
        /// <returns></returns>
        private TestResultID DescriptorFromDto(RepairDto repair)
        {
            var mainDeviceType =
                _dictionaryPool.DeviceTypes.FirstOrDefault(el => el.DeviceType == repair.MainDevice.DeviceType);
            var resDevice = new TestResultID() {IsNewCheckType = true};
            resDevice.Id = (int)repair.Id;
            resDevice.MainDevice = new DeviceInfo()
            {
                DeviceType = mainDeviceType,
                SerialNumber = repair.MainDevice.SerialNumber,
                TSN = repair.MainDevice.TSN,
                TSO = repair.MainDevice.TSO,
                Metadate = repair.MainDevice.Metadata.ToDictionary(el=>el.Key, elV=>elV.Value),
            };
            resDevice.Devices = repair.Devices.Select(item =>
            {
                var devType = _dictionaryPool.DeviceTypes.FirstOrDefault(el => item.DeviceType == el.DeviceType);
                return new DeviceInfo()
                {
                    DeviceType = devType,
                    SerialNumber = item.SerialNumber,
                    TSN = item.TSN,
                    TSO = item.TSO,
                    Metadate = repair.MainDevice.Metadata.ToDictionary(el => el.Key, elV => elV.Value),
                };
            });
            resDevice.AirType = _dictionaryPool.AirTypes.FirstOrDefault(el => el.Id == repair.AirType.Id);
            if (resDevice.AirType == null)
                resDevice.AirType = new AirTypeDescriptor(repair.AirType.Title) { Id = (int)repair.AirType.Id };
            resDevice.Executor = _dictionaryPool.Users.FirstOrDefault(el => el.Id == repair.Executor.Id);
            if (resDevice.Executor == null)
                resDevice.Executor = new UserDescriptor(repair.Executor.Name)
                {
                    Id = (int)repair.Executor.Id,
                    Surname = repair.Executor.Surname,
                    Patronymic = repair.Executor.Patronymic,
                    PersonnelNumber = repair.Executor.PersonnelNumber,
                    Note = repair.Executor.Note,
                    Post = null,//rnew PostDescriptor(repair.Executor.Post.Title){Id = repair.Executor.Post.Id},
                    //Photo = null,//repair.Executor.PhotPath
                };
            resDevice.FormTO = _dictionaryPool.Forms.FirstOrDefault(el => el.Id == repair.Id);
            if (resDevice.FormTO == null)
                resDevice.FormTO = new FormToDescriptor(repair.FormTO.Title) { Id = (int)repair.FormTO.Id };
            resDevice.AirMark = repair.AirMark;
            resDevice.AirNumber = repair.AirNumber;
            resDevice.Note = repair.Note;
            resDevice.CommonResult = repair.CommonResult;
            resDevice.TimeStamp = repair.TimeStamp;
            return resDevice;
        }

        /// <summary>
        /// Конвертация записи о проверке в DTO
        /// </summary>
        /// <param name="repair"></param>
        /// <returns></returns>
        private RepairDto DescriptorToDto(TestResultID repair)
        {
            return new RepairDto()
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
                    PhotoPath = "", // TODO GetPath repair.Executor.Photo
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
