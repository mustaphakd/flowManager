using System;
using System.Reflection;
using System.Threading.Tasks;
using Worosoft.Xamarin.HttpClientExtensions.Attributes;

namespace Worosoft.Xamarin.HttpClientExtensions.Extensions
{
    public static class ApiDefinitionExtension
    {

        public static ApiDefinitionAttribute GetAttribute<T>()
        {
            return typeof(T).GetTypeInfo().GetCustomAttribute<ApiDefinitionAttribute>();
        }

        public static Refit.HttpMethodAttribute GetHttpMethodAttribute<T>()
        {
            return typeof(T).GetTypeInfo().GetCustomAttribute<Refit.HttpMethodAttribute>();
        }
        public static string GetEndpointPath<T>()
        {
            var attribute = GetHttpMethodAttribute<T>();

            return attribute == null ? string.Empty : attribute.Path;
        }

        public static int GetRetryCount<T>()
        {
            var attribute = GetAttribute<T>();

            return attribute == null ? 0 : attribute.RetryCount;
        }

        public static Type GetHttpMessageHandler<T>()
        {
            return GetAttribute<T>()?.HttpMessageHandler;
        }

        public static Func<Task<string>> GetRemoteUrlProvider<T>()
        {
            return GetAttribute<T>()?.RemoteUrlProvider;
        }

        public static TimeSpan GetTimeout<T>()
        {
            var attribute = GetAttribute<T>();
            return attribute == null ? TimeSpan.FromSeconds(50) : attribute.Timeout;
        }
    }
}