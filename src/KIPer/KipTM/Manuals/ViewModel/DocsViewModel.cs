using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace KipTM.Manuals.ViewModel
{
    public class DocsViewModel:INotifyPropertyChanged
    {
        private ObservableCollection<BookViewModel> _books;
        private BookViewModel _selectedBook;

        public DocsViewModel(IDocsFactory docsFactory)
        {
            _books = new ObservableCollection<BookViewModel>(docsFactory.Load());
            SelectedBook = _books.FirstOrDefault();
        }

        /// <summary>
        /// Список книг
        /// </summary>
        public ObservableCollection<BookViewModel> Books
        {
            get { return _books; }
        }

        public BookViewModel SelectedBook
        {
            get { return _selectedBook; }
            set
            {
                _selectedBook = value; 
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
