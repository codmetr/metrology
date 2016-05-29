using System;
using KipTM.Model.Channels;
using KipTM.Model.TransportChannels;

namespace KipTM.ViewModel.Checks
{
    public interface IMethodViewModel
    {
        void SetConnection(ITransportChannelType connection);

        void SlectUserEthalonChannel();

        void SetEthalonChannel(string ethalonTypeKey, ITransportChannelType settings);

        event EventHandler Started;

        event EventHandler Stoped;
    }
}