using System;
using System.IO;
using System.Runtime.Serialization;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Xamarin.Forms;
using System.Text;

namespace Analyzer.Framework.Files
{
    /// <summary>
    /// Dew Xamarin Files Class
    /// </summary>
    public static class FileSystemManager
    {
        /// <summary>
        /// Local settings name (without extension), default: __woro_loc_sett
        /// </summary>
        public static string SettingsName = "__woro_loc_sett";
        /// <summary>
        /// Default buffer size for file write/read
        /// </summary>
        public static int BufferSize = 0;
        /// <summary>
        /// Read a file as byte array (if app, pass FileSystemManager.ApplicationPath() as basepath)
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static Task<byte[]> ReadLocalFileAsync(string path)
        {
            byte[] result = null;
            using (Stream s = File.Open(path, FileMode.Open))
            {
                result = new byte[s.Length];
                var offset = 0;
                while (s.Read(result, offset, BufferSize) > 0)
                {
                    offset += BufferSize;
                }
            }
            return Task.FromResult(result);
        }
        /// <summary>
        /// Read a file as string (if app, pass FileSystemManager.ApplicationPath() as basepath)
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static Task<string> ReadLocalFileTextAsync(string path)
        {
            return Task.FromResult(FileSystemManager.ReadLocalFileText(path));
        }

        public static string ReadLocalFileText(string path)
        {
            string result = null;
            using (Stream s = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.Write))
            {
                using (StreamReader sr = new StreamReader(s))
                {
                    try
                    {
                        result = sr.ReadToEnd();
                    }
                    catch (Exception ex)
                    {
                        var test = ex;
                    }
                }
            }
            return result;
        }
        /// <summary>
        /// Write a file as byte array (if app, pass FileSystemManager.ApplicationPath() as basepath)
        /// </summary>
        /// <param name="s">data</param>
        /// <param name="name"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        public async static Task WriteLocalFileAsync(byte[] s, string name, string path = null)
        {
            using (FileStream file = File.OpenWrite(path + Path.DirectorySeparatorChar + name))
            {
                BufferSize = BufferSize == 0 ? s.Length : BufferSize;
                var offset = 0;
                while (offset < s.Length)
                {
                    await file.WriteAsync(s, offset, BufferSize);
                    offset += BufferSize;
                }
            }
        }

        /// <summary>
        /// Append to a file as byte array (if app, pass FileSystemManager.ApplicationPath() as basepath)
        /// </summary>
        /// <param name="s">data</param>
        /// <param name="name"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        public async static Task AppendLocalFileAsync(byte[] s, string name, string path = null)
        {
            using (FileStream file = File.OpenWrite(path + Path.DirectorySeparatorChar + name))
            {
                var fileLength = file.Length;
                file.Position = fileLength;

                BufferSize = BufferSize == 0 ? s.Length : BufferSize;
                var offset = 0;
                while (offset < s.Length)
                {
                    await file.WriteAsync(s, offset, BufferSize);
                    offset += BufferSize;
                }
            }
        }

        private static Object writerLock_ = new object();

        /// <summary>
        /// Write a file as string (if app, pass FileSystemManager.ApplicationPath() as basepath)
        /// </summary>
        /// <param name="s">data</param>
        /// <param name="name"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        public static Task WriteLocalFileTextAsync(string s, string name, string path = null)
        {
            var fullPath = Path.Combine(path, name);

            lock (writerLock_)
            {
                using (var file = File.Open(fullPath, FileMode.OpenOrCreate, FileAccess.Write, FileShare.Read))
                {
                    using (StreamWriter sw = new StreamWriter(file))
                    {
                        sw.Write(s);
                    }
                }
            }

            return Task.CompletedTask;
        }

