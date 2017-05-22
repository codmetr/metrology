using System.Collections.Generic;

namespace KipTM.Manuals.ViewModel
{
    public interface IDocsFactory
    {
        IEnumerable<BookViewModel> Load();
    }
}