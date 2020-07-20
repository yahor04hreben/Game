using MathOperation.View;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MathOperation
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            MainPage = new NavigationPage(new MainPage(string.Empty, string.Empty));
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
