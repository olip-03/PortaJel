using Portajel.Connections.Interfaces;

namespace Portajel.Pages;

public partial class SearchPage : ContentPage
{
	private IDbConnector _database;
	public SearchPage(IDbConnector db)
	{
		_database = db;
        InitializeComponent();
	}
}