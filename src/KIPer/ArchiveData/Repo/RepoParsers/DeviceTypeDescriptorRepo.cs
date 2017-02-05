using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ADTSData.Keys;
using ArchiveData.DTO;
using ArchiveData.Repo;

namespace ADTSData.RepoParsers
{
    public class DeviceTypeDescriptorRepo : IRepo<DeviceTypeDescriptor>
    {
        /// <summary>
        /// Загрузка описателя типа устройства
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public DeviceTypeDescriptor Load(ITreeEntity node)
        {
            return new DeviceTypeDescriptor()
            {
                Model = node["Model"].Value,
                DeviceCommonType = node["DeviceCommonType"].Value,
                DeviceManufacturer = node["DeviceManufacturer"].Value
            };
        }

        /// <summary>
        /// Сохранени Описателя типа устройства
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public ITreeEntity Save(DeviceTypeDescriptor entity)
        {
            var result = new TreeEntity();
            result["Model"] = TreeEntity.Make(result.Id, entity.Model);
            result["DeviceCommonType"] = TreeEntity.Make(result.Id, entity.DeviceCommonType);
            result["DeviceManufacturer"] = TreeEntity.Make(result.Id, entity.DeviceManufacturer);
            result.Key = entity.GetKey();
            return result;
        }

        /// <summary>
        /// Обновить состояние описателя в соответствие с описателем типа устройства
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="node"></param>
        /// <returns></returns>
        public ITreeEntity Update(ITreeEntity node, DeviceTypeDescriptor entity)
        {
            node.Values["Model"] = entity.Model;
            node.Values["DeviceCommonType"] = entity.DeviceCommonType;
            node.Values["DeviceManufacturer"] = entity.DeviceManufacturer;
            return node;
        }
    }
}
