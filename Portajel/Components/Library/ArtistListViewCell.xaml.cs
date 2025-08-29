using System.Diagnostics;
using Portajel.Connections.Structs;

namespace Portajel.Components.Library
{
    public partial class ArtistListViewCell : VirtualViewCell
    {
        private BaseData? _itemData;

        public ArtistListViewCell()
        {
            InitializeComponent();
        }

        protected override void OnBindingContextChanged()
        {
            // you can also put cachedImage.Source = null; here to prevent showing old images occasionally
            var item = BindingContext as BaseData;
            _itemData = item;
            if (item == null)
            {
                return;
            }
            base.OnBindingContextChanged();
        }

        private async void Button_Clicked(object sender, EventArgs e)
        {
            try
            {
                if (_itemData == null)
                {
                    return;
                }
                
                await Shell.Current.GoToAsync("artist", new Dictionary<string, object>
                {
                    { "Properties", _itemData }
                });
            }
            catch (Exception ex)
            {
                Trace.WriteLine($"{GetType().Name} Failed: {ex.Message}");
            }
        }
    }
}