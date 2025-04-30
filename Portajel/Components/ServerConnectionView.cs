using CommunityToolkit.Maui.Behaviors;
using Portajel.Connections.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Portajel.Components
{
    public class ServerConnectionView: Grid
    {
        public static readonly BindableProperty ImgBlurhashSourceProperty =
        BindableProperty.Create(nameof(ImgBlurhashSource), typeof(string), typeof(MusicListItem), default(string));
        public string ImgBlurhashSource
        {
            get => (string)GetValue(ImgBlurhashSourceProperty);
            set => SetValue(ImgBlurhashSourceProperty, value);
        }

        ServerConnectionView()
        {

        }

        protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);
        } 
    }
}
