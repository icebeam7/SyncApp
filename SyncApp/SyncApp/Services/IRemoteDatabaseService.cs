using SyncApp.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SyncApp.Services
{
    public interface IRemoteDatabaseApiService
    {
        Task<List<T>> GetItems<T>(string endpoint, bool active = true) where T : new();
        Task<T> GetItem<T>(string endpoint, int id) where T : new();
        Task<bool> AddItems<T>(string endpoint, List<T> items) where T : BasicTable, new();
        Task<bool> SaveItem<T>(string endpoint, T item) where T : BasicTable, new();
        Task<bool> DeleteItem<T>(string endpoint, int id) where T : BasicTable, new();
        Task<bool> DeleteSoftItem<T>(string endpoint, T item) where T : BasicTable, new();

        Task<bool> ImportItems<T>(string endpoint, string localTable) where T : BasicTable, new();

        Task<bool> SyncItems<T>(string endpoint, string localTable) where T : BasicTable, new();
    }
}
