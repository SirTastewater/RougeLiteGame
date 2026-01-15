using System;
using System.Text;
using Godot;

namespace RougeLiteGame.logger.console;

public class ConsoleLogger(Type type) : BasicLogger(type)
{

    public override void Flush()
    {
        ConsoleStream.Output(Snapshots());
    }

    protected LogEntry[] Snapshots()
    {
        LogEntry[] snapshotEntries;

        lock (Lock)
        {
            if (Count == 0) return [];

            int countSnapshot = Count;
            int indexSnapshot = Index;

            Count = 0;

            snapshotEntries = new LogEntry[countSnapshot];

            for (var i = 0; i < countSnapshot; i++)
            {
                int idx = indexSnapshot - countSnapshot + i;
                if (idx < 0) idx += MaxEntries;
                snapshotEntries[i] = Buffer[idx];
            }
        }

        return snapshotEntries;
    }
}