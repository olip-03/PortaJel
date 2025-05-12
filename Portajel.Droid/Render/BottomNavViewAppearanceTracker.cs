using Google.Android.Material.BottomNavigation;
using Microsoft.Maui.Controls.Platform.Compatibility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Portajel.Droid.Render
{
    public class BottomNavViewAppearanceTracker : ShellBottomNavViewAppearanceTracker
    {
        public BottomNavViewAppearanceTracker(IShellContext shellContext, ShellItem shellItem)
            : base(shellContext, shellItem)
        {
        }
        public override void SetAppearance(BottomNavigationView bottomView, IShellAppearanceElement appearance)
        {
            bottomView.LayoutParameters.Height = 400;
            bottomView.SetBackgroundColor(Android.Graphics.Color.Red);
            bottomView.SetPadding(0, 0, 0, 48);
            base.SetAppearance(bottomView, appearance);
        }
    }
}
