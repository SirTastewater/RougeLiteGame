using System;

namespace RougeLiteGame.logger.async;

public class BasicAsyncLogger(Type type, ILogRenderer renderer, params ILogWriter[] logWriters) : BasicLogger(type, renderer, logWriters), IAsyncLogger;