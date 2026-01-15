using System;
using System.Threading;

namespace RougeLiteGame.logger;

public sealed class AsyncWorker : IDisposable
{
    private readonly ConsoleLogger _logger = new(typeof(AsyncWorker));
    
    private readonly Thread _thread;
    private readonly AutoResetEvent _signal = new(false);

    private volatile bool _running = true;
    private readonly TimeSpan _interval;

    public AsyncWorker(TimeSpan interval)
    {
        _interval = interval;

        _thread = new Thread(WorkerLoop)
        {
            IsBackground = true,
            Name = "Log-Thread"
        };
        _thread.Start();
    }
    
    private void WorkerLoop()
    {
        _logger.Info("The system has started the asynchronous Logger-Thread");
        _logger.Flush();
        
        while (_running)
        {
            _signal.WaitOne(_interval);

            try
            {
                LoggerFactory.GlobalFlush();
            }
            catch (Exception exception)
            {
                _logger.Error("Could not globally flush logger.", exception);
                _logger.Flush();
            }
        }
    }
    
    public void Dispose()
    {
        _running = false;
        _signal.Set();
        _thread.Join();
    }
}