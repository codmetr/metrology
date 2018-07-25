using System.Collections.Generic;
using System.Linq;
using ArchiveData.DTO;
using KipTM.Interfaces;
using KipTM.Interfaces.Checks;

namespace KipTM.Model
{
    public class MethodsService : IMethodsService
    {
        private readonly Dictionary<DeviceTypeDescriptor, Dictionary<string, ICheckMethod>> _methods;
        private readonly Dictionary<DeviceTypeDescriptor, IMethodFactory> _factories;

        public MethodsService(IEnumerable<IMethodFactory> factories )
        {
            _methods = factories.ToDictionary(el => el.GetKey(), el => el.GetMethods());
            _factories = factories.ToDictionary(el => el.GetKey(), el => el);
        }

        /// <summary>
        /// Набор поддерживаемых методик для конкретного типа устройств
        /// </summary>
        public IDictionary<string, ICheckMethod> MethodsForType(DeviceTypeDescriptor deviceKey)
        {
            return _methods[deviceKey];
        }

        /// <summary>
        /// Получить описатели устройств
        /// </summary>
        /// <returns></returns>
        public IEnumerable<DeviceViewDescriptor> GetDescriptors()
        {
            return _methods.Keys.Select(key =>
            {
                var factory = _factories[key];
                return new DeviceViewDescriptor()
                {
                    Key = key,
                    Title = factory.GetName(),
                    SmallImg = factory.GetSmallImage(),
                    BigImg = factory.GetBigImage()
                };
            });
        }
    }
}
