using ArchiveData.DTO;
using CheckFrame.Checks;
using CheckFrame.Model.Channels;
using CheckFrame.ViewModel.Checks.Channels;
using KipTM.Archive;
using KipTM.Checks;
using KipTM.Model;
using KipTM.Model.Channels;
using KipTM.Model.Checks;
using KipTM.Model.TransportChannels;

namespace KipTM.ViewModel.Checks
{
    public interface ICheckFabrik
    {

        /// <summary>
        /// Получить презентор типа проверки
        /// </summary>
        /// <returns></returns>
        IMethodViewModel GetViewModelFor();
        
    }
}