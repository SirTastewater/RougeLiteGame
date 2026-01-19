using System;

namespace RougeLiteGame.logger.async;

public class BasicAsyncLogger(Type type, params ILogWriter[] logWriters) : BasicLogger(type, logWriters), IAsyncLogger;