using Android.Graphics.Drawables;
using Android.Views;
using Microsoft.Maui.Controls.Platform;
using Microsoft.Maui.Controls.Platform.Compatibility;
using Microsoft.Maui.Platform;
using Portajel.Pages.Views;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Portajel.Droid.Render
{
    public class ToolbarAppearanceTracker : ShellToolbarAppearanceTracker
    {
        public ToolbarAppearanceTracker(IShellContext shellContext) : base(shellContext)
        {

        }

        public override void SetAppearance(AndroidX.AppCompat.Widget.Toolbar toolbar, IShellToolbarTracker toolbarTracker, ShellAppearance appearance)
        {
            base.SetAppearance(toolbar, toolbarTracker, appearance);
            
            // Unideal check, but the toolbar visibility straight up lies

            if(String.IsNullOrWhiteSpace(toolbar.Title))
            {
                // If toolbar is not visible, make sure to reset any margins
                var layoutParams = toolbar.LayoutParameters;
                if (layoutParams is ViewGroup.MarginLayoutParams marginLayoutParams)
                {
                    marginLayoutParams.TopMargin = 0;
                    toolbar.LayoutParameters = layoutParams;
                }
            }
            else
            {
                var sbId = toolbar.Resources.GetIdentifier("status_bar_height", "dimen", "android");
                var statusBar = toolbar.Resources.GetDimensionPixelSize(sbId);

                var layoutParams = toolbar.LayoutParameters;
                if (layoutParams is ViewGroup.MarginLayoutParams marginLayoutParams)
                {
                    marginLayoutParams.TopMargin = statusBar;
                    toolbar.LayoutParameters = layoutParams;
                }
            }
        }
    }
}
