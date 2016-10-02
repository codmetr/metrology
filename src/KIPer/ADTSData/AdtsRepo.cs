using System.Globalization;
using ArchiveData.DTO;
using SQLiteArchive.Repo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ADTSData.Keys;

namespace ADTSData
{
    public class AdtsRepo
    {
        /// <summary>
        /// Загрузка резльтата
        /// </summary>
        /// <param name="root"></param>
        /// <returns></returns>
        public TestResult Load(ITreeEntity root)
        {
            var result= new TestResult();
            result.CheckType = root["CheckType"].Value;
            result.Timestamp = DateTime.Parse(root["Timestamp"].Value, CultureInfo.InvariantCulture);
            result.TargetDeviceKey = root["TargetDeviceKey"].Value;
            result.User = root["User"].Value;
            result.Note = root["Note"].Value;
            result.AtmospherePressure = root["AtmospherePressure"].Value;
            result.Temperature = root["Temperature"].Value;
            result.Humidity = root["Humidity"].Value;
            result.Client = root["Client"].Value;
            result.Channel = root["Channel"].Value;
            result.TargetDevice = LoadDeviceDescriptor(root["TargetDevice"]);
            result.Etalon = root["Etalon"].Childs.Select(LoadDeviceDescriptor).ToList();
            result.Results = root["Results"].Childs.Select(LoadTestStepResult).ToList();

            return result;

        }

        /// <summary>
        /// Сохранение результата
        /// </summary>
        /// <param name="root"></param>
        /// <param name="result"></param>
        public void Save(ITreeEntity root, TestResult result)
        {
            root["CheckType"] = new TreeEntity(root.Id) { Value = result.CheckType };
            root["Timestamp"] = new TreeEntity(root.Id) { Value = result.Timestamp.ToString(CultureInfo.InvariantCulture) };
            root["TargetDeviceKey"] = new TreeEntity(root.Id) { Value = result.TargetDeviceKey };
            root["User"] = new TreeEntity(root.Id) { Value = result.User };
            root["Note"] = new TreeEntity(root.Id) { Value = result.Note };
            root["AtmospherePressure"] = new TreeEntity(root.Id) { Value = result.AtmospherePressure };
            root["Temperature"] = new TreeEntity(root.Id) { Value = result.Temperature };
            root["Humidity"] = new TreeEntity(root.Id) { Value = result.Humidity };
            root["Client"] = new TreeEntity(root.Id) { Value = result.Client };
            root["Channel"] = new TreeEntity(root.Id) { Value = result.Channel };
            root["TargetDevice"] =  Save(result.TargetDevice);
            root["Etalon"] = new TreeEntity(root.Id).AddRange(result.Etalon.Select(el => Save(el).SetKey(el.GetKey())));
            root["Results"] = new TreeEntity(root.Id).AddRange(result.Results.Select(el => Save(el).SetKey(el.GetKey())));
            root.Key = result.GetKey();
        }

        public void Update(ITreeEntity root, TestResult result)
        {
            root.Values["CheckType"] = result.CheckType;
            root.Values["Timestamp"] = result.Timestamp.ToString(CultureInfo.InvariantCulture);
            root.Values["TargetDeviceKey"] = result.TargetDeviceKey;
            root.Values["User"] = result.User;
            root.Values["Note"] = result.Note;
            root.Values["AtmospherePressure"] = result.AtmospherePressure;
            root.Values["Temperature"] = result.Temperature;
            root.Values["Humidity"] = result.Humidity;
            root.Values["Client"] = result.Client;
            root.Values["Channel"] = result.Channel;
            if (root.Childs.Any(el => el.Key == "TargetDevice"))
            {
                
            }
            else
                root["TargetDevice"] = Save(result.TargetDevice);
        }

        #region DeviceDescriptor

        /// <summary>
        /// Загрузка Описателя устройства
        /// </summary>
        /// <param name="root"></param>
        /// <returns></returns>
        private DeviceDescriptor LoadDeviceDescriptor(ITreeEntity root)
        {
            return new DeviceDescriptor()
            {
                DeviceType = LoadDeviceTypeDescriptor(root["DeviceType"]),
                PreviousCheckTime = DateTime.Parse(root["PreviousCheckTime"].Value),
                SerialNumber = root["SerialNumber"].Value,
            };
        }

