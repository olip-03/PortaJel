using Android.Content;
using Microsoft.Maui.Controls.Compatibility;
using Microsoft.Maui.Controls.Handlers.Compatibility;
using Microsoft.Maui.Controls.Platform.Compatibility;
using Portajel;
using Portajel.Components;
using Portajel.Droid.Render;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// https://vladislavantonyuk.github.io/articles/Customizing-.NET-MAUI-Shell/
// [assembly: ExportRenderer(typeof(AppShell), typeof(NavShellRenderer))]
namespace Portajel.Droid.Render
{
    public class NavShellRenderer : ShellRenderer
    {
        public NavShellRenderer(Context context) : base(context)
        {
        }

        protected override IShellBottomNavViewAppearanceTracker CreateBottomNavViewAppearanceTracker(ShellItem shellItem)
        {
            return new BottomNavViewAppearanceTracker(this, shellItem.CurrentItem);
        }

        protected override IShellToolbarAppearanceTracker CreateToolbarAppearanceTracker()
        {
            return new ToolbarAppearanceTracker(this);
        }
    }
}
