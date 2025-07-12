using System.Collections.ObjectModel;
using System.Reactive;
using ReactiveUI;
using Avalonia.Controls;
using CommunityToolkit.Mvvm.DependencyInjection;
using DynamicData;
using Portajel.Connections;
using Portajel.Desktop.Components;
using Portajel.Desktop.Components.SettingsPanelViews;
using Portajel.Desktop.Structures.ViewModel.Settings;        

namespace Portajel.Desktop.Structures.ViewModel
{
    public class SettingsViewModel : ReactiveObject, IRoutableViewModel
    {
        private ServerConnector _server = Ioc.Default.GetService<ServerConnector>();
        public SettingsConnectionViewModel ConnectionViewModel { get; }
        public AddConnectionViewModel AddConnectionViewModel { get; } 
        public ObservableCollection<SettingsCategory> SettingsCategories { get; }
        private UserControl _currentView;
        public UserControl CurrentView
        {
            get => _currentView;
            set => this.RaiseAndSetIfChanged(ref _currentView, value);
        }
        public ReactiveCommand<Unit, Unit> NavigateBackCommand { get; }
        public SettingsViewModel(IScreen screen)
        {
            HostScreen = screen;
            
            NavigateBackCommand = ReactiveCommand.Create(NavigateBack);
            ConnectionViewModel = new SettingsConnectionViewModel(ReactiveCommand.Create(() => NavigateToCategory("AddConnection")));
            AddConnectionViewModel = new(NavigateBackCommand);
            
            // Start with the index view
        }
        public void NavigateToCategory(string categoryName)
        {

        }
        public void NavigateBack()
        {
            ConnectionViewModel.Connections.Clear();
            ConnectionViewModel.Connections.AddRange(_server.Servers);
        }

        public string? UrlPathSegment { get; }
        public IScreen HostScreen { get; }
    }

    public class SettingsCategory
    {
        public SettingsCategory(string name, string description)
        {
            Name = name;
            Description = description;
        }

        public string Name { get; }
        public string Description { get; }
    }
}