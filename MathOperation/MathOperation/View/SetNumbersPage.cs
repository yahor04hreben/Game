using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xamarin.Forms;

namespace MathOperation.View
{
    public class SetNumbersPage : ContentPage
    {
        public SetNumbersPage()
        {

            Frame frame = new Frame();
            frame.HorizontalOptions = LayoutOptions.Center;
            frame.WidthRequest = 300;
            frame.VerticalOptions = LayoutOptions.Center;
            frame.HeightRequest = 300;
            frame.BackgroundColor = Color.White;

            var label = new Label
            {
                Text = "Hello",
                FontSize = Device.GetNamedSize(NamedSize.Large, typeof(Label)),
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.Center,
                TextColor = Color.Black
            };

            frame.Content = label;

            Content = frame;
            BackgroundColor = Color.FromHex("AB000000");

        }
    }
}