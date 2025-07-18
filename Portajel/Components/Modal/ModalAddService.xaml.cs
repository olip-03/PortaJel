using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Shapes;
using Microsoft.Maui.Handlers;
using Portajel.Connections;
using Portajel.Connections.Interfaces;
using Portajel.Connections.Services;
using Portajel.Structures.Functional;
using System.Collections.ObjectModel;
using System.Data.Common;
using System.Diagnostics;

namespace Portajel.Components.Modal;

public partial class ModalAddServer : ContentPage
{
    private IServerConnector _serverConnector;
    private IMediaServerConnector _server;
    private ObservableCollection<ConnectorPropertyValue> ConnectionItems { get; set; } = new();
    public Action<IMediaServerConnector> OnLoginSuccess { get; set; }
    public ModalAddServer(IServerConnector primaryConnector, IMediaServerConnector server)
    {
        _serverConnector  = primaryConnector;
        _server = server;
        InitializeComponent();
        foreach (var item in server.Properties)
        {
            if (!item.Value.UserVisisble) continue;
            ConnectionItems.Add(item.Value);
        }
        ViewCollections.ItemsSource = ConnectionItems;
    }

    private async void BtnCancelClicked(object sender, EventArgs e)
    {
        await Navigation.PopModalAsync();
    }

    private async void BtnConfirmClicked(object sender, EventArgs e)
    {
        ViewCollectionActivityIndicator.IsVisible = true;
        ViewCollections.IsEnabled = false;
        for (int i = 0; i < _server.Properties.Count; i++)
        {
            var connectorProperty = ConnectionItems.FirstOrDefault(c => c.Label == _server.Properties.ElementAt(i).Value.Label);
            if (connectorProperty == null) continue;
            _server.Properties.ElementAt(i).Value.Value = (string)connectorProperty.Value;
        }
        _server.Properties["AppName"].Value = AppInfo.Name;
        _server.Properties["AppVersion"].Value = AppInfo.VersionString;
        _server.Properties["DeviceName"].Value = DeviceInfo.Name;
        _server.Properties["DeviceID"].Value = DeviceInfo.Current.Idiom.ToString();
        AuthStatusInfo statusInfo = await _server.AuthenticateAsync();
        if (statusInfo.State == AuthState.Success )
        {
            _serverConnector.AddServer(_server);
            _serverConnector.Feeds.Refresh();
            await SaveHelper.SaveData(_serverConnector);
            await Navigation.PopModalAsync();
            OnLoginSuccess.Invoke(_server);
            _ = Task.Run(async () =>
            {
                try
                {
                    await _server.StartSyncAsync();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Startup error: {ex.Message}");
                }
            });
            
        }
        ViewCollectionActivityIndicator.IsVisible = false;
        ViewCollections.IsEnabled = true;
    }

    private void Entry_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (sender is not Entry entry) return;
        var connectorProperty = ConnectionItems.FirstOrDefault(c => c == entry.BindingContext);
        if (connectorProperty != null)
        {
            connectorProperty.Value = e.NewTextValue;
        }
    }
}
