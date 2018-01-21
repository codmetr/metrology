using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Graphic
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class UserControl1 : UserControl
    {
        private INotifyCollectionChanged _collection;

        public UserControl1()
        {
            InitializeComponent();
        }

        private void UserControl1_OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var newCollection = e.NewValue as INotifyCollectionChanged;
            if (newCollection == null)
                return;
            if (_collection != null)
                _collection.CollectionChanged -= _collection_CollectionChanged;
            _collection = newCollection;
            _collection.CollectionChanged += _collection_CollectionChanged;
        }

        private void _collection_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}
