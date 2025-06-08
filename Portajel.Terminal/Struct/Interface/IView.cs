using System;

namespace Portajel.Terminal.Struct.Interface
{
    public interface IView
    {
        public bool ShowTitle { get; set; }
        public string[] Title { get; }
        public string[] Contents { get; }
        public bool FormSubmitted { get; set; }
        public List<FormItem> Form { get; }
        public Dictionary<string, Action?> Selections { get; }
        public int Selected { get; set; }
    }
}
