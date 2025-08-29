using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Portajel.Structures.ViewModels.Components;

namespace Portajel.Components;

public partial class PlaybackControls : ContentView
{
    public event EventHandler? PlayPauseClicked;
    public event EventHandler? FavouriteClicked;
    public event EventHandler? DownloadClicked;
    public event EventHandler? LibraryClicked;
    public event EventHandler? MoreClicked;

    private bool _isPlaying;
    private readonly PlaybackControlsViewModel _viewModel = new();

    public PlaybackControls()
    {
        InitializeComponent();
    }

    private void PlayPauseButton_OnClicked(object? sender, EventArgs e)
    {
        _isPlaying = !_isPlaying;
        _viewModel.PlayPauseIcon = _isPlaying ? "media_pause.png" : "media_play.png";
        
        PlayPauseClicked?.Invoke(sender, e);
    }

    private void FavouriteButton_OnClicked(object? sender, EventArgs e)
    {
        FavouriteClicked?.Invoke(sender, e);
    }

    private void DownloadButton_OnClicked(object? sender, EventArgs e)
    {
        DownloadClicked?.Invoke(sender, e);
    }

    private void LibraryAddButton_OnClicked(object? sender, EventArgs e)
    {
        LibraryClicked?.Invoke(sender, e);
    }

    private void MoreButton_OnClicked(object? sender, EventArgs e)
    {
        MoreClicked?.Invoke(sender, e);
    }
}