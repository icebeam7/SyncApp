using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using SyncApp.ViewModels;

namespace SyncApp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class EventListView : ContentPage
    {
        public EventListView()
        {
            InitializeComponent();
        }

        protected async override void OnAppearing()
        {
            base.OnAppearing();

            var vm = (EventListViewModel)this.BindingContext;
            await vm.GetItemsCommand.ExecuteAsync(null);

            vm.SelectedItem = null;
        }
    }
}