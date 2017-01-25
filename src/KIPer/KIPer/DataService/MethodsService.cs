using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using KipTM.Interfaces;
using KipTM.Model.Checks;

namespace KipTM.Model
{
    public class MethodsService : IMethodsService
    {
        private readonly Dictionary<string, Dictionary<string, ICheckMethod>> _methods;
        private readonly Dictionary<string, IMethodFactory> _factories;
        private readonly Dictionary<string, Bitmap> _largeImmages;
        private readonly Dictionary<string, Bitmap> _smallImmages;
        private readonly Dictionary<string, string> _names;

        public MethodsService(IEnumerable<IMethodFactory> factories )
        {
            _methods = factories.ToDictionary(el => el.GetKey(), el => el.GetMethods());
            _factories = factories.ToDictionary(el => el.GetKey(), el => el);
        }

        /// <summary>
        /// Набор поддерживаемых методик для конкретного типа устройств
        /// </summary>
        public IDictionary<string, ICheckMethod> MethodsForType(string deviceKey)
        {
            return _methods[deviceKey];
        }

        /// <summary>
        /// Получить набор ключей
        /// </summary>
        /// <returns></returns>
        public IEnumerable<string> GetKeys()
        {
            return _methods.Keys;
        }

        /// <summary>
        /// Получить заголовок
        /// </summary>
        /// <param name="key">ключ</param>
        /// <returns></returns>
        public string GetTitle(string key)
        {
            return _factories[key].GetName();
        }

        /// <summary>
        /// Получить заголовок
        /// </summary>
        /// <param name="key">ключ</param>
        /// <returns></returns>
        public Bitmap GetBigImage(string key)
        {
            return _factories[key].GetBigImage();
        }

        /// <summary>
        /// Получить заголовок
        /// </summary>
        /// <param name="key">ключ</param>
        /// <returns></returns>
        public Bitmap GetSmallImage(string key)
        {
            return _factories[key].GetSmallImage();
        }
    }
}
