using System;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using MathOperation.ViewModel;

namespace MathOperation.Droid
{
    [Activity(Label = "MathOperation", Icon = "@mipmap/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        Xamarin.Forms.Button button;
        TimerViewModeal Timer;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(savedInstanceState);

            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);
            var app = new App();
            LoadApplication(app);

            button = app._MainPage.timerButton;
            Timer = app._MainPage.MainViewModel.TimerViewModeal;
            Timer.Timer.Elapsed += Timer_Elapsed;

            Timer.Start();

        }

        private void Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            Timer.Increment();
            RunOnUiThread(() => button.Text = Timer.GetTime());
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
    }
}