using Analyzer.Core;
using Analyzer.Framework.Files;
using Analyzer.Models;
using System;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Analyzer.Services.Impl
{
    public class SettingsService : ISettingsService
    {
        private string key => "wor_settings_00BABA54-91AD-402F-82F1-A0FF7345AE31"; //DefaultSettings.SettingsStorage;
        public SettingsModel GetSettings()
        {
            SettingsModel model = null ; //= new SettingsModel { ServerEndpoint = DefaultSettings.RootApiUrl };

            try
            {
                if (App.Current.Properties.ContainsKey(key))
                {
                    model = (SettingsModel)App.Current.Properties[key];
                }
            }
            catch (Exception)
            {

            }


            if (model == null)
            {
                try
                {
                    this.RetrieveModel(
                        (result) =>
                        {
                            model = result;
                        });
                }
                catch(Exception e)
                {
                    var test = e;
                    model = this.InitializeSettingModel();
                    this.registerSettingsModel(model);
                }
            }

            if (model == null)
            {
                model = this.InitializeSettingModel();
                this.registerSettingsModel(model);
            }

            return model;
        }

        private void RetrieveModel(Action<SettingsModel> modelSetter)
        {
            SettingsModel model = null;

            try
            {
                model = FileSystemManager.ReadLocalSetting<SettingsModel>(this.key);
            }
            catch(Exception ex)
            {
                DependencyService.Get<Infrastructure.ILoggingService>().Error(ex);
            }

            Core.CoreHelpers.ValidateDefined(model, "SettingsModel is required");

            modelSetter(model);
        }

        private async void registerSettingsModel(SettingsModel model)
        {
            await FileSystemManager.WriteLocalSetting<SettingsModel>(this.key, model);
            await this.SetSettings(model);
        }

        private SettingsModel InitializeSettingModel()
        {
            var defaultSettingsModel = EmbeddedResourceHelper.PopulateData<SettingsModel>("Analyzer.Data.settings.json");
            var model = new SettingsModel
            {
                DefaultConnectionString = "data.bin",
                Language = new Language { DisplayName = "English", ShortName = "en" },
                ServerEndpoint = defaultSettingsModel.ServerEndpoint
            };

            return model;
        }

        public async Task SetSettings(SettingsModel model)
        {
            Core.CoreHelpers.ValidateDefined(model, $"{nameof(model)} is required");
            Application.Current.Properties[key] = model;
            await Application.Current.SavePropertiesAsync();
            await FileSystemManager.WriteLocalSetting<SettingsModel>(this.key, model);
        }
    }
}
