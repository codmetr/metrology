using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ArchiveData.DTO;
using ArchiveData.Repo;

namespace ADTSData.RepoParsers
{
    public class TestStepResultRepo : IRepo<TestStepResult>
    {

        /// <summary>
        /// Загрузка результата шага
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public TestStepResult Load(ITreeEntity node)
        {
            return new TestStepResult()
            {
                ChannelKey = node["ChannelKey"].Value,
                CheckKey = node["CheckKey"].Value,
                StepKey = node["StepKey"].Value,
                Result = RepoFabrik.Get<AdtsPointResult>().Load(node["Result"])
            };
        }

        /// <summary>
        /// Сохранени результата шага
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public ITreeEntity Save(TestStepResult entity)
        {
            var result = new TreeEntity();
            result["ChannelKey"] = TreeEntity.Make(result.Id, entity.ChannelKey);
            result["CheckKey"] = TreeEntity.Make(result.Id, entity.CheckKey);
            result["StepKey"] = TreeEntity.Make(result.Id, entity.StepKey);
            result["Result"] = RepoFabrik.Get<AdtsPointResult>().Save(entity.Result as AdtsPointResult);
            return result;
        }

        /// <summary>
        /// Обновить состояние описателя в соответствие с результатом шага
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="node"></param>
        /// <returns></returns>
        public ITreeEntity Update(ITreeEntity node, TestStepResult entity)
        {
            node.Values["ChannelKey"] = entity.ChannelKey;
            node.Values["CheckKey"] = entity.CheckKey;
            node.Values["StepKey"] = entity.StepKey;

            if (node.Childs.Any(el => el.Key == "Result"))
                RepoFabrik.Get<AdtsPointResult>().Update(node["Result"], entity.Result as AdtsPointResult);
            else
                node["Result"] = RepoFabrik.Get<AdtsPointResult>().Save(entity.Result as AdtsPointResult);
            return node;
        }

    }
}
