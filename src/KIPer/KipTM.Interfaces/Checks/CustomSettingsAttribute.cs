using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KipTM.Interfaces.Checks
{
    /// <summary>
    /// Описывает фабруку по формированию презенторов частных настроек типа ArgumentType
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class CustomSettingsAttribute:Attribute
    {
        public Type ArgumentType { get; private set;}

        public CustomSettingsAttribute(Type argumentType)
        {
            ArgumentType = argumentType;
        }
    }
}
