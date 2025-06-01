namespace Portajel.Terminal.Struct;

public static class TxtHelper
{
    public static string[] Box(string text)
    {
        int padding = 2;
        int boxWidth = text.Length + (padding * 2);
        string horizontalBorder = new string('#', boxWidth + 2);
        string spaces = new string(' ', padding);

        return [
            " " + horizontalBorder,
            " #" + spaces + text + spaces + "#",
            " " + horizontalBorder
        ];
    }
    
    public static string[] Box(string[] text)
    {
        int padding = 2;
        int maxTextLength = text.Max(line => line.Length);
        int boxWidth = maxTextLength + (padding * 2);
        string horizontalBorder = new string('#', boxWidth + 2);
        string spaces = new string(' ', padding);

        var result = new List<string>();
        
        // Add top border
        result.Add(" " + horizontalBorder);
        
        // Add content lines
        foreach (string line in text)
        {
            int spacesNeeded = maxTextLength - line.Length;
            string contentLine = " #" + spaces + line + 
                                 new string(' ', spacesNeeded) + spaces + "#";
            result.Add(contentLine);
        }
        
        // Add bottom border
        result.Add(" " + horizontalBorder);
        
        return result.ToArray();
    }
}