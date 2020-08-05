using MathOperation.View;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MathOperation
{
    public partial class App : Application
    {
        public MenuPage _MenuPage;
        public App()
        {
            InitializeComponent();
            _MenuPage = new MenuPage();
            MainPage =  new NavigationPage(_MenuPage);
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
