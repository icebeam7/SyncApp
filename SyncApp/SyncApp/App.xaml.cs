using SyncApp.Helpers;
using SyncApp.Services;
using SyncApp.Views;
using Xamarin.Forms;

namespace SyncApp
{
    public partial class App : Application
    {
        public static ILocalDatabaseService LocalDb = 
            Singletons.LocalDatabaseService;

        public static IRemoteDatabaseApiService RemoteApi =
            Singletons.RemoteDatabaseApiService;

        public App()
        {
            InitializeComponent();

            MainPage = new NavigationPage(new EventListView());
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
