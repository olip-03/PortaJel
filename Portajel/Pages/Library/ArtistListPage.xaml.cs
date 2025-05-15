using FFImageLoading;
using FFImageLoading.Config;
using Portajel.Connections.Interfaces;
using Portajel.Structures.ViewModels.Pages.Library;

namespace Portajel.Pages.Library
{
    public partial class ArtistListPage : ContentPage
    {
        private BaseLibraryPage baseLibraryPage = new();
        public ArtistListPage(IDbConnector database)
        {
            InitializeComponent();
            baseLibraryPage.InitializeLibrary(this, database);
        }
    }
}

