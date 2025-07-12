using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FFImageLoading.Maui;
using Portajel.Connections.Database;
using Microsoft.Maui;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Xaml;
using NetTopologySuite.Index.HPRtree;
using Portajel.Structures;
using Portajel.Connections;
using Jellyfin.Sdk.Generated.Models;
using Portajel.Connections.Structs;
using Sharpnado.Tabs;
using CommunityToolkit.Maui.Views;

namespace Portajel.Components.Library
{
    public partial class SongViewCell : VirtualViewCell
    {
        private BaseData ItemData = null;

        public SongViewCell()
        {
            InitializeComponent();
        }

        protected override void OnBindingContextChanged()
        {
            // you can also put cachedImage.Source = null; here to prevent showing old images occasionally
            var item = BindingContext as BaseData;
            ItemData = item;
            if (item == null)
            {
                return;
            }
            base.OnBindingContextChanged();
        }

        private async void Button_Clicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("album", new Dictionary<string, object>
            {
                    { "Properties", ItemData }
                });
        }
    }
}