using System;
using Godot;

namespace RougeLiteGame.logger;

public abstract class BasicLogger : ILogger
{
    private readonly string _typeName;

    protected const int MaxEntries = 1024;

    protected readonly LogEntry[] Buffer = new LogEntry[MaxEntries];
    protected readonly object Lock = new();
    protected int Count;
    protected int Index;

    protected BasicLogger(Type type)
    {
        _typeName = type.Name;
        for (int i = 0; i < Buffer.Length; i++)
        {
            Buffer[i] = new LogEntry();
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
        
        lock (Lock)
        {
            Buffer[Index].Level = level;
            Buffer[Index].Message = message;
            Buffer[Index].Arguments = parameters;
            Buffer[Index].Type = _typeName;

            Index++;
            if (Index == MaxEntries)
            {
                Index = 0;
            }

            if (Count < MaxEntries)
            {
                Count++;
            }
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