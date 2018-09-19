using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KipTM.Interfaces;
using KipTM.Interfaces.Channels;
using PACEChecks.Channels.ViewModel;

namespace PACEChecks.Channels
{
    /// <summary>
    /// Фабрика конфигурации PACE для эталонного канала-источника
    /// </summary>
    public class PaceEtalonSourceFactory: IEtalonSourceCannelFactory<Units>, IEtalonSourceCannelFactory
    {
        private readonly PaceConfigViewModel _configViewModel;
        private const string Name = "PACE 6000";

        public PaceEtalonSourceFactory()
        {
            _configViewModel = new PaceConfigViewModel();
        }

        /// <summary>
        /// Название типа эталона
        /// </summary>
        public string TypeName { get { return Name; } }

        /// <summary>
        /// Измерительный канал-источник
        /// </summary>
        /// <returns></returns>
        public IEtalonSourceChannel<Units> GetChanel()
        {
            //TODO реализовать зависимость от модели
            return new PaceEtalonSource(_configViewModel.Address);
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
        public object ConfigViewModel
        {
            get { return _configViewModel; }
        }
    }
}
