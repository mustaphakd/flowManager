using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Linq;

namespace Worosoft.Xamarin.HttpClientExtensions.Extensions
{
    public static class HandlerExtension
    {
        private static Dictionary<int, Object> HoistCache = new Dictionary<int, object>();


        public static void SetClient(this HttpMessageHandler handler, int httpClientHash)
        {
            HoistCache.Add(httpClientHash, handler);
        }

        public static object GetHoistHandler(this HttpClient client)
        {
            if(HoistCache.TryGetValue(client.GetHashCode(), out Object val))
            {
                return val;
            }

            return null;
        }

        public static void RemoveClient(this HttpMessageHandler handler)
        {
            var found = HoistCache.FirstOrDefault(val => Object.ReferenceEquals(val.Value, handler));

            if(found.Value == null)
            {
                return;
            }

            HoistCache.Remove(found.Key);
        }
    }
}