        /// <summary>
        /// Сохранени Описателя устройства
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private TreeEntity Save(DeviceDescriptor obj)
        {
            var result = new TreeEntity();
            result["PreviousCheckTime"] = TreeEntity.Make(result.Id, obj.PreviousCheckTime.ToString());
            result["SerialNumber"] = TreeEntity.Make(result.Id, obj.SerialNumber);
            result["DeviceType"] = Save(obj.DeviceType);
            result.Key = obj.GetKey();
            return result;
        }

        private TreeEntity Update(DeviceDescriptor obj, TreeEntity entity)
        {


            return entity;
        }
        #endregion

        #region DeviceTypeDescriptor

        /// <summary>
        /// Загрузка описателя типа устройства
        /// </summary>
        /// <param name="root"></param>
        /// <returns></returns>
        private DeviceTypeDescriptor LoadDeviceTypeDescriptor(ITreeEntity root)
        {
            return new DeviceTypeDescriptor()
            {
                Model = root["Model"].Value,
                DeviceCommonType = root["DeviceCommonType"].Value,
                DeviceManufacturer = root["DeviceManufacturer"].Value
            };
        }

        /// <summary>
        /// Сохранени Описателя типа устройства
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private TreeEntity Save(DeviceTypeDescriptor obj)
        {
            var result = new TreeEntity();
            result["Model"] = TreeEntity.Make(result.Id, obj.Model);
            result["DeviceCommonType"] = TreeEntity.Make(result.Id, obj.DeviceCommonType);
            result["DeviceManufacturer"] = TreeEntity.Make(result.Id, obj.DeviceManufacturer);
            result.Key = obj.GetKey();
            return result;
        }

        #endregion

        #region TestStepResult

        /// <summary>
        /// Загрузка результата шага
        /// </summary>
        /// <param name="root"></param>
        /// <returns></returns>
        private TestStepResult LoadTestStepResult(ITreeEntity root)
        {
            return new TestStepResult()
            {
                ChannelKey = root["ChannelKey"].Value,
                CheckKey = root["CheckKey"].Value,
                StepKey = root["StepKey"].Value,
                Result = LoadAdtsPointResult(root["Result"])
            };
        }

        /// <summary>
        /// Сохранени Описателя типа устройства
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private TreeEntity Save(TestStepResult obj)
        {
            var result = new TreeEntity();
            result["ChannelKey"] = TreeEntity.Make(result.Id, obj.ChannelKey);
            result["CheckKey"] = TreeEntity.Make(result.Id, obj.CheckKey);
            result["StepKey"] = TreeEntity.Make(result.Id, obj.StepKey);
            result["Result"] = Save(obj.Result as AdtsPointResult);
            return result;
        }

        #endregion

        #region AdtsPointResult

        /// <summary>
        /// загрузка результата точки
        /// </summary>
        /// <param name="root"></param>
        /// <returns></returns>
        private AdtsPointResult LoadAdtsPointResult(ITreeEntity root)
        {
            return new AdtsPointResult()
            {
                Point = double.Parse(root["Point"].Value, CultureInfo.InvariantCulture),
                Tolerance = double.Parse(root["Tolerance"].Value, CultureInfo.InvariantCulture),
                RealValue = double.Parse(root["RealValue"].Value, CultureInfo.InvariantCulture),
                Error = double.Parse(root["Error"].Value, CultureInfo.InvariantCulture),
                IsCorrect = bool.Parse(root["IsCorrect"].Value)
            };
        }

        /// <summary>
        /// Сохранени результата проверки конкретной точки
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private TreeEntity Save(AdtsPointResult obj)
        {
            var result = new TreeEntity();
            result["Point"] = TreeEntity.Make(result.Id, obj.Point.ToString(CultureInfo.InvariantCulture));
            result["Tolerance"] = TreeEntity.Make(result.Id, obj.Tolerance.ToString(CultureInfo.InvariantCulture));
            result["RealValue"] = TreeEntity.Make(result.Id, obj.RealValue.ToString(CultureInfo.InvariantCulture));
            result["Error"] = TreeEntity.Make(result.Id, obj.Error.ToString(CultureInfo.InvariantCulture));
            result["IsCorrect"] = TreeEntity.Make(result.Id, obj.IsCorrect.ToString(CultureInfo.InvariantCulture));
            return result;
        }

        #endregion
    }
}
