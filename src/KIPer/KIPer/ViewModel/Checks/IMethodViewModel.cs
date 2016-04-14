using System;
using KipTM.Model.Channels;

namespace KipTM.ViewModel.Checks
{
    public interface IMethodViewModel
    {
        void SlectUserEthalonChannel();

        void SetEthalonChannel(string ethalonTypeKey, object settings);

        event EventHandler Started;

        event EventHandler Stoped;
    }
}