using System;
using System.IO;
using SyncApp.Services;

namespace SyncApp.Helpers
{
    public static class Singletons
    {
        private static ILocalDatabaseService localDatabaseService;

        public static ILocalDatabaseService LocalDatabaseService
        {
            get
            {
                if (localDatabaseService == null)
                {
                    var dbPath = Path.Combine(
                        Environment.GetFolderPath(
                            Environment.SpecialFolder.LocalApplicationData),
                        Constants.LocalDbFileName);

                    localDatabaseService = new LocalDatabaseService(dbPath);
                }

                return localDatabaseService;
            }
        }

        private static IRemoteDatabaseApiService remoteDatabaseApiService;

        public static IRemoteDatabaseApiService RemoteDatabaseApiService
        {
            get
            {
                if (remoteDatabaseApiService == null)
                    remoteDatabaseApiService = new RemoteDatabaseApiService();

                return remoteDatabaseApiService;
            }
        }
    }
}
