using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Net;
using CommunityToolkit.Maui.Core.Extensions;
using Portajel.Connections.Database;
using Portajel.Connections.Interfaces;
using Portajel.Structures.ViewModels.Pages.Views;
using SQLite;
using Portajel.Connections;
namespace Portajel.Pages.Views;

public partial class ArtistPage : ContentPage, IQueryAttributable
{
	private readonly IDbConnector _database;
	private readonly ServerConnector _server;
	
	private double _screenWidth;
	private ArtistPageViewModel _viewModel = new();
	public ArtistPage(IDbConnector database, ServerConnector server)
	{
		_database = database;
		_server = server;
		var setting = this;

		var density = DeviceDisplay.Current.MainDisplayInfo.Density;
		_screenWidth = DeviceDisplay.Current.MainDisplayInfo.Width / density;
		var pxHeight = DeviceDisplay.Current.MainDisplayInfo.Height / density;
		InitializeComponent();
	}
	
	public void ApplyQueryAttributes(IDictionary<string, object> query)
	{
		var objects = query;
		foreach (var item in objects)
		{
			try
			{
				if (item.Value is ArtistData artist)
				{
					_viewModel = new ArtistPageViewModel()
					{
						// square image
						BackgroundHeight = _screenWidth
					};
					_viewModel.Update([], artist);
				}
				BindingContext = _viewModel;
			}
			catch
			{
				return;
			}
		}
	}

	protected override void OnNavigatedTo(NavigatedToEventArgs args)
	{
		Init();
		base.OnNavigatedTo(args);
	}

	private async void Init()
	{
		try
		{
			await Update();
		}
		catch (Exception e)
		{
			Trace.WriteLine(e);
		}
	}

	private async void RefreshView_OnRefreshing(object? sender, EventArgs e)
	{
		try
		{
			await Update();
			if (sender is RefreshView refreshView)
			{
				refreshView.IsRefreshing = false;
			}
		}
		catch (Exception ex)
		{
			Trace.WriteLine(ex);
		}
	}
	
	private async Task Update(bool retry = true)
	{
		try
		{
			if (_database.Connectors.Album is TableQuery<AlbumData> albumTable)
			{
				var albums = albumTable.Where(a => a.ParentId == _viewModel.Id).ToArray();
				_viewModel.Update(albums, null);
			}
			
		    var upatedData = await Download();
			
			_viewModel.Update(upatedData.Item2, upatedData.Item1);
			BindingContext = _viewModel;

			// Todo: create insert or replace functions for database
			// _database.Connectors.Album.Insert(album);
			// _database.Connectors.Song.InsertRange(songs);
		}
		catch (HttpRequestException authEx)
		{
			var server = _server[_viewModel.ServerAddress];
			if (authEx.StatusCode == HttpStatusCode.Unauthorized && retry && server != null)
			{
				await server.AuthenticateAsync();
				await Update(false);
			}
		}
		catch (Exception e)
		{
			Trace.WriteLine(e);
		}
	}

	private async Task<(ArtistData?, AlbumData[])> Download()
	{
		var server = _server[_viewModel.ServerAddress];
		if (server != null)
		{
			var id = _viewModel.ServerId;

			var artistTask = server.DataConnectors["Artist"].GetAsync(id);
			var albumTask = server.DataConnectors["Album"].GetAllAsync(parentId: id);
			await Task.WhenAll(artistTask, albumTask);

			return (artistTask.Result.ToArtist(), albumTask.Result.Cast<AlbumData>().ToArray());
		}
		return (null, []);
	}

	private async void AlbumButton_OnClicked(object? sender, EventArgs e)
	{
		if (sender is not Button button) return;
		if (button.BindingContext is not AlbumData album) return;
		await Shell.Current.GoToAsync("album", new Dictionary<string, object>
		{
			{ "Properties", album }
		});
	}
	private async void Header_OnBackButtonClicked(object? sender, EventArgs e)
	{
		try
		{
			await Shell.Current.GoToAsync("..", true);
		}
		catch (Exception ex)
		{
			Trace.WriteLine(ex.Message);
		}
	}
}