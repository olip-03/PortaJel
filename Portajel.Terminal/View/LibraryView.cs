using Portajel.Terminal.Struct;
using Portajel.Terminal.Struct.Interface;

namespace Portajel.Terminal.View;

public class LibraryView: IView
{
    public bool ShowTitle { get; set; }
    public string[] Title { get; }
    public string[] Contents { get; }
    public bool FormSubmitted { get; set; }
    public List<FormItem> Form { get; }
    public Dictionary<string, Action?> Selections { get; }
    public int Selected { get; set; }
}