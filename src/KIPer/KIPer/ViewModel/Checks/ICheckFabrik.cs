using KipTM.Archive;
using KipTM.Model;
using KipTM.Model.Checks;

namespace KipTM.ViewModel.Checks
{
    public interface ICheckFabrik
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        IMethodViewModel GetViewModelFor(CheckConfig checkConfig);
    }
}