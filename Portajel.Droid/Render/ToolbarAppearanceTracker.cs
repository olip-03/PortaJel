using Android.Graphics.Drawables;
using Android.Views;
using Microsoft.Maui.Controls.Platform;
using Microsoft.Maui.Controls.Platform.Compatibility;
using Microsoft.Maui.Devices;
using Microsoft.Maui.Platform;
using Portajel.Pages.Views;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Google.Android.Material.AppBar;
using Portajel.Render;

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
            
            if (toolbar.Resources == null) return;
            var layoutParams = toolbar.LayoutParameters;

            if(String.IsNullOrWhiteSpace(toolbar.Title))
            {
                // If toolbar is not visible, make sure to reset any margins
                if (layoutParams is ViewGroup.MarginLayoutParams nullLayoutParams)
                {
                    nullLayoutParams.TopMargin = 0;
                    toolbar.LayoutParameters = nullLayoutParams;
                }

                return;
            }
            
            var sbId = toolbar.Resources.GetIdentifier("status_bar_height", "dimen", "android");
            var statusBar = toolbar.Resources.GetDimensionPixelSize(sbId);
            var density = DeviceDisplay.MainDisplayInfo.Density;
            var mauiHeight = statusBar / density;
            UpdateStyle(mauiHeight);
            if (layoutParams is not ViewGroup.MarginLayoutParams marginLayoutParams) return;
            marginLayoutParams.TopMargin = statusBar;
            toolbar.LayoutParameters = marginLayoutParams;

            var container = toolbar.Parent;
            if (container is AppBarLayout appBarLayout)
            {
                appBarLayout.Background = BackgroundHelper.GetColor("BackgroundColor");
            }
        }

        private void UpdateStyle(double value)
        {
            ICollection<ResourceDictionary> mergedDictionaries = Application.Current.Resources.MergedDictionaries;
            if (mergedDictionaries != null)
            {
                foreach(ResourceDictionary dictionaries in mergedDictionaries)
                {
                    var heightFound = dictionaries.TryGetValue("SystemHeaderHeight", out var height);
                    if (heightFound)
                        dictionaries["SystemHeaderHeight"] = value; 
                }
            }
        }
    }
}
