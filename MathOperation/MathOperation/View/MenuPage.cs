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
        public MainViewModel MainViewModel { get; private set; }

        public Button TimerButton { get; private set; }
        public Grid Grid { get; private set; }
        public int Goal { get; private set; }

        public int FromNumber { get; set; }

        public int ToNumber { get; set; }

        private bool _IsPaused;
        public bool IsPaused
        {
            get => _IsPaused;
            set
            {
                _IsPaused = value;
                if (_IsPaused)
                    ResumeColor = StaticResources.GoalBackgroundColor;
                else
                    ResumeColor = StaticResources.AddCellUnClickableColor;


            }
        }

        private Color _ResumeColor;
        public Color ResumeColor
        {
            get => _ResumeColor;
            set
            {
                if(_ResumeColor != value)
                {
                    _ResumeColor = value;
                    OnPropertyChanged("ResumeColor");
                }
            }
        }

        public MenuPage(MenuPage menuPage, int From, int To) : this( menuPage.TimerButton, menuPage.MainViewModel, menuPage.Grid, menuPage.Goal)
        {
            this.FromNumber = From;
            this.ToNumber = To;
        }

        public MenuPage(Button timerButton = null, MainViewModel viewModel = null, Grid grid = null, int goal = -1)
        {
            IsPaused = false;
            TimerButton = timerButton;
            Grid = grid;
            Goal = goal;
            MainViewModel = viewModel;
            MainPage = new MainPage(string.Empty, string.Empty, MainViewModel, TimerButton);

            StartButton = CreateButtonByText("Start");
            SettingButton = CreateButtonByText("Setting");
            CancelButton = CreateButtonByText("Cancel");
            ResumeButton = CreateButtonByText("Resume", true);

            StartButton.Clicked += ClickOnStartButton;
            ResumeButton.Clicked += ClickOnResumeButton;
            SettingButton.Clicked += ClickOnSettingButton;

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
            MainPage.MainViewModel.TimerViewModeal.ResetTimer();
        }

        private Button CreateButtonByText(string text, bool isResumeButton = false)
        {
            var btn =  new Button
            {
                Text = text,
                TextColor = Color.White,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center,
                CornerRadius = StaticResources.RadiusGoal,
                WidthRequest = StaticResources.Width * 0.9,
                Margin = 5
            };

            if (isResumeButton)
            {
                Binding bindingColor = new Binding { Source = this, Path = "ResumeColor" };
                btn.SetBinding(Button.BackgroundColorProperty, bindingColor);
            }
            else
                btn.BackgroundColor = StaticResources.GoalBackgroundColor;

            return btn;
        }

        private async void ClickOnStartButton(object sender, EventArgs args)
        {
            MainPage.MainViewModel.TimerViewModeal.Start();
            MainPage.IsStart = true;
            MainPage?.MainViewModel?.HelpViewModel?.ResetCount(); 
            await Navigation.PushAsync(MainPage);
        }

        private async void ClickOnResumeButton(object sender, EventArgs args)
        {
            if(IsPaused)
            {
                MainPage.MainViewModel.TimerViewModeal.Resume();
                await Navigation.PushAsync(new MainPage(string.Empty, string.Empty, MainPage.MainViewModel, MainPage.timerButton, Grid, Goal));
            }
        }

        private async void ClickOnSettingButton(object sender, EventArgs args)
        {
            await Navigation.PushAsync(new SetNumbersPage(this));
        }
    }
}
