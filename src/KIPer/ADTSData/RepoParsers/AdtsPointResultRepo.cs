using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using SQLiteArchive.Repo;

namespace ADTSData.RepoParsers
{
    public class AdtsPointResultRepo : IRepo<AdtsPointResult>
    {
        /// <summary>
        /// загрузка результата точки
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public AdtsPointResult Load(ITreeEntity node)
        {
            return new AdtsPointResult()
            {
                Point = double.Parse(node["Point"].Value, CultureInfo.InvariantCulture),
                Tolerance = double.Parse(node["Tolerance"].Value, CultureInfo.InvariantCulture),
                RealValue = double.Parse(node["RealValue"].Value, CultureInfo.InvariantCulture),
                Error = double.Parse(node["Error"].Value, CultureInfo.InvariantCulture),
                IsCorrect = bool.Parse(node["IsCorrect"].Value)
            };
        }

        /// <summary>
        /// Сохранени результата проверки конкретной точки
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public ITreeEntity Save(AdtsPointResult entity)
        {
            var result = new TreeEntity();
            result["Point"] = TreeEntity.Make(result.Id, entity.Point.ToString(CultureInfo.InvariantCulture));
            result["Tolerance"] = TreeEntity.Make(result.Id, entity.Tolerance.ToString(CultureInfo.InvariantCulture));
            result["RealValue"] = TreeEntity.Make(result.Id, entity.RealValue.ToString(CultureInfo.InvariantCulture));
            result["Error"] = TreeEntity.Make(result.Id, entity.Error.ToString(CultureInfo.InvariantCulture));
            result["IsCorrect"] = TreeEntity.Make(result.Id, entity.IsCorrect.ToString(CultureInfo.InvariantCulture));
            return result;
        }

        /// <summary>
        /// Обновление результата проверки конкретной точки
        /// </summary>
        /// <param name="node"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        public ITreeEntity Update(ITreeEntity node, AdtsPointResult entity)
        {
            node.Values["Point"] = entity.Point.ToString(CultureInfo.InvariantCulture);
            node.Values["Tolerance"] = entity.Tolerance.ToString(CultureInfo.InvariantCulture);
            node.Values["RealValue"] = entity.RealValue.ToString(CultureInfo.InvariantCulture);
            node.Values["Error"] = entity.Error.ToString(CultureInfo.InvariantCulture);
            node.Values["IsCorrect"] = entity.IsCorrect.ToString(CultureInfo.InvariantCulture);
            return node;
        }
    }
}
