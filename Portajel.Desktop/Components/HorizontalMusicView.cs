using System.Collections.ObjectModel;
using System.Reactive.Disposables;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using ReactiveUI;
using Portajel.Connections.Database;
using Portajel.Desktop.Structures.ViewModel.Components;
using Portajel.Desktop.Structures.ViewModel.Music;

namespace Portajel.Desktop.Components
{
    public partial class HorizontalMusicView
        : ReactiveUserControl<HorizontalMusicViewModel>
    {
        private ListBox? _listBox;
        public HorizontalMusicView()
        {
            InitializeComponent();

            this.WhenActivated(disposables =>
            {
                _listBox = this.FindControl<ListBox>("DataView");
            });
        }

        private void InitializeComponent()
            => AvaloniaXamlLoader.Load(this);

        private void InputElement_OnPointerPressed(object? sender, PointerPressedEventArgs e)
        {
            if (sender is StackPanel container)
            {
                switch (container.DataContext)
                {
                    case AlbumData album:
                        Program.Router.Navigate.Execute(new AlbumViewModel(ViewModel.HostScreen, album));
                        break;
                    case ArtistData:
                        break;
                }
            }
        }
    }
}