using KipTM.Model.Channels;

namespace KipTM.ViewModel.Checks
{
    public interface IMethodViewModel
    {
        void SlectUserEthalonChannel();

        void SetEthalonChannel(IEthalonChannel ethalon);

    }
}