using System.Threading.Tasks;
using System.Collections.ObjectModel;

using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.ComponentModel;

using SyncApp.Models;
using SyncApp.Helpers;
using System;
using Xamarin.Forms;

namespace SyncApp.ViewModels
{
    public partial class EventListViewModel : BaseViewModel
    {
        [ObservableProperty]
        Event selectedItem;

        INavigation navigation;

        public ObservableCollection<Event> Items { get; } 
            = new ObservableCollection<Event>();

        public EventListViewModel()
        {
            Title = "Event List";
        }

        [RelayCommand]
        private async Task GetItemsAsync()
        {
            if (IsBusy)
                return;

            try
            {
                IsBusy = true;

                var items = await App.LocalDb.GetActiveItems<Event>();

                if (Items.Count != 0)
                    Items.Clear();

                foreach (var item in items)
                    Items.Add(item);
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

        [RelayCommand]
        private async Task AddNewAsync()
        {
            await NavigateToDetailsAsync(new Event());
        }

        [RelayCommand]
        private async Task GoToDetailsAsync()
        {
            if (selectedItem != null)
                await NavigateToDetailsAsync(selectedItem);
        }

        private async Task NavigateToDetailsAsync(Event item)
        {
            var vm = new EventDetailsViewModel(item);

            var page = new Views.EventDetailsView();
            page.BindingContext = vm;

            await App.Current.MainPage.Navigation.PushAsync(page);
        }

        [RelayCommand]
        private async Task ImportDataAsync()
        {
            try
            {
                var op = await App.RemoteApi.ImportItems<Event>(
                    Constants.EndpointEvents,
                    Constants.TableEvents);

                if (op)
                {
                    await App.Current.MainPage.DisplayAlert("Success!", "Table successfully imported", "OK");
                    await GetItemsAsync();
                }
                else
                    await App.Current.MainPage.DisplayAlert("Error!", "There was an error", "OK");
            }
            catch (Exception ex)
            {
                await App.Current.MainPage.DisplayAlert("Error!", ex.Message, "OK");
            }
        }

        [RelayCommand]
        private async Task SyncDataAsync()
        {
            try
            {
                var op = await App.RemoteApi.SyncItems<Event>(
                    Constants.EndpointEvents,
                    Constants.TableEvents);

                if (op)
                {
                    await App.Current.MainPage.DisplayAlert("Success!", "Table successfully synchronized", "OK");
                    await GetItemsAsync();
                }
                else
                    await App.Current.MainPage.DisplayAlert("Error!", "There was an error", "OK");
            }
            catch (Exception ex)
            {
                await App.Current.MainPage.DisplayAlert("Error!", ex.Message, "OK");
            }

        }
    }
}