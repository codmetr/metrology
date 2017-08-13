using System;
using System.Diagnostics;

namespace ArchiveData.DTO
{
    /// <summary>
    /// Описатель измерительного канала
    /// </summary>
    [DebuggerDisplay("{Name}: [{Max}, {Min}], {Error})")]
    public class ChannelDescriptor
    {
        private Func<string> _getLocalizedName;
        private string _name;

        /// <summary>
        /// Описатель измерительного канала
        /// </summary>
        public ChannelDescriptor()
        {
            _name = string.Empty;
            _getLocalizedName = () => _name;
        }

        ///// <summary>
        ///// Описатель измерительного канала
        ///// </summary>
        ///// <param name="getLocalizedName">Механизм получения локализованного имени</param>
        //public ChannelDescriptor(Func<string> getLocalizedName)
        //    :this()
        //{
        //    // todo: придумать механизм устаноки способа локализации после загрузки из БД (десериализаци с помощью IRepo<>)
        //    _getLocalizedName = getLocalizedName;
        //}

        /// <summary>
        /// Название канала
        /// </summary>
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        /// <summary>
        /// Направленность
        /// </summary>
        public ChannelOrder Order { get; set; }

        /// <summary>
        /// Тип измеряемой величины
        /// </summary>
        public ChannelType TypeChannel { get; set; }

        /// <summary>
        /// Минимум
        /// </summary>
        public double Max { get; set; }

        /// <summary>
        /// Максимум
        /// </summary>
        public double Min { get; set; }

        /// <summary>
        /// Допустимая погрешность
        /// </summary>
        public double Error { get; set; }

        /// <summary>
        /// Механизм получения локализованного имени
        /// </summary>
        public ChannelDescriptor SetLocalizedNameFunc(Func<string> getLocalizedName)
        {
            _getLocalizedName = getLocalizedName;
            return this;
        }

        /// <summary>
        /// Получение локализованного имени
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            var name = Name;
            try
            {
                name = _getLocalizedName();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
                name = Name;
            }
            return name;
        }
    }
}
