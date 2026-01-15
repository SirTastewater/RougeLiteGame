using System;

namespace RougeLiteGame.logger.console;

public class AsyncConsoleLogger(Type type, AsyncWorker asyncWorker) : ConsoleLogger(type)
{
    public override void Flush()
    {
        asyncWorker.Enqueue(Snapshots());
    }
}