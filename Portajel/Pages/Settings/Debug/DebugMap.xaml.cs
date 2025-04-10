using Mapsui;
using Mapsui.Extensions;
using Mapsui.Layers;
using Mapsui.Nts;
using Mapsui.Styles;
using Mapsui.Tiling;
using Mapsui.Tiling.Layers;
using Mapsui.UI.Maui;
using Mapsui.Widgets;
using Microsoft.Maui.ApplicationModel;
using NetTopologySuite.Geometries;
using Map = Mapsui.Map;

namespace Portajel.Pages.Settings.Debug;

public partial class DebugMap : ContentPage
{
    // https://mapsui.com/v5/custom-style-renders/
    public DebugMap()
	{
		InitializeComponent();

        MapControl.UseGPU = true;
        var mapControl = new MapControl();
        mapControl.Map = CreateMap(1);
        mapControl.Map.Navigator.RotationLock = true;
        Content = mapControl;
    }

    public static Map CreateMap(float pixelDensity)
    {
        var map = new Map();
        map.Layers.Add(OpenStreetMap.CreateTileLayer());
        return map;
    }

    private static ILayer CreateLineStringLayer()
    {
        return new MemoryLayer
        {
            Name = "LineString",
            Features = GetFeature()
        };
    }

    private static List<IFeature> GetFeature()
    {
        var list = new List<IFeature>();
        for (int i = 0; i < 10000; i++)
        {
            var lineString = CreateLineStringWithManyVertices();
            var feature = new GeometryFeature();
            AddStyles(feature);
            feature.Geometry = lineString;
            feature["Name"] = $"LineString with {lineString.Coordinates.Length} vertices";
            list.Add(feature);
        }

        return list;
    }

    private static LineString CreateLineStringWithManyVertices()
    {
        var startPoint = new Coordinate(1623484, 7652571);

        var points = new List<Coordinate>();

        for (var i = 0; i < 4; i++)
        {
            points.Add(new Coordinate(startPoint.X + i * 100, startPoint.Y + i * 100));
        }

        return new LineString(points.ToArray());
    }

    private static void AddStyles(IFeature feature)
    {
        //// route outline style
        //var vsout = new VectorStyle
        //{
        //    Opacity = 0.5f,
        //    Line = new Pen(System.Drawing.Color.White, 10f),
        //};

        //var vs = new VectorStyle
        //{
        //    Fill = null,
        //    Outline = null,
        //    Line = { Color = Color.Red, Width = 5f }
        //};

        //feature.Styles.Add(vsout);
        //feature.Styles.Add(vs);
    }
}