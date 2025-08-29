using Android.Graphics.Drawables;
using Google.Android.Material.AppBar;
using Microsoft.Maui.Platform;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Portajel.Render
{
    public static class BackgroundHelper
    {
        public static ColorDrawable GetColor(string colorKey)
        {
            // 1) Try to grab a Color or SolidColorBrush from the merged dictionaries
            var rd = Application.Current?.Resources;
            Color mauiColor = Colors.Transparent;

            if (rd != null && rd.TryGetValue(colorKey, out var brushOrColor))
            {
                if (brushOrColor is Color c)
                    mauiColor = c;
                else if (brushOrColor is SolidColorBrush scb)
                    mauiColor = scb.Color;
            }

            // 2) Convert to Android.Graphics.Color
            var androidColor = mauiColor.ToPlatform();

            // 3) Apply as a ColorDrawable
            return new ColorDrawable(androidColor);
        }
    }
}
