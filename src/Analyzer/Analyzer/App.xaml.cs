
using Analyzer.Services;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

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

            // Manager.PersistAuthenticationAcrossReload= true/false;
            //Manager.AppUpdateUrl = string

            var task = Manager.Initialize();

            MainPage = new Pages.Main();


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
