
using Analyzer.Services;
using Analyzer.Views;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

[assembly: ExportFont("FontAwesome5Regular400.otf", Alias = "FontAwesome5Regular")]
namespace Analyzer
{
    public partial class App : Application
    {
        public ApplicationManager Manager { get; }
        public App()
        {
            // inside SettingsServices, change key and defaultStorage values to match app
            InitializeComponent();
            //Set settings Name
            //Framework.Files.FileSystemManager.SettingsName = "xxxx_ss"

            Manager = new ApplicationManager(this, "1.0.0", 15);
            Manager.RegisterMenuItemsProvider<Services.Impl.MenuItemsProvider>(); //comment this line if app doesn't use authorization and roles

            // Manager.PersistAuthenticationAcrossReload= true/false;
            //Manager.AppUpdateUrl = string

            Manager.SetViews(typeof(Pages.Main),typeof(LoginView) ); //typeof(LoginView)
            var task = Manager.Initialize();

            task.GetAwaiter().GetResult();
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
