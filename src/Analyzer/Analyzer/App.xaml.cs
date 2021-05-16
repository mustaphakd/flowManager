
using Analyzer.Services;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Analyzer
{
    public partial class App : Application
    {
        public ApplicationManager Manager { get; }

        //private ApplicationManager manager;
        public App()
        {
            InitializeComponent();

            Manager = new ApplicationManager(this);
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