        /// <summary>
        /// Append a file as string (if app, pass FileSystemManager.ApplicationPath() as basepath)
        /// </summary>
        /// <param name="s">data</param>
        /// <param name="name"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        public async static Task AppendLocalFileTextAsync(string s, string name, string path = null)
        {
            using (FileStream file = File.OpenWrite(path + Path.DirectorySeparatorChar + name))
            {
                file.Position = file.Length;

                using (StreamWriter sw = new StreamWriter(file))
                {
                    await sw.WriteAsync(s);
                }
            }
        }
        /// <summary>
        /// Return the base path for the current platform
        /// </summary>
        /// <returns></returns>
        public static string ApplicationPath()
        {
            //var assembly = Application.Current.GetType().Assembly;
            string osv = "";

            try
            {
                switch (Device.RuntimePlatform)
                {
                    case Device.iOS:
                    case Device.macOS:
                        {
                            osv = "Library";
                            break;
                        }
                    case Device.Android:
                        break;
                    case Device.UWP:
                        //case Device.WinRT:
                        //case Device.WinPhone:
                        {
                            osv = "LocalState";
                            break;
                        }
                }

            }
            catch(Exception) {
            }

#if ANDROID


#elif IOS

                            osv = "Library";
#endif

            return Path.Combine(Path.GetDirectoryName(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)), osv);
        }

        public static string ConstructFullPath(params string[] pathes)
        {
            var root = FileSystemManager.ApplicationPath();

            if (pathes == null || pathes.Length < 1)
            {
                return root;
            }

            var dir = new StringBuilder();
            dir.Append(root + Path.DirectorySeparatorChar + pathes[0]);

            for(var i = 1; i < pathes.Length; i++)
            {
                var path = pathes[i];
                dir.Append(Path.DirectorySeparatorChar + path);
            }

            return dir.ToString();
        }

        public static string Combine(string directory, string fileName)
        {
            return Path.Combine(directory, fileName);
        }

        /// <summary>
        /// Check if a file exists
        /// </summary>
        /// <param name="file">the path to the file</param>
        /// <returns></returns>
        public static bool CheckFileExists(string file)
        {
            var fil = new FileInfo(file);
            return fil.Exists;
        }


        public static void EnsureFileCreated(string file, bool recreateFile = false)
        {
            var exist = CheckFileExists(file);

            if (exist && recreateFile == false )
            { return; }

            if (exist == true)
            {
                File.Delete(file);
                using (File.Create(file)) { }
            }

            var fileInfo = new FileInfo(file);
            using (fileInfo.OpenWrite()) { }
        }

        /// <summary>
        /// Check if a directory exists
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static bool CheckDirectoryExists(string path)
        {
            var dir = new DirectoryInfo(path);
            return dir.Exists;
        }

        public static void EnsureDirectoryCreated(string path)
        {
            if (CheckDirectoryExists(path))
            {
                return;
            }

            var dir = new DirectoryInfo(path);
            dir.Create();
        }


        /// <summary>
        /// Write a local setting
        /// </summary>
        /// <param name="key"></param>
        /// <param name="option"></param>
        /// <returns></returns>
        public async static Task WriteLocalSetting(string key, string option)
        {
            var path = ApplicationPath();
            if (!CheckFileExists(path + Path.DirectorySeparatorChar + SettingsName + ".json"))
            {
                await WriteLocalFileTextAsync("{}", SettingsName + ".json", path + Path.DirectorySeparatorChar);
            }
            string json = await ReadLocalFileTextAsync(path + Path.DirectorySeparatorChar + SettingsName + ".json");
            Dictionary<string, string> settings = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, string>>(json);
            if (settings.ContainsKey(key))
            {
                string serializate = option;
                settings[key] = serializate;
            }
            else
                settings.Add(key, option.ToString());
            await WriteLocalFileTextAsync(System.Text.Json.JsonSerializer.Serialize(settings), SettingsName + ".json", path + Path.DirectorySeparatorChar);
        }
        /// <summary>
        /// Write a local setting of type T (T must be serializable)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">Key under which to store model</param>
        /// <param name="option"> Model to store</param>
        /// <returns></returns>
        public async static Task WriteLocalSetting<T>(string key, T option)
        {
            var path = ApplicationPath();
            if (!CheckFileExists(path + Path.DirectorySeparatorChar + SettingsName + ".json"))
            {
                await WriteLocalFileTextAsync("{}", SettingsName + ".json", path + Path.DirectorySeparatorChar);
            }


            string json = String.Empty;
            Dictionary<string, string> settings;

            try
            {
                json = await ReadLocalFileTextAsync(path + Path.DirectorySeparatorChar + SettingsName + ".json");
                settings = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, string>>(json);

            }
            catch
            {
                settings = new Dictionary<string, string>();
            }


            string serializate = SerializeObject(option);

            if (settings.ContainsKey(key))
            {
                settings[key] = serializate;
            }
            else
                settings.Add(key, serializate);
            await WriteLocalFileTextAsync(System.Text.Json.JsonSerializer.Serialize(settings), SettingsName + ".json", path + Path.DirectorySeparatorChar);
        }
        /// <summary>
        /// Check if a local setting exists
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public async static Task<bool> CheckLocalSettingExists(string key)
        {
            var path = ApplicationPath();
            if (!CheckFileExists(Path.Combine(path, SettingsName + ".json")))
            {
                await WriteLocalFileTextAsync("{}", SettingsName + ".json", path + Path.DirectorySeparatorChar);
            }
            string json = await ReadLocalFileTextAsync(Path.Combine(ApplicationPath(), SettingsName + ".json"));
            Dictionary<string, string> settings = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, string>>(json);
            return settings.ContainsKey(key);
        }
        /// <summary>
        /// Read a local setting of type T
        /// </summary>
        /// <typeparam name="T">model to pull from settings storage</typeparam>
        /// <param name="key">Key under which the model is stored</param>
        /// <returns></returns>
        public async static Task<T> ReadLocalSettingAsync<T>(string key)
        {
            if (!CheckFileExists(Path.Combine(ApplicationPath(), SettingsName + ".json")))
            {
                throw new FileNotFoundException();
            }
            var path = Path.Combine(ApplicationPath(), SettingsName + ".json");
            string json = await ReadLocalFileTextAsync(path);
            Dictionary<string, string> settings = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, string>>(json);
            var source = settings[key];
            var deserialized = DeserializeObject<T>(source);
            return deserialized;
        }


        public static T ReadLocalSetting<T>(string key)
        {
            if (!CheckFileExists(Path.Combine(ApplicationPath(), SettingsName + ".json")))
            {
                throw new FileNotFoundException();
            }
            var path = Path.Combine(ApplicationPath(), SettingsName + ".json");
            string json = ReadLocalFileText(path);
            Dictionary<string, string> settings = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, string>>(json);
            var source = settings[key];
            var deserialized = DeserializeObject<T>(source);
            return deserialized;
        }
        /// <summary>
        /// Read a local setting as string
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public async static Task<string> ReadLocalSetting(string key)
        {
            if (!CheckFileExists(Path.Combine(ApplicationPath(), SettingsName + ".json")))
            {
                throw new FileNotFoundException();
            }
            string json = await ReadLocalFileTextAsync(Path.Combine(ApplicationPath(), SettingsName + ".json"));
            Dictionary<string, string> settings = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, string>>(json);
            return settings[key];
        }
        /// <summary>
        /// Serialize to json an object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="toSerialize"></param>
        /// <returns></returns>
        public static string SerializeObject<T>(T toSerialize)
        {
            return System.Text.Json.JsonSerializer.Serialize(toSerialize);
        }
        /// <summary>
        /// Serialize to json an object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="toDeserialize"></param>
        /// <returns></returns>
        public static T DeserializeObject<T>(string toDeserialize)
        {
            return System.Text.Json.JsonSerializer.Deserialize<T>(toDeserialize);
        }
    }
}


