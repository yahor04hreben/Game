using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using Xamarin.Essentials;

namespace MathOperation.Common
{
    public static class StaticResources
    {
        public static Color CellColor => Color.FromArgb(43, 43, 43);
        public static Color CellColorAfterPressed => Color.FromArgb(102, 102, 102);

        public static Color AddCellUnClickableColor => Color.FromArgb(150, 58, 0);

        public static int CellTextSize => 40;

        public static int GoalTextSize => 60;

        public static Color GoalBackgroundColor => Color.FromArgb(247, 95, 30);
        public static int MarginGoal => 5;
        public static int RadiusGoal => 10;

        public static Color ColorGoalText => Color.White;

        public static Color ErrorBackColor => Color.FromArgb(5, 130, 0, 0);

        public static float Width => (float)(DeviceDisplay.MainDisplayInfo.Width / DeviceDisplay.MainDisplayInfo.Density);
        public static float Height => (float)(DeviceDisplay.MainDisplayInfo.Height / DeviceDisplay.MainDisplayInfo.Density);

        public static int index = 0;
    }
}
