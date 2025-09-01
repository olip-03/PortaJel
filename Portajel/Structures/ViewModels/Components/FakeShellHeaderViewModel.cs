using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Portajel.Structures.ViewModels.Components;

public class FakeShellHeaderViewModel : INotifyPropertyChanged
{
    double _systemHeaderPadding;
    public double SystemHeaderPadding
    {
        get => _systemHeaderPadding;
        set
        {
            if (_systemHeaderPadding == value) return;
            _systemHeaderPadding = value;
            OnPropertyChanged();
        }
    }

    public string LabelText { get; set; } 
    
    public event PropertyChangedEventHandler? PropertyChanged;
    void OnPropertyChanged([CallerMemberName] string? name = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}