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
        Frame frame = null;
        private string From = String.Empty;
        private string To = string.Empty;

        private Entry fromText;
        private Entry toText;
        public SetNumbersPage()
        {

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
            var majorLayout = new StackLayout
            {
                Orientation = StackOrientation.Vertical,
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.Center
            };

            var boxWidth = StaticResources.Height * 0.25;
            var boxHeight = StaticResources.Height * 0.5 * 0.3;

            var layout1 = new StackLayout
            {
                Orientation = StackOrientation.Vertical,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center,
                HeightRequest = boxHeight,
                WidthRequest = boxWidth
            };

            var layout2 = new StackLayout
            {
                Orientation = StackOrientation.Vertical,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center,
                HeightRequest = boxHeight,
                WidthRequest = boxWidth
            };

            var fromLabel = new Label { Text = "From:", FontSize = StaticResources.GoalTextSize * 0.7, WidthRequest = boxWidth };
            fromText = new Entry { WidthRequest = boxWidth * 0.9, BackgroundColor = Color.White};

            var toLabel = new Label { Text = "To:", FontSize = StaticResources.GoalTextSize * 0.7, WidthRequest = boxWidth };
            toText = new Entry { WidthRequest = boxWidth * 0.9, BackgroundColor = Color.White};

            layout1.Children.Add(fromLabel);
            layout1.Children.Add(fromText);

            layout2.Children.Add(toLabel);
            layout2.Children.Add(toText);

            var minorLayout = new StackLayout
            {
                Orientation = StackOrientation.Horizontal,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center,
                HeightRequest = frame.Height,
                WidthRequest = frame.Width
            };

            minorLayout.Children.Add(layout1);
            minorLayout.Children.Add(layout2);

            var OkButton = new Button
            {
                Text = "OK",
                FontSize = StaticResources.GoalTextSize * 0.6,
                BackgroundColor = Color.White
            };
            OkButton.Clicked += ClickOnOk;

            majorLayout.Children.Add(minorLayout);
            majorLayout.Children.Add(OkButton);

            frame.Content = majorLayout;
        }


        private async void ClickOnOk(object sander, EventArgs args)
        {
            await Navigation.PushModalAsync(new MainPage(fromText.Text, toText.Text));
        }
    }
}