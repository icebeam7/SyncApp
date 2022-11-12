using System;
using System.Threading.Tasks;
using System.Collections.Generic;

using SQLite;

using SyncApp.Models;
using SyncApp.Services;

namespace SyncApp.Services
{
    public class LocalDatabaseService : ILocalDatabaseService
    {
        private string dbPath;
        private SQLiteAsyncConnection connection;

        public LocalDatabaseService(string dbPath)
        {
            this.dbPath = dbPath;
        }

        private async Task Init()
        {
            if (connection != null)
                return;

            try
            {
                connection = new SQLiteAsyncConnection(dbPath);

                connection.Tracer = new Action<string>(q =>
                    System.Diagnostics.Debug.WriteLine(q));
                connection.Trace = true;

                await connection.CreateTableAsync<Event>();
            }
            catch (Exception ex)
            {
            }
        }

        public async Task<List<T>> GetItems<T>() where T : new()
        {
            await Init();

            return await connection.Table<T>().ToListAsync();
        }

        public async Task<List<T>> GetActiveItems<T>() where T : BasicTable, new()
        {
            await Init();

            return await connection.Table<T>()
                .Where(x => x.IsActive)
                .ToListAsync();
        }

        public async Task<bool> AddItems<T>(List<T> items) where T : BasicTable, new()
        {
            await Init();

            var op = await connection.InsertAllAsync(items);
            return op == items.Count;
        }

        public async Task<bool> SaveItem<T>(T item) where T : BasicTable, new()
        {
            await Init();
            int op;

            if (item.IdLocal == 0)
            {
                item.IsActive = true;
                op = await connection.InsertAsync(item);
            }
            else
            {
                op = await connection.UpdateAsync(item);
            }

            return op == 1;
        }

        public async Task<bool> DeleteItem<T>(T item) where T : new()
        {
            await Init();

            var op = await connection.DeleteAsync(item);
            return op == 1;
        }

        //Soft Delete
        public async Task<bool> DeleteSoftItem<T>(T item) where T : BasicTable
        {
            await Init();
            item.IsActive = false;

            var op = await connection.UpdateAsync(item);
            return op == 1;
        }

        public async Task<bool> DeleteItemsWithQuery(string query)
        {
            await Init();

            var op = await connection.ExecuteAsync(query);
            return op > 0;
        }
    }
}
