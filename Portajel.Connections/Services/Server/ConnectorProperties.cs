namespace Portajel.Connections.Services;

public class ConnectorProperties: Dictionary<string, ConnectorPropertyValue>
{
    
}

public class ConnectorPropertyValue
{
    public ConnectorPropertyValue()
    {
    }
    public ConnectorPropertyValue(string label, string description, object value, bool protectValue, bool userVisible, string icon = "")
    {
        Label = label;
        Description = description;
        Value = value;
        ProtectValue = protectValue;
        UserVisisble = userVisible;
        Icon = icon;
    }
    public string Label { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public object Value { get; set; } = null!;
    public bool ProtectValue { get; set; }
    public bool UserVisisble { get; set; }
    public string Icon { get; set; } 
}
