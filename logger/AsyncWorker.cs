using System;
using System.Collections.Concurrent;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Godot;
using RougeLiteGame.logger.console;

namespace RougeLiteGame.logger;

public sealed class AsyncWorker : IDisposable
{
    private readonly ConsoleLogger _logger = new(typeof(AsyncWorker));
    
    private readonly ConcurrentQueue<LogEntry[]> _queue = new();
    private readonly SemaphoreSlim _signal = new(0);
    private readonly CancellationTokenSource _cts = new();

    private readonly Task _workerTask;

    public AsyncWorker()
    {
        _workerTask = Task.Run(WorkerLoop);
    }

    public void Dispose()
    {
        Stop();
        _workerTask.Wait();
        _signal.Dispose();
        _cts.Dispose();
    }

    public void Enqueue(LogEntry[] command)
    {
        _queue.Enqueue(command);
        _signal.Release();
    }
    
    public void Stop()
    {
        _cts.Cancel();
        _signal.Release(); // wake worker
    }
    
    private async void WorkerLoop()
    {
        try
        {
            _logger.Info("The worker has started running asynchronously.");
            _logger.Flush();

            while (true)
            {
                await _signal.WaitAsync(_cts.Token);
                
                while (_queue.TryDequeue(out LogEntry[] entries))
                {
                    ConsoleStream.Output(entries);   
                }
            }
        }
        catch (OperationCanceledException)
        {
            // task is being canceled
        }
        catch (Exception exception)
        {
            _logger.Error("An error occurred when running the logger thread.", exception);
        }
        finally
        {
            _logger.Info("The worker has been stopped.");
            _logger.Flush();
        }
    }
}