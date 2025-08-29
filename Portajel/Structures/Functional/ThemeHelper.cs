namespace Portajel.Structures.Functional;

public static class ThemeHelper
{
    public static void UpdatePrimaryColor(string themeName)
    {
        Preferences.Set("Theme", themeName); 

        ICollection<ResourceDictionary> mergedDictionaries = Application.Current.Resources.MergedDictionaries;
        if (mergedDictionaries != null)
        {
            foreach(ResourceDictionary dictionaries in mergedDictionaries)
            {
                var primaryFound = dictionaries.TryGetValue(themeName + "Primary", out var primary);
                if (primaryFound)
                    dictionaries["Primary"] = primary; 

                var secondaryFound = dictionaries.TryGetValue(themeName + "Secondary", out var secondary);
                if (secondaryFound)
                    dictionaries["Secondary"] = secondary; 

                var tertiaryFound = dictionaries.TryGetValue(themeName + "Tertiary", out var tertiary);
                if (tertiaryFound)
                    dictionaries["Tertiary"] = tertiary; 

                var backgroundFound = dictionaries.TryGetValue(themeName + "BackgroundColor", out var background);
                if (backgroundFound)
                    dictionaries["BackgroundColor"] = background; 
                
                var tertiaryBackgroundFound = dictionaries.TryGetValue(themeName + "TertiaryBackground", out var tertiaryBackground);
                if (tertiaryBackgroundFound)
                    dictionaries["TertiaryBackground"] = tertiaryBackground; 
                
                var primaryTextFound = dictionaries.TryGetValue(themeName + "PrimaryTextColor", out var primaryText);
                if (primaryTextFound)
                    dictionaries["PrimaryTextColor"] = primaryText; 
                
                var secondaryTextFound = dictionaries.TryGetValue(themeName + "SecondaryTextColor", out var secondaryText);
                if (secondaryTextFound)
                    dictionaries["SecondaryTextColor"] = secondaryText; 
            }
        }
    }
}