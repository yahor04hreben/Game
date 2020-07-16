using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace MathOperation.Common
{
    public static class StaticResources
    {
        public static Color CellColor => Color.FromArgb(43, 43, 43);
        public static Color CellColorAfterPressed => Color.FromArgb(102, 102, 102);

        public static Color AddCellUnClickable => Color.FromArgb(150, 58, 0);

        public static int CellTextSize => 40;

        public static int GoalTextSize => 60;

        public static Color GoalBackgroundColor => Color.FromArgb(247, 95, 30);
        public static int MarginGoal => 5;
        public static int RadiusGoal => 20;

        public static Color ColorGoalText => Color.White;
    }
}
