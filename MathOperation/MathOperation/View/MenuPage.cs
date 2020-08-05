using MathOperation.Common;
using MathOperation.ViewModel;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace MathOperation.View
{
    public class MenuPage : ContentPage
    {
        public StackLayout MainLayout { get; private set; }
        public Button StartButton { get; private set; }
        public Button ResumeButton { get; private set; }
        public Button SettingButton { get; private set; }
        public Button CancelButton { get; private set; }

        public StackLayout LayoutForButton { get; private set; }

        public MainPage MainPage { get; private set; }

        public Button TimerButton { get; private set; }
        public Grid Grid { get; private set; }
        public int Goal { get; private set; }
        public MenuPage(Button timerButton = null, MainViewModel viewModel = null, Grid grid = null, int goal = -1)
        {
            TimerButton = timerButton;
            Grid = grid;
            Goal = goal;
            MainPage = new MainPage(string.Empty, string.Empty, viewModel, TimerButton);

            StartButton = CreateButtonByText("Start");
            SettingButton = CreateButtonByText("Setting");
            CancelButton = CreateButtonByText("Cancel");
            ResumeButton = CreateButtonByText("Resume");

            StartButton.Clicked += ClickOnStartButton;
            ResumeButton.Clicked += ClickOnResumeButton;

            LayoutForButton = new StackLayout
            {
                Orientation = StackOrientation.Vertical,
                BackgroundColor = Color.Black,
                VerticalOptions = LayoutOptions.CenterAndExpand,
                Children =
                {
                    StartButton,
                    ResumeButton,
                    SettingButton,
                    CancelButton
                }
            };
            

            MainLayout = new StackLayout
            {
                Orientation = StackOrientation.Vertical,
                BackgroundColor = Color.Black,
                VerticalOptions = LayoutOptions.CenterAndExpand,
                HeightRequest = StaticResources.Height,
                Children =
                {
                    LayoutForButton
                }
            };

            Content = MainLayout;
            NavigationPage.SetHasNavigationBar(this, false);
        }

        private Button CreateButtonByText(string text, ImageSource image = null)
        {
            return new Button
            {
                Text = text,
                BackgroundColor = StaticResources.GoalBackgroundColor,
                TextColor = Color.White,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center,
                CornerRadius = StaticResources.RadiusGoal,
                WidthRequest = StaticResources.Width * 0.9,
                Margin = 5
            };
        }

        private async void ClickOnStartButton(object sender, EventArgs args)
        {
            MainPage.MainViewModel.TimerViewModeal.Start();
            MainPage.IsStart = true;
            await Navigation.PushAsync(MainPage);
        }

        private async void ClickOnResumeButton(object sender, EventArgs args)
        {
            MainPage.MainViewModel.TimerViewModeal.Resume();
            await Navigation.PushAsync(new MainPage(string.Empty, string.Empty, MainPage.MainViewModel, MainPage.timerButton, Grid, Goal));
        }
    }
}
