using MathOperation.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xamarin.Forms;

namespace MathOperation.View
{
    public class SetNumbersPage : ContentPage
    {
        private MainPage page;
        Frame frame = null;
        private string From = String.Empty;
        private string To = string.Empty;

        private Entry fromText;
        private Entry toText;

        private Label errorLabel;

        private StackLayout layoutFrom;
        private StackLayout layoutTo;
        private StackLayout minorLayout;
        private StackLayout majorLayout;

        private Color _BorderColor;
        public Color BorderColor
        {
            get => _BorderColor;
            set
            {
                _BorderColor = value;
                OnPropertyChanged("BorderColor");
            }
        }
        public SetNumbersPage(MainPage page)
        {
            this.page = page;
            BorderColor = Color.Red;
            frame = new Frame();
            frame.HorizontalOptions = LayoutOptions.Center;
            frame.WidthRequest = StaticResources.Width * 0.8;
            frame.VerticalOptions = LayoutOptions.Center;
            frame.HeightRequest = StaticResources.Height * 0.5;
            frame.BackgroundColor = StaticResources.GoalBackgroundColor;
            CreateFromBlock();
            /*var label = new Label
            {
                Text = "Hello",
                FontSize = Device.GetNamedSize(NamedSize.Large, typeof(Label)),
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.Center,
                TextColor = StaticResources.ColorGoalText
            };

            frame.Content = label;
            */
            Content = frame;
            BackgroundColor = StaticResources.CellColor;
        }

        public void CreateFromBlock()
        {
            majorLayout = new StackLayout
            {
                Orientation = StackOrientation.Vertical,
                VerticalOptions = LayoutOptions.FillAndExpand,
                HorizontalOptions = LayoutOptions.FillAndExpand
            };

            var boxWidth = StaticResources.Height * 0.25;
            var boxHeight = StaticResources.Height * 0.5 * 0.3;

            layoutFrom = new StackLayout
            {
                Orientation = StackOrientation.Vertical,
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                VerticalOptions = LayoutOptions.CenterAndExpand,
                HeightRequest = boxHeight,
                WidthRequest = boxWidth
            };

            layoutTo = new StackLayout
            {
                Orientation = StackOrientation.Vertical,
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                VerticalOptions = LayoutOptions.CenterAndExpand,
                HeightRequest = boxHeight,
                WidthRequest = boxWidth
            };

            var fromLabel = new Label { Text = "From:", FontSize = StaticResources.GoalTextSize * 0.7, WidthRequest = boxWidth };
            fromText = new Entry
            {
                WidthRequest = boxWidth * 0.9,
                BackgroundColor = Color.White,
                Placeholder = "From",
            };

            var toLabel = new Label
            {
                Text = "To:",
                FontSize = StaticResources.GoalTextSize * 0.7,
                WidthRequest = boxWidth ,
            };

            toText = new Entry 
            {
                WidthRequest = boxWidth * 0.9,
                BackgroundColor = Color.White,
                Placeholder = "To",
            };

            layoutFrom.Children.Add(fromLabel);
            layoutFrom.Children.Add(fromText);

            layoutTo.Children.Add(toLabel);
            layoutTo.Children.Add(toText);

            minorLayout = new StackLayout
            {
                Orientation = StackOrientation.Horizontal,
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                VerticalOptions = LayoutOptions.CenterAndExpand,
                HeightRequest = frame.Height,
                WidthRequest = frame.Width
            };

            minorLayout.Children.Add(layoutFrom);
            minorLayout.Children.Add(layoutTo);

            StackLayout layoutToButton = new StackLayout
            {
                Orientation = StackOrientation.Horizontal,
                VerticalOptions = LayoutOptions.End,
                HorizontalOptions = LayoutOptions.CenterAndExpand
            };

            var OkButton = new Button
            {
                Text = "OK",
                FontSize = StaticResources.GoalTextSize * 0.6,
                BackgroundColor = Color.White,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center,
                WidthRequest = frame.WidthRequest * 0.5,
                Margin = new Thickness(5),
            };
            OkButton.Clicked += ClickOnOk;

            var cancelButton = new Button
            {
                Text = "Cancel",
                FontSize = StaticResources.GoalTextSize * 0.5,
                BackgroundColor = Color.White,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center,
                WidthRequest = frame.WidthRequest * 0.5,
                Margin = new Thickness(5)
            };
            cancelButton.Clicked += ClickOnCancel;

            layoutToButton.Children.Add(cancelButton);
            layoutToButton.Children.Add(OkButton);

            majorLayout.Children.Add(minorLayout);
            majorLayout.Children.Add(layoutToButton);

            frame.Content = majorLayout;
        }

        private async void ClickOnCancel(object sender, EventArgs e)
        {
            page.MainViewModel.TimerViewModeal.Resume();
            await Navigation.PopModalAsync();
        }

        private async void ClickOnOk(object sander, EventArgs args)
        {
            bool resultFrom = ValidateFromData(fromText);
            bool resultTo = ValidateToData(toText);

            if (resultFrom && resultTo)
            {
                if (Int32.Parse(fromText.Text) >= Int32.Parse(toText.Text))
                    CreateErrorLabel();
                else
                    await Navigation.PushModalAsync(new MainPage(fromText.Text, toText.Text, page.MainViewModel, page.timerButton, page.grid));
            }   
        }


        private void CreateErrorLabel()
        {
            fromText.Text = string.Empty;
            toText.Text = string.Empty;

            fromText.Placeholder = "Less than To";
            fromText.PlaceholderColor = Color.Red;

            toText.Placeholder = "More than From";
            toText.PlaceholderColor = Color.Red;
        }
        public bool ValidateData(Entry entry, Predicate<string> condition, out string errorMessage)
        {
            if (entry.Text == null || entry.Text == string.Empty)
            {
                errorMessage = "Not Empty";
                return false;
            }
            else
            {
                var result = condition(entry.Text.Trim());
                if (!result)
                    errorMessage = "More than 1";
                else
                    errorMessage = string.Empty;

                return result;
            }  
           
        }

        public bool ValidateFromData(Entry entry)
        {
            int number;
            string errorMessage;
            if (!ValidateData(fromText, t => Int32.TryParse(t, out number) && number > 1, out errorMessage))
            {
                fromText.Text = string.Empty;
              //  fromText.BackgroundColor = StaticResources.ErrorBackColor;
                fromText.Placeholder = errorMessage;
                fromText.PlaceholderColor = Color.Red;

                return false;
            }
            else
            {
                fromText.Placeholder = "From";
               // fromText.BackgroundColor = Color.White;
                fromText.PlaceholderColor = Color.Black;

                return true;
            }
        }

        public bool ValidateToData(Entry entry)
        {
            int number;
            string errorMessage;
            if (!ValidateData(toText, t => Int32.TryParse(t, out number) && number > 1, out errorMessage))
            {
                toText.Text = string.Empty;
                //toText.BackgroundColor = StaticResources.ErrorBackColor;
                toText.Placeholder = errorMessage;
                toText.PlaceholderColor = Color.Red;

                return false;
            }
            else
            {
                //toText.BackgroundColor = Color.White;
                toText.Placeholder = "From";
                toText.PlaceholderColor = Color.Black;

                return true;
            }
        }
    }
}