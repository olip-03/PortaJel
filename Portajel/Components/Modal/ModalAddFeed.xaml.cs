using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Portajel.Connections.Interfaces;
using Portajel.Connections.Structs;

namespace Portajel.Components.Modal;

public partial class ModalAddFeed : ContentPage
{
    public Action<IMediaFeed> OnLoginSuccess { get; set; }
    public ModalAddFeed(IServerConnector primaryConnector, BaseMediaFeed server)
    {
        InitializeComponent();
        BindingContext = server;
    }

    private async void Cancel_OnClicked(object? sender, EventArgs e)
    {
        await Navigation.PopModalAsync();
    }

    private async void Confirm_OnClicked(object? sender, EventArgs e)
    {
        await Navigation.PopModalAsync();
    }
}