using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Worosoft.Xamarin.HttpClientExtensions.Attributes
{
    [AttributeUsage(AttributeTargets.Interface, AllowMultiple = false)]
    public class ApiDefinitionAttribute : Attribute
    {
        public ApiDefinitionAttribute()
        {

        }

        public int RetryCount { get; set; } = 3;
        public Type HttpMessageHandler { get; set; }

        /// <summary>
        /// Enable loading remote Url from configuration if its not static
        /// </summary>
        public Func<Task<string>> RemoteUrlProvider { get; set; }

        public TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(50);
    }
}
