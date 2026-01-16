using System;

namespace RougeLiteGame.logger;

public abstract class BasicLogger : ILogger
{
    private readonly string _typeName;

    private const int MaxEntries = 1024;

    private readonly LogEntry[] _buffer = new LogEntry[MaxEntries];
    private readonly object _lock = new();
    private int _count;
    private int _index;

    protected BasicLogger(Type type)
    {
        _typeName = type.Name;
        for (int i = 0; i < _buffer.Length; i++)
        {
            _buffer[i] = new LogEntry();
        }
    }

    public void Log(object message, params object[] parameters)
    {
        Log(LogLevel.Info, message, parameters);
    }

    public void Log(LogLevel level, object message, params object[] parameters)
    {
        /*if (!EngineDebugger.IsActive())
        { // TODO find better solution to not exclude rider runs
            return; // disable in production as logging is hilariously slow
        }*/

        // ToString is very slow, so yeah
        Log((level, message.ToString(), parameters));
    }

    public void Log(LogLevel level, string message, params object[] parameters)
    {
        if (message == null) return;
        
        lock (_lock)
        {
            _buffer[_index].Level = level;
            _buffer[_index].Message = message;
            _buffer[_index].Arguments = parameters;
            _buffer[_index].Type = _typeName;

            _index++;
            if (_index == MaxEntries)
            {
                if (this is IAsyncLogger)
                {
                    LoggerFactory.AsyncWorker.RequestFlush();
                }
                else
                {
                    Flush();
                }
                
                _index = 0;
                _count = 0;
                return;
            }

            if (_count < MaxEntries)
            {
                _count++;
            }
        }
    }

    protected LogEntry[] Drain()
    {
        lock (_lock)
        {
            if (_count == 0)
            {
                return [];
            }

            LogEntry[] result = new LogEntry[_count];

            int start = (_index - _count + MaxEntries) % MaxEntries;
            for (int i = 0; i < _count; i++)
            {
                result[i] = _buffer[(start + i) % MaxEntries];
            }

            _index = 0;
            _count = 0;

            return result;
        }
    }

    public void Trace(object message, params object[] parameters)
    {
        Log(LogLevel.Trace, message, parameters);
    }

    public void Trace(string message, params object[] parameters)
    {
        Log(LogLevel.Trace, message, parameters);
    }

    public void Fine(object message, params object[] parameters)
    {
        Log(LogLevel.Fine, message, parameters);
    }

    public void Fine(string message, params object[] parameters)
    {
        Log(LogLevel.Fine, message, parameters);
    }

    public void Debug(object message, params object[] parameters)
    {
        Log(LogLevel.Debug, message, parameters);
    }

    public void Debug(string message, params object[] parameters)
    {
        Log(LogLevel.Debug, message, parameters);
    }

    public void Info(object message, params object[] parameters)
    {
        Log(LogLevel.Info, message, parameters);
    }

    public void Info(string message, params object[] parameters)
    {
        Log(LogLevel.Info, message, parameters);
    }

    public void Success(object message, params object[] parameters)
    {
        Log(LogLevel.Success, message, parameters);
    }

    public void Success(string message, params object[] parameters)
    {
        Log(LogLevel.Success, message, parameters);
    }

    public void Error(object message, params object[] parameters)
    {
        Log(LogLevel.Error, message, parameters);
    }

    public void Error(string message, params object[] parameters)
    {
        Log(LogLevel.Error, message, parameters);
    }

    public void Warn(object message, params object[] parameters)
    {
        Log(LogLevel.Warn, message, parameters);
    }

    public void Warn(string message, params object[] parameters)
    {
        Log(LogLevel.Warn, message, parameters);
    }

    public void Critical(object message, params object[] parameters)
    {
        Log(LogLevel.Critical, message, parameters);
    }

    public void Critical(string message, params object[] parameters)
    {
        Log(LogLevel.Critical, message, parameters);
    }

    public void Fatal(object message, params object[] parameters)
    {
        Log(LogLevel.Fatal, message, parameters);
    }

    public void Fatal(string message, params object[] parameters)
    {
        Log(LogLevel.Fatal, message, parameters);
    }

    public abstract void Flush();
}