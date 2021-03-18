using System.IO;
using System.Reflection;
using System.Runtime.Serialization.Json;

namespace Analyzer.Core
{
    internal static class EmbeddedResourceHelper
    {
        internal static string Load(string resourceName)
        {
            var assembly = Assembly.GetExecutingAssembly();

            using (var stream = assembly.GetManifestResourceStream(resourceName))
            using (var reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }


        /// <summary>
        /// Populates the data for view model from json file.
        /// </summary>
        /// <typeparam name="T">Type of view model.</typeparam>
        /// <param name="fileName">Json file to fetch data. Path should include directories name separated with a dot(.)</param>
        /// <returns>Returns the view model object.</returns>
        internal static T PopulateData<T>(string fileName)
        {
            var assembly = typeof(App).GetTypeInfo().Assembly;

            T obj;

            using (var stream = assembly.GetManifestResourceStream(fileName))
            {
                var serializer = new DataContractJsonSerializer(typeof(T));
                obj = (T)serializer.ReadObject(stream);
            }

            return obj;
        }
    }
}