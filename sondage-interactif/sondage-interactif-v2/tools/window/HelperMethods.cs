using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace SondageInteractifv2.tools.window
{
    class HelperMethods
    {
        public static DependencyObject GetScrollViewer(DependencyObject o)
        {
            // Return the DependencyObject if it is a ScrollViewer
            if (o is ScrollViewer)
            { return o; }

            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(o); i++)
            {
                var child = VisualTreeHelper.GetChild(o, i);

                var result = GetScrollViewer(child);
                if (result == null)
                {
                    continue;
                }
                return result;
            }
            return null;
        }

        public static bool PointOnButton(Point point, Button button, Grid containinGrid)
        {
            var p = button.TranslatePoint(new Point(), containinGrid);

            return (p.X <= point.X * containinGrid.ActualWidth) &&
                   ((p.X + button.ActualWidth) >= point.X * containinGrid.ActualWidth) &&
                   ((p.Y + button.ActualHeight) >= point.Y * containinGrid.ActualHeight) &&
                   (p.Y <= point.Y * containinGrid.ActualHeight);
        }

        public static void CenterWindowOnScreen(Window Window)
        {
            double screenWidth = System.Windows.SystemParameters.PrimaryScreenWidth;
            double screenHeight = System.Windows.SystemParameters.PrimaryScreenHeight;
            double windowWidth = Window.Width;
            double windowHeight = Window.Height;
            Window.Left = (screenWidth / 2) - (windowWidth / 2);
            Window.Top = (screenHeight / 2) - (windowHeight / 2);
        }
    }

    public static class ExtensionMethods
    {
        public static string ToFormattedString(this PXCMPoint3DF32 pos)
        {
            return String.Format("{0:###0.000}, {1:###0.000}, {2:###0.000}", pos.x, pos.y, pos.z);
        }
    }
}
