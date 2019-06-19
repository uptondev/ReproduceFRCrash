using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TestHScroll
{
    public partial class App : Application
    {
        public static IImageService ImageService;

        public App()
        {
            InitializeComponent();

            RegisterDependencyServices();

            MainPage = new MainPage();
        }

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }

        private void RegisterDependencyServices()
        {
            ImageService = DependencyService.Get<IImageService>();
        }
    }
}
