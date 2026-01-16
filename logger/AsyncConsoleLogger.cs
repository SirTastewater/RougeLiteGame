using System;

namespace RougeLiteGame.logger;

public class AsyncConsoleLogger(Type type) : ConsoleLogger(type), IAsyncLogger;