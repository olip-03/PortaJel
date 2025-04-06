using Portajel.Connections.Interfaces;
using Portajel.Connections.Services.Jellyfin;
using System.Text.Json.Serialization;
using System.Text.Json;

public class MediaServerConnectorConverter : JsonConverter<IMediaServerConnector>
{
    public override IMediaServerConnector Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        // Correctly parse the JSON document
        using JsonDocument document = JsonDocument.ParseValue(ref reader);
        var rootElement = document.RootElement;

        // Check the Name property
        if (!rootElement.TryGetProperty("Name", out JsonElement nameProperty))
        {
            throw new JsonException("JSON missing Name property");
        }

        string name = nameProperty.GetString();

        // Convert the element back to JSON
        string jsonString = JsonSerializer.Serialize(rootElement);

        // Create a new options object without our converter to avoid infinite recursion
        var newOptions = new JsonSerializerOptions(options);
        //.newOptions.Converters.RemoveAll(c => c is MediaServerConnectorConverter);

        // Return the correct connector type
        return name switch
        {
            "JellyFin" => JsonSerializer.Deserialize<JellyfinServerConnector>(jsonString, newOptions),
            _ => throw new JsonException($"Unknown connector type: {name}")
        };
    }

    public override void Write(Utf8JsonWriter writer, IMediaServerConnector value, JsonSerializerOptions options)
    {
        JsonSerializer.Serialize(writer, value, value.GetType(), options);
    }
}
