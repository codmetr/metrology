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
        protected IEthalonChannel _ethalonChannel;
        protected IUserChannel _userChannel;


        public ChannelDescriptor Channel {
            get { return _calibChan; }
            set { _calibChan = value; } }

        public IEthalonChannel EthChannel
        {
            get { return _ethalonChannel; }
            set { _ethalonChannel = value; }
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
                if (!_ethalonChannel.Activate(EthalonChannelType))
                    throw new Exception(string.Format("Can not Activate ethalon channel: {0}", _ethalonChannel));
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
            if(_ethalonChannel!=null)
                _ethalonChannel.Stop();
        }

        /// <summary>
        /// Описатель канала подключения к целевому устройству
        /// </summary>
        public ITransportChannelType ChannelType;
        /// <summary>
        /// Описатель канала подключения к эталонному устройству
        /// </summary>
        public ITransportChannelType EthalonChannelType;


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
        /// <param name="ethalonChannel"></param>
        /// <param name="transport"></param>
        public void SetEthalonChannel(IEnumerable<CheckStepConfig> steps, IEthalonChannel ethalonChannel, ITransportChannelType transport)
        {
            _ethalonChannel = ethalonChannel;
            EthalonChannelType = transport;
            foreach (var testStep in steps)
            {
                var step = testStep.Step as ISettedEthalonChannel;
                if (step == null)
                    continue;
                step.SetEthalonChannel(ethalonChannel);
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
