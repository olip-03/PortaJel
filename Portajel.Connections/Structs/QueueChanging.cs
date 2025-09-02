using System.ComponentModel;
using Portajel.Connections.Database;

namespace Portajel.Connections.Structs;

public enum QueueChangeKind { Add, AddRange, Remove, RemoveRange, SetCollection, ClearCollection }

public class QueueChangingEventArgs(
    QueueChangeKind kind,
    IEnumerable<SongData> songs,
    int index = -1,
    int toIndex = -1)
    : CancelEventArgs
{
    public QueueChangeKind Kind { get; } = kind;
    public IReadOnlyList<SongData> Songs { get; } = songs?.ToArray() ?? [];
    public int Index { get; } = index;
    public int ToIndex { get; } = toIndex;
}

public class QueueChangedEventArgs(
    QueueChangeKind kind,
    IEnumerable<SongData> songs,
    int index = -1,
    int toIndex = -1)
    : EventArgs
{
    public QueueChangeKind Kind { get; } = kind;
    public IReadOnlyList<SongData> Songs { get; } = songs?.ToArray() ?? [];
    public int Index { get; } = index;
    public int ToIndex { get; } = toIndex;
}

public interface IQueueEventSource
{
    event EventHandler<QueueChangedEventArgs> QueueChanged;
}