namespace SmartSelectors
{
    using Microsoft.Extensions.Caching.Memory;

    internal class SimpleMemoryCache<TItem>
    {
        private readonly MemoryCache _cache = new MemoryCache(new MemoryCacheOptions());

        public TItem TryGet(object key)
        {
            return _cache.TryGetValue(key, out TItem cacheEntry) ? cacheEntry : default;
        }

        public TItem Set(object key, TItem item)
        {
            return _cache.Set(key, item);
        }
    }
}
