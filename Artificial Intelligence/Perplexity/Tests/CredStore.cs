using Google.Apis.Util.Store;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PerplexityTests
{
    internal class CredStore : IDataStore
    {
        private Dictionary<string, object> _store = new Dictionary<string, object>();
        public Task ClearAsync()
        {
            _store.Clear();
            return Task.CompletedTask;
        }

        public Task DeleteAsync<T>(string key)
        {
            _store.Remove(key);
            return Task.CompletedTask;
        }

        public Task<T> GetAsync<T>(string key)
        {
            object value;
            _store.TryGetValue(key, out value);
            T v = (T)value;
            return Task.FromResult(v);
        }

        public Task StoreAsync<T>(string key, T value)
        {
            _store.Add(key, value);
            return Task.CompletedTask;
        }
    }
}
