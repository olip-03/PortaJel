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

namespace Portajel.Components
{
    public partial class GenericViewCell : VirtualViewCell
    {
        public GenericViewCell()
        {
            InitializeComponent();
        }

        protected override void OnBindingContextChanged()
        {
            // you can also put cachedImage.Source = null; here to prevent showing old images occasionally
            Image.Source = null;
            var item = BindingContext as AlbumData;

            if (item == null)
            {
                return;
            }
            Image.Source = item.ImgSource;
            base.OnBindingContextChanged();
        }
    }
}