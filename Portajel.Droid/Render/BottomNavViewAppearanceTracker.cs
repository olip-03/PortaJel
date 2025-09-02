using Android.Graphics.Drawables;
using Android.OS;
using Android.Views;
using Android.Widget;
using AndroidX.Core.View;
using CommunityToolkit.Maui.Core.Platform;
using Google.Android.Material.AppBar;
using Google.Android.Material.BottomNavigation;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Embedding;
using Microsoft.Maui.Controls.Platform;
using Microsoft.Maui.Controls.Platform.Compatibility;
using Microsoft.Maui.Platform;
using Portajel.Components.Media;
using Portajel.Render;
using Portajel.Structures.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using View = Android.Views.View;

namespace Portajel.Droid.Render
{
    public class BottomNavViewAppearanceTracker : ShellBottomNavViewAppearanceTracker
    {
        private readonly IShellContext shellContext;
        private readonly MiniPlayer miniPlayer;

        public BottomNavViewAppearanceTracker(IShellContext shellContext, 
                                              ShellItem shellItem)
            : base(shellContext, shellItem)
        {
            this.shellContext = shellContext;

            var mediaService = Application.Current?.Handler.GetServiceProvider().GetService<IMediaController>(); ;
            miniPlayer = new(mediaService);
        }
        
        public override void SetAppearance(
            BottomNavigationView bottomView, 
            IShellAppearanceElement appearance)
        {
            base.SetAppearance(bottomView, appearance);

            UpdateStyle(bottomView);
            UpdateBottomView(bottomView);
        }

        private void UpdateStyle(BottomNavigationView bottomView)
        {
            var container = bottomView.Parent;
            if (container is LinearLayout appBarLayout)
            {
                appBarLayout.Background = BackgroundHelper.GetColor("TertiaryBackground");
            }

            if (bottomView?.Resources?.DisplayMetrics == null)
                return;
            if (Build.VERSION.SdkInt >= BuildVersionCodes.Lollipop)
            {
                var d = bottomView.Resources.DisplayMetrics.Density;
                float elevation = 8f * d;  // 8 dp
                bottomView.Elevation = elevation;
                // also kill the state-list animator so it never “lifts” or “drops” on press
                bottomView.StateListAnimator = null;
            }
        }

        private View RefreshMiniplayer()
        {
            var appHandler = Application.Current?.Handler;
            string exception = "Can't get MiniPlayer without Application.Current.Handler.MauiContext";
            return miniPlayer.ToPlatformEmbedded(appHandler?.MauiContext ?? throw new Exception(exception));
        } 

        private void UpdateBottomView(BottomNavigationView bottomView)
        {
            if (bottomView.Resources == null)
                return;
            var nbId = bottomView.Resources.GetIdentifier("navigation_bar_height", 
                "dimen", "android");
            var navBar = bottomView.Resources.GetDimensionPixelSize(nbId);
            var layoutParams = bottomView.LayoutParameters;
            if (layoutParams is ViewGroup.MarginLayoutParams marginLayoutParams)
            {
                marginLayoutParams.BottomMargin = navBar;
                marginLayoutParams.LeftMargin = 0;
                marginLayoutParams.RightMargin = 0;
                bottomView.LayoutParameters = layoutParams;
            }
            
            bottomView.Post(() =>
            {
                if (!(bottomView.Parent is ViewGroup parent))
                    return;
                
                for (int i = 0; i < parent.ChildCount; i++)
                {
                    var child = parent.GetChildAt(i);
                    if (child?.Tag != null && child.Tag.ToString() == "MiniPlayer")
                        return; 
                }
                
                var native = RefreshMiniplayer();
                native.Tag = "MiniPlayer";
                native.Background = bottomView.Background;
                
                int bottomViewIndex = parent.IndexOfChild(bottomView);
                parent.AddView(native, bottomViewIndex);
            });
        }
    }
}