using NetTopologySuite.Index.HPRtree;
using Portajel.Connections;
using Portajel.Connections.Interfaces;
using Portajel.Connections.Services.Database;
using System;
using System.Text.Json;
using System.Threading;

namespace Portajel.Pages.Settings.Debug;

public partial class DebugDatabase : ContentPage, IDisposable
{
    private PeriodicTimer _timer;
    private CancellationTokenSource _cancellationTokenSource;
    private IServerConnector _server = default!;
    private IDbConnector _database = default!;
    public DebugDatabase(IServerConnector serverConnector, IDbConnector dbConnector)
    {
        _server = serverConnector;
        _database = dbConnector;

        InitializeComponent();
    }

    protected override async void OnNavigatedTo(NavigatedToEventArgs args)
    {
        _cancellationTokenSource = new CancellationTokenSource();
        _timer = new PeriodicTimer(TimeSpan.FromSeconds(1));

        // Start the timer when page appears
        this.Unloaded += OnUnloaded;
        await StartTimerAsync();
    }

    private void OnUnloaded(object sender, EventArgs e)
    {
        StopTimer();
    }

    private async Task StartTimerAsync()
    {
        try
        {
            while (await _timer.WaitForNextTickAsync(_cancellationTokenSource.Token))
            {
                await Tick();
            }
        }
        catch (OperationCanceledException)
        {
            // Timer was canceled, this is expected
        }
    }

    private void StopTimer()
    {
        _cancellationTokenSource.Cancel();
    }

    private async Task Tick()
    {
        // Example: Update UI on main thread if needed
        await MainThread.InvokeOnMainThreadAsync(() =>
        {
            string combination = "";

            foreach (var server in _server.Servers)
            {
                foreach (var item in server.Properties)
                {
                    if (item.Value.ProtectValue)
                    {
                        combination += $"{item.Value.Label}: *****\n";
                    }
                    else
                    {
                        combination += $"{item.Value.Label}: {item.Value.Value.ToString()}\n";
                    }
                }
                combination += "\n";
                foreach (var item in server.GetDataConnectors())
                {
                    if(item.Value != null)
                    {
                        //string json = JsonSerializer.Serialize(item.Value.SyncStatusInfo, new JsonSerializerOptions { WriteIndented = true });
                        //System.Diagnostics.Debug.WriteLine(json);
                        //combination += item.Key + "\n";
                        //combination += json + "\n";
                        combination += $"{item.Key}\n";
                        combination += $"Status: {GetFriendlyTaskStatus(item.Value.SyncStatusInfo.TaskStatus)}\n";
                        combination += $"Completion: {item.Value.SyncStatusInfo.StatusPercentage}%\n";
                        combination += $"Items: {item.Value.SyncStatusInfo.ServerItemCount}/{item.Value.SyncStatusInfo.ServerItemTotal}\n";
                        combination += "\n";
                    }
                }
                combination += "------";
                combination += "\n";
            }

            ticker.Text = combination;
        });
    }

    public void Dispose()
    {
        _cancellationTokenSource?.Cancel();
        _cancellationTokenSource?.Dispose();
        _timer?.Dispose();
    }

    private static string GetFriendlyTaskStatus(TaskStatus status)
    {
        return status switch
        {
            TaskStatus.Created => "Not Started",
            TaskStatus.WaitingForActivation => "Waiting to Activate",
            TaskStatus.WaitingToRun => "Waiting to Start",
            TaskStatus.Running => "Running",
            TaskStatus.WaitingForChildrenToComplete => "Waiting for Sub-tasks",
            TaskStatus.RanToCompletion => "Completed",
            TaskStatus.Canceled => "Canceled",
            TaskStatus.Faulted => "Failed",
            _ => status.ToString()
        };
    }
}
