using DMTP.lib.Caching;

using Microsoft.Extensions.Caching.Memory;

using Exception = System.Exception;

namespace DMTP.REST.InterfaceImplementations
{
    public class InMemoryCaching : ICaching
    {
        private readonly IMemoryCache _cache;

        private readonly NLog.Logger _log = NLog.LogManager.GetCurrentClassLogger();

        public InMemoryCaching()
        {
            var cacheOptions = new MemoryCacheOptions()
            {
                SizeLimit = 4096
            };

            _cache = new MemoryCache(cacheOptions);
        }

        public (T Object, bool NotFound) Get<T>(string key) => _cache.TryGetValue(key, out var cachedObject) ? ((T)cachedObject, false) : (default, true);

        public bool InsertOrUpdate<T>(string key, T objectValue)
        {
            try
            {
                _cache.Set(key, objectValue);

                return true;
            }
            catch (Exception ex)
            {
                _log.Error(ex, $"Failed adding {key} to the cache");

                return false;
            }
        }
    }
}