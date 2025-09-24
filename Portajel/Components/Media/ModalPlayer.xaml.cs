using Portajel.Connections.Database;
using Portajel.Structures.ViewModels.Components;

namespace Portajel.Components.Media;

public partial class ModalPlayer : ContentPage
{
	public EventHandler OnClose;
	public MediaPlayerViewModel ViewModel = new();
	
	private bool _isPlaying;
	private bool _isFavourite;
	
	public ModalPlayer(MediaPlayerViewModel viewModel)
	{
		ViewModel = viewModel;
		InitializeComponent();
		BindingContext = ViewModel;
	}
	
	private void CarouselView_OnCurrentItemChanged(object? sender, CurrentItemChangedEventArgs e)
	{
		if (sender is CarouselView carouselView)
		{
			ViewModel.QueuePosition = carouselView.Position;
		}
		if (e.CurrentItem is SongData current)
		{
			UpdateView(current);
		}
	}

	protected override void OnAppearing()
	{
		SongCarousel.Position = ViewModel.QueuePosition;
		base.OnAppearing();
	}

	protected override void OnDisappearing()
	{
		OnClose.Invoke(this, EventArgs.Empty);
		base.OnDisappearing();
	}

	private void PlayPause_OnClicked(object? sender, EventArgs e)
	{
		_isPlaying = !_isPlaying;
		ViewModel.PlayPauseIcon = _isPlaying ? "media_pause.png" : "media_play.png";
	}

	private void Prev_OnClicked(object? sender, EventArgs e)
	{
		int scrollTo = SongCarousel.Position - 1;
		if (scrollTo >= 0)
		{
			SongCarousel.ScrollTo(scrollTo);
			var song = ViewModel.Queue[scrollTo];
			UpdateView(song);
		}
	}
	
	private void Next_OnClicked(object? sender, EventArgs e)
	{
		int scrollTo = SongCarousel.Position + 1;
		if (scrollTo < ViewModel.Queue.Count)
		{
			SongCarousel.ScrollTo(scrollTo);
			var song = ViewModel.Queue[scrollTo];
			UpdateView(song);
		}
	}

	private void shuffle_OnClicked(object? sender, EventArgs e)
	{
		
	}

	private void Repeat_OnClicked(object? sender, EventArgs e)
	{
		
	}

	private void Favourite_OnClick(object? sender, EventArgs e)
	{
		if (SongCarousel.CurrentItem is SongData song)
		{
			song.IsFavourite = !song.IsFavourite;
			_isFavourite = song.IsFavourite;
			UpdateView();
		}
	}

	private void UpdateView(SongData? song = null)
	{
		if (song != null)
		{
			ViewModel.Current.Name = song.Name;
			ViewModel.Current.ArtistNames = song.ArtistNames;
			ViewModel.Blurhash = song.ImgBlurhash ?? "";
			_isFavourite = song.IsFavourite;
		}
		ViewModel.FavButtonIcon = _isFavourite ? "favourite_filled.png" : "favourite_empty.png";

	}
}