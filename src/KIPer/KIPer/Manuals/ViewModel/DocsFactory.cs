using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KipTM.Manuals.ViewModel
{
    class DocsFactory : IDocsFactory
    {
        public IEnumerable<BookViewModel> Load()
        {
            var books = new List<BookViewModel>();
            var basePath = Path.Combine(Path.GetFullPath(Environment.CurrentDirectory), "Manuals");
            var path = Path.Combine(basePath, @"DPI\DPI620_Genii_Help_KRU0541_ru.pdf");
            books.Add(new BookViewModel() { Title = "DPI620Genii Руководство пользователя(RU)", Path = path });
            path = Path.Combine(basePath, @"PACE\pace5000_pace6000_user_manual_k0443_ru.pdf");
            books.Add(new BookViewModel() { Title = "PACE Руководство пользователя(RU)", Path = path });
            path = Path.Combine(basePath, @"PACE\pace5000_pace6000_user_manual_k0443_en.pdf");
            books.Add(new BookViewModel() { Title = "PACE Руководство пользователя(EN)", Path = path });
            return books;
        }
    }
}
