using System;
using System.Collections.Generic;
using KipTM.Model.Checks;

namespace KipTM.Interfaces
{
    public interface IMethodFactory
    {
        Tuple<string, Dictionary<string, ICheckMethod>> GetDefault();
    }
}