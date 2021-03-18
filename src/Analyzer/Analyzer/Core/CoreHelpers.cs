using System;

namespace Analyzer.Core
{
    public static class CoreHelpers
    {
        public static void ValidateDefined<T>(T arg, string message)
        {
            if (arg == null)
            {
                throw new ArgumentNullException(nameof(arg), message);
            }

            var stringValue = arg as string;
            if( stringValue == null)
            {
                return;
            }

            var trimmed = stringValue.Trim();

            if (trimmed.Length < 1)
            {
                throw new ArgumentNullException(nameof(arg), message);
            }
        }

        public static string RetrieveTranslation(string translationKey)
        {
            var translateExtension = new Analyzer.Extensions.TranslateExtension();
            translateExtension.Text = translationKey;
            var translation = translateExtension.ProvideValue(null) as string;
            return translation;
        }

        public static string FormatTranslation(string translationKey,params string[]  args)
        {
            var translatedText = CoreHelpers.RetrieveTranslation(translationKey);
            var formatedText = String.Format(translatedText, args);
            return formatedText;
        }
    }
}