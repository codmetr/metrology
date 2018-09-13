using System;
using System.Collections.Generic;
using System.Diagnostics;
using ArchiveData.DTO;
using CheckFrame.Model.Checks.Steps;
using KipTM.EventAggregator;
using KipTM.Interfaces.Channels;
using KipTM.Model.Channels;
using KipTM.Model.Checks;
using KipTM.Model.TransportChannels;
using KipTM.ViewModel.Events;

namespace CheckFrame.Checks
{
    public class ChannelsConfig
    {
        private IEventAggregator _agregator;

        protected ChannelDescriptor _calibChan;
        protected IEtalonChannel EtalonChannel;
        protected IUserChannel _userChannel;


        public ChannelDescriptor Channel {
            get { return _calibChan; }
            set { _calibChan = value; } }

        public IEtalonChannel EthChannel
        {
            get { return EtalonChannel; }
            set { EtalonChannel = value; }
        }

        public IUserChannel UsrChannel
        {
            get { return _userChannel; }
            set { _userChannel = value; }
        }

        /// <summary>
        /// Ключ канала
        /// </summary>
        public string ChannelKey
        {
            get { return _calibChan.Name; }
        }

        public void Activate()
        {
            try
            {
                if (!EtalonChannel.Activate(EtalonChannelType))
                    throw new Exception(string.Format("Can not Activate etalon channel: {0}", EtalonChannel));
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
                if (_agregator != null)
                    _agregator.Post(new ErrorMessageEventArg("Не удалось подключить эталонный канал"));
                throw;
            }
        }

        public void Stop()
        {
            if(EtalonChannel!=null)
                EtalonChannel.Stop();
        }

        /// <summary>
        /// Описатель канала подключения к целевому устройству
        /// </summary>
        public ITransportChannelType ChannelType;
        /// <summary>
        /// Описатель канала подключения к эталонному устройству
        /// </summary>
        public ITransportChannelType EtalonChannelType;


        /// <summary>
        /// Задать агрегатор событий
        /// </summary>
        /// <param name="agregator">агрегатор событий</param>
        public void SetAggregator(IEventAggregator agregator)
        {
            _agregator = agregator;
        }

        /// <summary>
        /// Задать канал эталона
        /// </summary>
        /// <param name="etalonChannel"></param>
        /// <param name="transport"></param>
        public void SetEtalonChannel(IEnumerable<CheckStepConfig> steps, IEtalonChannel etalonChannel, ITransportChannelType transport)
        {
            EtalonChannel = etalonChannel;
            EtalonChannelType = transport;
            foreach (var testStep in steps)
            {
                var step = testStep.Step as ISettedEtalonChannel;
                if (step == null)
                    continue;
                step.SetEtalonChannel(etalonChannel);
            }
        }

        /// <summary>
        /// Задать канал связи с пользователем
        /// </summary>
        /// <param name="userChannel"></param>
        public void SetUserChannel(IEnumerable<CheckStepConfig> steps, IUserChannel userChannel)
        {
            _userChannel = userChannel;
            foreach (var testStep in steps)
            {
                var step = testStep.Step as ISettedUserChannel;
                if (step == null)
                    continue;
                step.SetUserChannel(userChannel);
            }
        }
    }
}
