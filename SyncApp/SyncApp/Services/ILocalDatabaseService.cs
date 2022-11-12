using SyncApp.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SyncApp.Services
{
    public interface ILocalDatabaseService
    {
        Task<List<T>> GetItems<T>() where T : new();
        Task<List<T>> GetActiveItems<T>() where T : BasicTable, new();
        Task<bool> SaveItem<T>(T item) where T : BasicTable, new();
        Task<bool> DeleteItem<T>(T item) where T : new();
        Task<bool> DeleteSoftItem<T>(T item) where T : BasicTable;

        Task<bool> AddItems<T>(List<T> items) where T : BasicTable, new();
        Task<bool> DeleteItemsWithQuery(string query);
    }
}