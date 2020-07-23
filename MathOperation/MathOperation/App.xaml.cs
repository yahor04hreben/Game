using MathOperation.View;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MathOperation
{
    public partial class App : Application
    {
        public MainPage _MainPage;
        public App()
        {
            InitializeComponent();
            _MainPage = new MainPage(string.Empty, string.Empty);
            MainPage = new NavigationPage(_MainPage);
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
