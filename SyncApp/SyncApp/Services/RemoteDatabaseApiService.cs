using System;
using System.Text;
using System.Net.Http;
using System.Text.Json;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Net.Http.Headers;
using System.Collections.Generic;

using SyncApp;
using SyncApp.Models;
using SyncApp.Helpers;
using System.Linq;

namespace SyncApp.Services
{
    public class RemoteDatabaseApiService : IRemoteDatabaseApiService
    {
        HttpClient client;

        JsonSerializerOptions options = new JsonSerializerOptions()
        {
            PropertyNameCaseInsensitive = true,
        };

        public RemoteDatabaseApiService()
        {
            client = new HttpClient();
            client.BaseAddress = new Uri(Constants.RemoteApiUrl);
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public async Task<List<T>> GetItems<T>(string endpoint, bool active = true) where T : new()
        {
            if (!active)
                endpoint += "/All";

            var response = await client.GetAsync(endpoint);

            if (response.IsSuccessStatusCode)
                return await response.Content.ReadFromJsonAsync<List<T>>(options);

            return default;
        }

        public async Task<T> GetItem<T>(string endpoint, int id) where T : new()
        {
            endpoint += $"/{id}";

            var response = await client.GetAsync(endpoint);

            if (response.IsSuccessStatusCode)
                return await response.Content.ReadFromJsonAsync<T>(options);

            return default;
        }

        public async Task<bool> AddItems<T>(string endpoint, List<T> items) where T : BasicTable, new()
        {
            var body = JsonSerializer.Serialize(items);
            var content = new StringContent(body, Encoding.UTF8, "application/json");

            var response = await client.PostAsync(endpoint, content);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> UpdateItems<T>(string endpoint, List<T> items) where T : BasicTable, new()
        {
            var body = JsonSerializer.Serialize(items);
            var content = new StringContent(body, Encoding.UTF8, "application/json");

            var response = await client.PutAsync(endpoint, content);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteItems<T>(string endpoint, List<int> items) where T : BasicTable, new()
        {
            var body = JsonSerializer.Serialize(items);
            var content = new StringContent(body, Encoding.UTF8, "application/json");

            var message = new HttpRequestMessage(HttpMethod.Delete, endpoint);
            message.Content = content;

            var response = await client.SendAsync(message);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> SaveItem<T>(string endpoint, T item) where T : BasicTable, new()
        {
            var body = JsonSerializer.Serialize(item);
            var content = new StringContent(body, Encoding.UTF8, "application/json");

            var response = (item.IdServer > 0)
                ? await client.PostAsync(endpoint, content)
                : await client.PutAsync(endpoint, content);

            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteItem<T>(string endpoint, int id) where T : BasicTable, new()
        {
            endpoint += $"/{id}";

            var response = await client.DeleteAsync(endpoint);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteSoftItem<T>(string endpoint, T item) where T : BasicTable, new()
        {
            endpoint += $"/SoftDelete/{item.IdLocal}";

            var body = JsonSerializer.Serialize(item);
            var content = new StringContent(body, Encoding.UTF8, "application/json");

            var response = await client.PutAsync(endpoint, content);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> ImportItems<T>(string endpoint, string localTable) where T : BasicTable, new()
        {
            // Step 1. Get remote items
            var items = await GetItems<T>(endpoint);

            // Step 2. Clear local items
            var query = $"DELETE FROM {localTable}";
            await App.LocalDb.DeleteItemsWithQuery(query);

            // Step 3. Add remote items to empty table
            foreach (var item in items)
                item.IdLocal = 0;

            var op = await App.LocalDb.AddItems<T>(items);
            return op;
        }

        public async Task<bool> SyncItems<T>(string endpoint, string localTable) where T : BasicTable, new()
        {
            // Step 1. Get local items
            var localItems = await App.LocalDb.GetItems<T>();

            // Step 2. Get remote items
            var remoteItems = await GetItems<T>(endpoint);

            // Step 3. Compare idServer for CUD
            var itemsToInsert = new List<T>();
            var itemsToDelete = new List<T>();
            var itemsToUpdate = new List<T>();

            foreach (var localItem in localItems)
            {
                if (localItem.IdServer == 0)
                    itemsToInsert.Add(localItem);

                var remoteItem = remoteItems.FirstOrDefault(x => x.IdServer == localItem.IdServer);

                if (remoteItem != null)
                    itemsToUpdate.Add(localItem);
                else
                    itemsToDelete.Add(localItem);
            }

            var idsToDelete = itemsToDelete.Select(x => x.IdServer).ToList();

            // Step 4. Perform bulk CUD 
            var op = await AddItems<T>(endpoint + "/All", itemsToInsert);
            op &= await UpdateItems<T>(endpoint + "/All", itemsToUpdate);
            op &= await DeleteItems<T>(endpoint + "/All", idsToDelete);

            return op;
        }
    }
}
