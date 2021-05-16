using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Analyzer.Services.Impl
{
    public class DialogService : IDialogService
    {
        public async Task<bool> ConfirmDeletion(string contentTextAppend, bool singleItem)
        {
            var translateExtension = new Analyzer.Extensions.TranslateExtension();
            translateExtension.Text = "ConfirmItemDeletionTitle";
            var deleteTitle = translateExtension.ProvideValue(null) as string;

            translateExtension.Text = singleItem == true ? "ConfirmItemDeletionContent" : "ConfirmItemDeletionCount";
            var confirmationDeleteItemText = translateExtension.ProvideValue(null) as string;
            var confirmationDeleteItemTextFormat = String.Format(confirmationDeleteItemText, contentTextAppend);

            translateExtension.Text = "Yes";
            var yestText = translateExtension.ProvideValue(null) as string;


            translateExtension.Text = "No";
            var NoText = translateExtension.ProvideValue(null) as string;

            bool answer = await App.Current.MainPage.DisplayAlert(deleteTitle, confirmationDeleteItemTextFormat, yestText, NoText);

            return answer;
        }

        public async Task AlertInfo(string titleKey, string messageKey, params Object[] args)
        {
            var translateExtension = new Analyzer.Extensions.TranslateExtension();
            translateExtension.Text = titleKey;
            var title = translateExtension.ProvideValue(null) as string;

            translateExtension.Text = messageKey;
            var messageText = translateExtension.ProvideValue(null) as string;

            if (args.Length > 0)
            {
                messageText = String.Format(messageText, args);
            }

            translateExtension.Text = "Ok";
            var okText = translateExtension.ProvideValue(null) as string;

            await App.Current.MainPage.DisplayAlert(title, messageText, okText);
        }
    }
}
