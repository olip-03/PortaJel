using Android.Graphics.Drawables;
using Android.Views;
using AndroidX.Core.View;
using CommunityToolkit.Maui.Core.Platform;
using Google.Android.Material.BottomNavigation;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Platform;
using Microsoft.Maui.Controls.Platform.Compatibility;
using Microsoft.Maui.Platform;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Portajel.Droid.Render
{
    public class BottomNavViewAppearanceTracker : ShellBottomNavViewAppearanceTracker
    {
        private readonly IShellContext shellContext;
        public BottomNavViewAppearanceTracker(IShellContext shellContext, ShellItem shellItem) : base(shellContext, shellItem)
        {
            this.shellContext = shellContext;
        }
        public override void SetAppearance(BottomNavigationView bottomView, IShellAppearanceElement appearance)
        {
            base.SetAppearance(bottomView, appearance);

            // var windowInsets = new WindowInsetsHandler();
          
            var nbId = bottomView.Resources.GetIdentifier("navigation_bar_height", "dimen", "android");
            var navBar = bottomView.Resources.GetDimensionPixelSize(nbId);

            var layoutParams = bottomView.LayoutParameters;
            if (layoutParams is ViewGroup.MarginLayoutParams marginLayoutParams)
            {
                marginLayoutParams.BottomMargin = navBar;
                marginLayoutParams.LeftMargin = 0;
                marginLayoutParams.RightMargin = 0;
                bottomView.LayoutParameters = layoutParams;
            }
        }
    }
}
