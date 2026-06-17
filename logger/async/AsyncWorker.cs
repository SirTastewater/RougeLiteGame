using System;
using System.Threading;

namespace RougeLiteGame.logger.async;

public sealed class AsyncWorker : IDisposable
{
    // The async loggers logger is actually not asynchronous,
    // as it is already completely executed asynchronous by itself
    // We don't want it to flush itself, but keep control of its flushes if necessary 
    private readonly ILogger _logger = LoggerFactory.GetLogger<AsyncWorker>(false);
    
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
    
    public void RequestFlush()
    {
        _signal.Set(); // wake worker early
    }
    
    private void WorkerLoop()
    {
        _logger.Trace("The system has started the asynchronous Logger-Thread");
        _logger.Flush();
        
        while (_running)
        {
            _signal.WaitOne(_interval);

            try
            {
                LoggerFactory.GlobalAsyncFlush();
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