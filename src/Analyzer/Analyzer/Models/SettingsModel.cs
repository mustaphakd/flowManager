using System.Collections.ObjectModel;
using System.Runtime.Serialization;

namespace Analyzer.Models
{
    [DataContract]
    public class SettingsModel
    {
        public static ObservableCollection<Language> Languages { get; }

        public Language Language { get; set; }

        static SettingsModel()
        {
            Languages = new ObservableCollection<Language>()
            {
                new Language { DisplayName =  "中文 - Chinese (simplified)", ShortName = "zh-Hans" },
                new Language { DisplayName =  "Chinese(Traditional)", ShortName = "zh-Hant" },
                new Language { DisplayName =  "English", ShortName = "en" },
                new Language { DisplayName =  "Français - French", ShortName = "fr" },
                new Language { DisplayName =  "Deutsche - German", ShortName = "de" },
                new Language { DisplayName =  "日本語 - Japanese", ShortName = "ja" },
                new Language { DisplayName =  "한국어 - Korean", ShortName = "ko" },
                new Language { DisplayName =  "Română - Romanian", ShortName = "ro" },
                new Language { DisplayName =  "Русский - Russian", ShortName = "ru" }
            };

            // PickerLanguages.SelectedIndexChanged += PickerLanguages_SelectedIndexChanged;

        }
        // inside xaml =>  xmlns:i18n="clr-namespace:obile.Extensions;assembly=LocalizationSample"
        // <Label Text="{i18n:Translate HelloWorld}"

        /*
         * private void PickerLanguages_SelectedIndexChanged(object sender, EventArgs e)
        {
            var language = Languages[PickerLanguages.SelectedIndex];

            try
            {
                var culture = new CultureInfo(language.ShortName);
                AppResources.Culture = culture;
                CrossMultilingual.Current.CurrentCultureInfo = culture;
            }
            catch (Exception)
            {
            }

            LabelTranslate.Text = AppResources.HelloWorld;
        }
         * */
         [DataMember(Name = "serverUrl")]
        public string ServerEndpoint { get; set; }


        [DataMember(Name = "devMode", IsRequired =false)]
        public bool EnableDebugMode { get; set; }


        [DataMember(Name = "bluetoothTracking", EmitDefaultValue = true)]
        [System.ComponentModel.DefaultValue(false)]
        public bool BluetoothTrackingEnabled { get; set; }


        [DataMember(Name = "devModeVisible", IsRequired = false)]
        public bool DebugModeVisible { get; set; }

        [DataMember(Name = "connectionString")]
        public string DefaultConnectionString { get; set; }
    }
}
