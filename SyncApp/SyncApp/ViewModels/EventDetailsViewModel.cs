using System;
using System.Threading.Tasks;

using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.ComponentModel;

using SyncApp.Models;

namespace SyncApp.ViewModels
{
    public partial class EventDetailsViewModel : BaseViewModel
    {
        [ObservableProperty]
        Event item;

        public EventDetailsViewModel(Event item)
        {
            this.item = item;
        }

        [RelayCommand]
        private async Task SaveItemAsync()
        {
            if (IsBusy)
                return;

            try
            {
                IsBusy = true;

                var op = await App.LocalDb.SaveItem(item);

                if (op)
                    await App.Current.MainPage.DisplayAlert("Success!", "Data successfully saved!", "OK");
                else
                    await App.Current.MainPage.DisplayAlert("Error!", "Review the data", "OK");

                await App.Current.MainPage.Navigation.PopAsync();
            }
            catch (Exception ex)
            {
                await App.Current.MainPage.DisplayAlert("Error!", ex.Message, "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

        public bool IsNewItem => item.IdLocal == 0;
        public bool IsNotNewItem => item.IdLocal > 0;

        [RelayCommand]
        private async Task DeleteItemAsync()
        {
            var confirm = await App.Current.MainPage
                .DisplayAlert("Confirm operation", "Do you really want to DELETE this item?", "Yes", "No");

            if (confirm)
            {
                IsBusy = true;

                await App.LocalDb.DeleteSoftItem(item);
                await App.Current.MainPage.DisplayAlert("Yay", "Item deleted!", "OK");
                await App.Current.MainPage.Navigation.PopAsync();

                IsBusy = false;
            }
        }
    }
}
