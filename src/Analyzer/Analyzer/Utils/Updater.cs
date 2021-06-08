using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Analyzer.Framework.Files;
using Analyzer.Infrastructure;
using Analyzer.Models;
using Xamarin.Essentials;
using Xamarin.Forms;
using System.Linq;

namespace Analyzer.Utils
{
    internal class Updater
    {
        private string _appUdateUrl;
        private int _updateChecks;
        //private bool _performUpdate = false;

        const string APPUPDATECHECK = "application_update_check";
        const string APPCURRENTVERSION = "applicaton_version";
        const string APPNEXTVERSION = "applicaton_next_version";

        public Updater(string appUpdateUrl, int updateChecks)
        {
            _appUdateUrl = appUpdateUrl;
            _updateChecks = updateChecks;
        }

        public async Task CheckforUpdate()
        {
            var exist = await FileSystemManager.CheckLocalSettingExists(APPUPDATECHECK);

            if (! exist) //first time usage
            {
                await FileSystemManager.WriteLocalSetting(APPUPDATECHECK, DateTime.Today);
                return;
            }

            var maxAllowedWaitDuration = TimeSpan.FromDays(_updateChecks);
            var lastCheck = await FileSystemManager.ReadLocalSettingAsync<DateTime>(APPUPDATECHECK);
            var today = DateTime.Today;
            var duration = today - lastCheck;

            if (duration <= maxAllowedWaitDuration) return;

            var client = new HttpClient();
            var result = await client.GetByteArrayAsync(_appUdateUrl);
            var rawJson = Encoding.GetEncoding("utf-8").GetString(result, 0, result.Length - 1);
            var updates = System.Text.Json.JsonSerializer.Deserialize<UpdateModel[]>(result);

            if (updates == null || updates.Length < 1) return;

            var currentPlatform = Enum.GetName(typeof(DevicePlatform), DeviceInfo.Platform).ToLowerInvariant();
            var updateForCurrentPlatform = updates.FirstOrDefault(item => item.DevicePlatform == currentPlatform);

            if (updateForCurrentPlatform == null) return;

            var currentAppVersion = await FileSystemManager.ReadLocalSettingAsync<string>(APPCURRENTVERSION);
            var nextVersion = updateForCurrentPlatform.Version;
            await FileSystemManager.WriteLocalSetting(APPUPDATECHECK, DateTime.Today);

            if (nextVersion == currentAppVersion)
            {
                //no update available
                await FileSystemManager.WriteLocalSetting(APPNEXTVERSION, null);
                return;
            }


            await FileSystemManager.WriteLocalSetting(APPNEXTVERSION, updateForCurrentPlatform);
            ////_performUpdate = true ;
        }

        public async Task SetCurrentVersion(string version)
        {
            await FileSystemManager.WriteLocalSetting(APPCURRENTVERSION, version);
        }

        public async Task PerformUpdate()
        {
            var updateInfo = await GetInfo();
            await Browser.OpenAsync(updateInfo.Url, BrowserLaunchMode.SystemPreferred);
        }

        public async Task<UpdateModel> GetInfo()
        {
            //if (!_performUpdate) return null;

            var updateInfo = await FileSystemManager.ReadLocalSettingAsync<UpdateModel>(APPNEXTVERSION);
            return updateInfo;
        }
    }
}
