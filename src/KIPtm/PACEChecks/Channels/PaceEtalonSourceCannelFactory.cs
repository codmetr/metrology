using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KipTM.Interfaces.Channels;
using PACEChecks.Channels.ViewModel;

namespace PACEChecks.Channels
{
    /// <summary>
    /// Фабрика конфигурации PACE для эталонного канала-источника
    /// </summary>
    public class PaceEtalonSourceCannelFactory: IEtalonSourceCannelFactory<double>, IEtalonSourceCannelFactory
    {
        private const string Name = "PACE5000/6000";

        public PaceEtalonSourceCannelFactory()
        {
            ConfigViewModel = new PaseConfigViewModel();
        }

        /// <summary>
        /// Название типа эталона
        /// </summary>
        public string TypeName { get { return Name; } }

        /// <summary>
        /// Измерительный канал-источник
        /// </summary>
        /// <returns></returns>
        public IEtalonSourceChannel<double> GetChanel()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Измерительный канал-источник
        /// </summary>
        /// <returns></returns>
        IEtalonSourceChannel IEtalonSourceCannelFactory.GetChanel()
        {
            return GetChanel();
        }

        /// <summary>
        /// ViewModel конфигурирования измерительного канала
        /// </summary>
        public object ConfigViewModel { get; }
    }
}
