using Microsoft.Extensions.Caching.Memory;

namespace TroubleTrack.Services
{
    public class AppStateService
    {
        private readonly IMemoryCache _memoryCache;

        public AppStateService(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }

        public void SaveValue<T>(string key, T state)
        {
            _memoryCache.Set(key, state);
        }

        public T GetValue<T>(string key)
        {
            _memoryCache.TryGetValue(key, out T state);
            return state;
        }

        public void RemoveValue(string key)
        {
            _memoryCache.Remove(key);
        }
    }
}