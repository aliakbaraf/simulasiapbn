/*
 * Simulasi APBN
 *
 * Program ditulis oleh Danang Galuh Tegar Prasetyo (https://danang.id/)
 * untuk Kementerian Keuangan Republik Indonesia.
 */
using System;
using Microsoft.Extensions.Caching.Memory;
using SimulasiAPBN.Web.Models;

namespace SimulasiAPBN.Web.Common.Extensions
{
    public static class MemoryCacheExtension
    {
        public static string GetAppTitle(this IMemoryCache memoryCache)
        {
            return memoryCache.Get<string>(CacheKeys.AppTitle);
        }

        public static bool GetIsAppInstalled(this IMemoryCache memoryCache)
        {
            return memoryCache.Get<bool>(CacheKeys.IsAppInstalled);
        }

        public static string SetAppTitle(this IMemoryCache memoryCache, string appTitle)
        {
            var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromMinutes(60));

            return memoryCache.Set(CacheKeys.AppTitle, appTitle, cacheEntryOptions);
        }

        public static bool SetIsAppInstalled(this IMemoryCache memoryCache, bool isAppInstalled)
        {
            var cacheEntryOptions = new MemoryCacheEntryOptions()
                .SetSlidingExpiration(isAppInstalled
                ? TimeSpan.FromMinutes(60)
                : TimeSpan.FromSeconds(5));

            return memoryCache.Set(CacheKeys.AppTitle, isAppInstalled, cacheEntryOptions);
        }

        public static bool TryGetAppTitle(this IMemoryCache memoryCache, out string appTitle)
        {
            var found = memoryCache.TryGetValue(CacheKeys.AppTitle, out string value);
            appTitle = value;
            return found;
        }

        public static bool TryGetIsAppInstalled(this IMemoryCache memoryCache, out bool isAppInstalled)
        {
            var found = memoryCache.TryGetValue(CacheKeys.IsAppInstalled, out bool value);
            isAppInstalled = value;
            return found;
        }
    }
}
