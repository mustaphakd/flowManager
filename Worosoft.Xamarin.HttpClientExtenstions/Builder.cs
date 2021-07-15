using Refit;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Reflection;
using System.Text.RegularExpressions;
using Worosoft.Xamarin.HttpClientExtensions.Extensions;
using Worosoft.Xamarin.HttpClientExtensions.Handlers;
using Xamarin.Forms;

namespace Worosoft.Xamarin.HttpClientExtensions
{
    public class Builder
    {

        private static readonly IDictionary<TypeInfo, object> apiEndpoints = new Dictionary<TypeInfo, object>();
        public static T1 Build<T1>(RefitSettings refitSettings)
        {
            var typeInfo = typeof(T1).GetTypeInfo();
            if(apiEndpoints.ContainsKey(typeInfo))
            {
                return (T1)apiEndpoints[typeInfo];
            }

            var setting = BuildSettings(refitSettings);
            var client = BuildClient<T1>();
            var api = Refit.RestService.For<T1>(client, setting);

            return api;
        }

        private static HttpClient BuildClient<T1>()
        {
            var baseUrl = FindHostUrl<T1>();
            //var retryCount = ApiDefinitionExtension.GetRetryCount<T1>(); // todo:// polly integration point for retry
            var timeout = ApiDefinitionExtension.GetTimeout<T1>();
            var handlerType = ApiDefinitionExtension.GetHttpMessageHandler<T1>();

            //var endpointPath = ApiDefinitionExtension.GetEndpointPath<T1>().TrimStart('/');

            //var schemeRegex = new Regex("http(s)?://");
            //var endpointHasScheme = schemeRegex.IsMatch(endpointPath);

            //if(endpointHasScheme)
            //{
            //    baseUrl = endpointPath;
            //}
            //else
            //{
            //    baseUrl += baseUrl.EndsWith("/") ? endpointPath : "/" + endpointPath;
            //}

            var factory = DependencyService.Get<IHttpClientFactory>();
            var client = factory.CreateClient(baseUrl);

            // don't change baseAddress if already set
            if((client.BaseAddress == null || string.IsNullOrEmpty(client.BaseAddress.ToString()) ) && Uri.TryCreate(baseUrl, UriKind.RelativeOrAbsolute, out Uri address))
            {
                client.BaseAddress = address;
                client.Timeout = timeout;
            }


            if(handlerType != null)
            {
                var handler = (MessageProcessingHandler)Activator.CreateInstance(handlerType);
                var hoistHandler = (HoistHandler) client.GetHoistHandler();
                hoistHandler.Hoist = handler;
            }

            return client;
        }

        private static string FindHostUrl<T1>()
        {
            var provider = ApiDefinitionExtension.GetRemoteUrlProvider<T1>();

            if (provider == null) return Configuration.RemoteHost;

            var url = provider().GetAwaiter().GetResult();
            return url;
        }

        private static RefitSettings BuildSettings(RefitSettings refitSettings)
        {
            return refitSettings ?? new RefitSettings();
        }
    }
}