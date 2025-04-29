using Portajel.Connections.Interfaces;
using Portajel.Structures.ViewModels.Pages.Library;

namespace Portajel.Pages.Library;

public partial class AlbumListPage : ContentPage
{
    private AlbumListViewModel _vm;
	public AlbumListPage(IDbConnector database)
	{
        _vm = new(database);
        InitializeComponent();
        BindingContext = _vm;
    }
}