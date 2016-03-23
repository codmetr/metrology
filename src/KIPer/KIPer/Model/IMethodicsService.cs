using System.Collections.Generic;
using KipTM.Model.Checks;

namespace KipTM.Model
{
    public interface IMethodicsService
    {
        /// <summary>
        /// Набор поддерживаемых методик
        /// </summary>
        IDictionary<string, ICheckMethodic> Methodics { get; }
    }
}