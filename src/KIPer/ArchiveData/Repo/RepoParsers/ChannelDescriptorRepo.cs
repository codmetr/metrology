using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using ADTSData.Keys;
using ArchiveData.DTO;
using ArchiveData.Repo;

namespace ADTSData.RepoParsers
{
    public class ChannelDescriptorRepo : IRepo<ChannelDescriptor>
    {
        /// <summary>
        /// Загрузка резльтата из дерева репозитория
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public ChannelDescriptor Load(ITreeEntity node)
        {
            var result = new ChannelDescriptor();
            result.Name = node["Name"].Value;
            result.TypeChannel = (ChannelType)Enum.Parse(typeof(ChannelType), node["TypeChannel"].Value);
            result.Order = (ChannelOrder)Enum.Parse(typeof(ChannelOrder), node["Order"].Value);
            result.Min = double.Parse(node["Min"].Value, NumberStyles.Any, CultureInfo.InvariantCulture);
            result.Max = double.Parse(node["Max"].Value, NumberStyles.Any, CultureInfo.InvariantCulture);
            result.Error = double.Parse(node["Error"].Value, NumberStyles.Any, CultureInfo.InvariantCulture);
            return result;
        }

        /// <summary>
        /// Сохранение результата в дерева репозитория
        /// </summary>
        /// <param name="root"></param>
        /// <param name="entity"></param>
        public ITreeEntity Save(ChannelDescriptor entity)
        {
            ITreeEntity root = new TreeEntity();
            root["Name"] = new TreeEntity(root.Id) { Value = entity.Name };
            root["TypeChannel"] = new TreeEntity(root.Id) { Value = entity.TypeChannel.ToString()};
            root["Order"] = new TreeEntity(root.Id) { Value = entity.Order.ToString() };
            root["Min"] = new TreeEntity(root.Id) { Value = entity.Min.ToString(CultureInfo.InvariantCulture) };
            root["Max"] = new TreeEntity(root.Id) { Value = entity.Max.ToString(CultureInfo.InvariantCulture) };
            root["Error"] = new TreeEntity(root.Id) { Value = entity.Error.ToString(CultureInfo.InvariantCulture) };
            return root;
        }

        /// <summary>
        /// Обновление состояния дерева репозитория
        /// </summary>
        /// <param name="node"></param>
        /// <param name="entity"></param>
        public ITreeEntity Update(ITreeEntity node, ChannelDescriptor entity)
        {
            node.Values["Name"] = entity.Name;
            node.Values["TypeChannel"] = entity.TypeChannel.ToString();
            node.Values["Order"] = entity.Order.ToString();
            node.Values["Min"] = entity.Min.ToString(CultureInfo.InvariantCulture);
            node.Values["Max"] = entity.Max.ToString(CultureInfo.InvariantCulture);
            node.Values["Error"] = entity.Error.ToString(CultureInfo.InvariantCulture);
            return node;
        }

    }
}
