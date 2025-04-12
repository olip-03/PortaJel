namespace Portajel.Components;

public class MusicListItem : ContentView
{
	public MusicListItemSize Size = MusicListItemSize.Medium;
    public MusicListItem()
	{
		Content = new Grid
		{
            HeightRequest = 100,
            Children = {
				new Button
				{

				}
			}
		};
	}
}

/// <summary>
/// Enum to define the size of the MusicListItem.
/// Small: Small size, for showing songs in an album or a playlist
/// Medium: Medium size, for showing music items in the library or search page
/// GridSmall: For 3x3 grids in the library or search page
/// GridLarge: For 2x2 grids in the library or search page
/// </summary>
public enum MusicListItemSize
{
    Small,
    Medium,
    GridSmall,
	GridLarge
}