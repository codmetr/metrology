using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using ADTSData.Keys;
using ArchiveData.DTO;
using SQLiteArchive.Repo;

namespace ADTSData.RepoParsers
{
    public class TestResultRepo : IRepo<TestResult>
    {
        /// <summary>
        /// Загрузка резльтата из дерева репозитория
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public TestResult Load(ITreeEntity node)
        {
            var result = new TestResult();
            result.CheckType = node["CheckType"].Value;
            result.Timestamp = DateTime.Parse(node["Timestamp"].Value, CultureInfo.InvariantCulture);
            result.TargetDeviceKey = node["TargetDeviceKey"].Value;
            result.User = node["User"].Value;
            result.Note = node["Note"].Value;
            result.AtmospherePressure = node["AtmospherePressure"].Value;
            result.Temperature = node["Temperature"].Value;
            result.Humidity = node["Humidity"].Value;
            result.Client = node["Client"].Value;
            result.Channel = node["Channel"].Value;
            result.TargetDevice = RepoFabrik.Get<DeviceDescriptor>().Load(node["TargetDevice"]);
            result.Etalon = node["Etalon"].Childs.Select(RepoFabrik.Get<DeviceDescriptor>().Load).ToList();
            result.Results = node["Results"].Childs.Select(RepoFabrik.Get<TestStepResult>().Load).ToList();

            return result;

        }

        /// <summary>
        /// Сохранение результата в дерева репозитория
        /// </summary>
        /// <param name="root"></param>
        /// <param name="entity"></param>
        public ITreeEntity Save(TestResult entity)
        {
            ITreeEntity root = new TreeEntity();
            root["CheckType"] = new TreeEntity(root.Id) { Value = entity.CheckType };
            root["Timestamp"] = new TreeEntity(root.Id) { Value = entity.Timestamp.ToString(CultureInfo.InvariantCulture) };
            root["TargetDeviceKey"] = new TreeEntity(root.Id) { Value = entity.TargetDeviceKey };
            root["User"] = new TreeEntity(root.Id) { Value = entity.User };
            root["Note"] = new TreeEntity(root.Id) { Value = entity.Note };
            root["AtmospherePressure"] = new TreeEntity(root.Id) { Value = entity.AtmospherePressure };
            root["Temperature"] = new TreeEntity(root.Id) { Value = entity.Temperature };
            root["Humidity"] = new TreeEntity(root.Id) { Value = entity.Humidity };
            root["Client"] = new TreeEntity(root.Id) { Value = entity.Client };
            root["Channel"] = new TreeEntity(root.Id) { Value = entity.Channel };
            root["TargetDevice"] = RepoFabrik.Get<DeviceDescriptor>().Save(entity.TargetDevice);
            root["Etalon"] =
                new TreeEntity(root.Id).AddRange(
                    entity.Etalon.Select(el => RepoFabrik.Get<DeviceDescriptor>().Save(el).SetKey(el.GetKey())));
            root["Results"] =
                new TreeEntity(root.Id).AddRange(
                    entity.Results.Select(el => RepoFabrik.Get<TestStepResult>().Save(el).SetKey(el.GetKey())));
            root.Key = entity.GetKey();
            return root;
        }

        /// <summary>
        /// Обновление состояния дерева репозитория
        /// </summary>
        /// <param name="node"></param>
        /// <param name="entity"></param>
        public ITreeEntity Update(ITreeEntity node, TestResult entity)
        {
            node.Values["CheckType"] = entity.CheckType;
            node.Values["Timestamp"] = entity.Timestamp.ToString(CultureInfo.InvariantCulture);
            node.Values["TargetDeviceKey"] = entity.TargetDeviceKey;
            node.Values["User"] = entity.User;
            node.Values["Note"] = entity.Note;
            node.Values["AtmospherePressure"] = entity.AtmospherePressure;
            node.Values["Temperature"] = entity.Temperature;
            node.Values["Humidity"] = entity.Humidity;
            node.Values["Client"] = entity.Client;
            node.Values["Channel"] = entity.Channel;
            if (node.Childs.Any(el => el.Key == "TargetDevice"))
                RepoFabrik.Get<DeviceDescriptor>().Update(node["TargetDevice"], entity.TargetDevice);
            else
                node["TargetDevice"] = RepoFabrik.Get<DeviceDescriptor>().Save(entity.TargetDevice);

            if (node.Childs.Any(el => el.Key == "Etalon"))
            {
                var forDelete = node["Etalon"].Childs.ToList();
                foreach (var deviceDescriptor in entity.Etalon)
                {
                    if (forDelete.All(el => el.Key != deviceDescriptor.GetKey()))
                        continue;
                    var item = RepoFabrik.Get<DeviceDescriptor>()
                        .Update(node["Etalon"][deviceDescriptor.GetKey()], deviceDescriptor);
                    forDelete.Remove(item);
                }
                node["Etalon"].RemoveRange(forDelete);
            }
            else
                node["Etalon"] =
                    new TreeEntity(node.Id).AddRange(
                        entity.Etalon.Select(el => RepoFabrik.Get<DeviceDescriptor>().Save(el).SetKey(el.GetKey())));

            if (node.Childs.Any(el => el.Key == "Results"))
            {
                var forDelete = node["Results"].Childs.ToList();
                foreach (var res in entity.Results)
                {
                    if (forDelete.All(el => el.Key != res.GetKey()))
                        continue;
                    var item = RepoFabrik.Get<TestStepResult>().Update(node["Etalon"][res.GetKey()], res);
                    forDelete.Remove(item);
                }
                node["Results"].RemoveRange(forDelete);
            }
            else
                node["Results"] =
                    new TreeEntity(node.Id).AddRange(
                        entity.Results.Select(el => RepoFabrik.Get<TestStepResult>().Save(el).SetKey(el.GetKey())));

            return node;
        }

    }
}
