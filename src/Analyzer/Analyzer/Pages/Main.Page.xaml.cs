using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Analyzer.Core;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Xamarin.Essentials;

namespace Analyzer.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Main : FlyoutPage
    {
        public Main()
        {
            InitializeComponent();

            flyoutPage.listView.ItemSelected += OnItemSelected;

            // Subscribe to changes of screen metrics
            DeviceDisplay.MainDisplayInfoChanged += OnMainDisplayInfoChanged;

            // Get Metrics
            var mainDisplayInfo = DeviceDisplay.MainDisplayInfo;
            HandleDisplayInfo(mainDisplayInfo);
        }

        private void OnMainDisplayInfoChanged(object sender, DisplayInfoChangedEventArgs e)
        {
            // Process changes
            var displayInfo = e.DisplayInfo;
            HandleDisplayInfo(displayInfo);
        }

        private void HandleDisplayInfo(DisplayInfo displayInfo)
        {
            // Orientation (Landscape, Portrait, Square, Unknown)
            var orientation = displayInfo.Orientation;



            if (Device.RuntimePlatform == Device.UWP && orientation == DisplayOrientation.Portrait)
            {
                FlyoutLayoutBehavior = FlyoutLayoutBehavior.Popover;
            }
            else if (Device.Idiom == TargetIdiom.Tablet)
            {
                FlyoutLayoutBehavior = FlyoutLayoutBehavior.SplitOnLandscape;
            }
            else if(Device.Idiom == TargetIdiom.Desktop || Device.Idiom == TargetIdiom.TV)
            {
                FlyoutLayoutBehavior = FlyoutLayoutBehavior.Split;
            }
        }

        void OnItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            var item = e.SelectedItem as PageItem;
            if (item != null)
            {
                Detail = new NavigationPage((Page)Activator.CreateInstance(item.TargetType));
                flyoutPage.listView.SelectedItem = null;

                var behavior = this.FlyoutLayoutBehavior;
                // Get Metrics
                var mainDisplayInfo = DeviceDisplay.MainDisplayInfo;

                // Orientation (Landscape, Portrait, Square, Unknown)
                var orientation = mainDisplayInfo.Orientation;

                if (behavior == FlyoutLayoutBehavior.Split ||
                    (behavior == FlyoutLayoutBehavior.SplitOnLandscape && orientation == DisplayOrientation.Landscape) ||
                    (behavior == FlyoutLayoutBehavior.SplitOnPortrait && orientation == DisplayOrientation.Portrait)) return;
                IsPresented = false;
            }
        }
    }
}