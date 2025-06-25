using System.Collections.ObjectModel;
using System.Reactive.Disposables;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using ReactiveUI;
using Portajel.Connections.Database;
using Portajel.Desktop.Structures.ViewModel.Components;

namespace Portajel.Desktop.Components
{
    public partial class HorizontalMusicView
        : ReactiveUserControl<HorizontalMusicViewModel>
    {
        public HorizontalMusicView()
        {
            InitializeComponent();

            this.WhenActivated(disposables =>
            {
                
            });
        }

        private void InitializeComponent()
            => AvaloniaXamlLoader.Load(this);
    }
}