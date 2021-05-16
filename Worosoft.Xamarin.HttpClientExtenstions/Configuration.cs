using Refit;
using System;
using System.Collections.Generic;
using System.Text;

namespace Worosoft.Xamarin.HttpClientExtensions
{
    public static class Configuration
    {
       public static RefitSettings RefitSettings { get; set; }

        public static string RemoteHost { get; set; }
    }
}
