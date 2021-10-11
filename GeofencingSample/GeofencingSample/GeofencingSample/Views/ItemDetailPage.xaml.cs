using GeofencingSample.ViewModels;
using System.ComponentModel;
using Xamarin.Forms;

namespace GeofencingSample.Views
{
    public partial class ItemDetailPage : ContentPage
    {
        public ItemDetailPage()
        {
            InitializeComponent();
            BindingContext = new ItemDetailViewModel();
        }
    }
}