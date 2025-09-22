using Portajel.Connections.Database;
using Portajel.Structures.ViewModels.Components;

namespace Portajel.Components.Media;

public partial class ModalPlayer : ContentPage
{
	public EventHandler OnClose;
	public MediaPlayerViewModel ViewModel = new();
	
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
			ViewModel.Current.Name = current.Name;
			ViewModel.Current.ArtistNames = current.ArtistNames;
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
}