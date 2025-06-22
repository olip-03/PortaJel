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
    public class SettingsPanelViewModel : ReactiveObject
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
        public SettingsPanelViewModel()
        {
            NavigateBackCommand = ReactiveCommand.Create(NavigateBack);
            ConnectionViewModel = new SettingsConnectionViewModel(ReactiveCommand.Create(() => NavigateToCategory("AddConnection")));
            AddConnectionViewModel = new(NavigateBackCommand);
            
            // Start with the index view
            CurrentView = new SettingsIndex { DataContext = this };
        }
        public void NavigateToCategory(string categoryName)
        {
            CurrentView = categoryName switch
            {
                "AddConnection" => new AddConnection { DataContext = AddConnectionViewModel },
                _ => new SettingsIndex { DataContext = this }
            };
        }
        public void NavigateBack()
        {
            ConnectionViewModel.Connections.Clear();
            ConnectionViewModel.Connections.AddRange(_server.Servers);
            CurrentView = new SettingsIndex { DataContext = this };
        }
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