using System;
using System.Reflection;
using System.Resources;
using Xamarin.CommunityToolkit.Helpers;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Analyzer.Extensions
{
    //https://gist.github.com/HenKun/c5fa795fa53ac75a7ecee57bd09a225e
    [ContentProperty("Text")]
    [AcceptEmptyServiceProvider]
    public class TranslateExtension :  IMarkupExtension<BindingBase>, IMarkupExtension, IValueConverter 
    {
        const string ResourceId = "Analyzer.Resources.AppResources";

        static readonly Lazy<ResourceManager> resmgr = new Lazy<ResourceManager>(() => new ResourceManager(ResourceId, typeof(TranslateExtension).GetTypeInfo().Assembly));

        public string StringFormat { get; set; }
        public object Source { get; set; }
        public string Path { get; set; } = ".";


        public string Text { get; set; }

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            Text = value == null ? Text : value as string;
            var translatedText = (this as IMarkupExtension).ProvideValue(null);
            return translatedText;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public object ProvideValue(IServiceProvider serviceProvider)
        {
            if (Text == null)
                return "";

            var ci = LocalizationResourceManager.Current.CurrentCulture;

            var translation = resmgr.Value.GetString(Text, ci);

            if (translation == null)
            {

//#if DEBUG
//                throw new ArgumentException(
//                    String.Format("Key '{0}' was not found in resources '{1}' for culture '{2}'.", Text, ResourceId, ci.Name),
//                    "Text");
//#else
                translation = Text; // returns the key, which GETS DISPLAYED TO THE USER
//#endif
            }
            return translation;
        }

        BindingBase IMarkupExtension<BindingBase>.ProvideValue(IServiceProvider serviceProvider)
        {
            var binding = new Binding(Path, BindingMode.Default, this, null, StringFormat, Source);
            return binding;
        }
    }
}