using System.Collections.ObjectModel;
using Portajel.Connections.Interfaces;

namespace Portajel.Structures.ViewModels.Settings;

public class HomeSettingsViewModel
{
    public ObservableCollection<IMediaFeed> Feeds { get; set; } = [];
}