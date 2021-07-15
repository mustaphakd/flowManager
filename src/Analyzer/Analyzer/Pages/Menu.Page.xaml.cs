using Analyzer.Core;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Analyzer.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Menu : ContentPage
    {
        private ObservableCollection<PageItem> pageItems;
        public Menu()
        {
            InitializeComponent();
            pageItems = new ObservableCollection<PageItem>();
            listView.ItemsSource = pageItems;
        }

        public void AddPageItem(PageItem pageItem)
        {
            pageItems.Add(pageItem);
        }
    }
}