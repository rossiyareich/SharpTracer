﻿namespace SharpTracer.Core.Logging;

public class ConsoleLogger : ILogger
{
    private static readonly ConsoleLogger _logger = new();
    private static readonly ConsoleColor _defaultColor = ConsoleColor.White;

    private ConsoleLogger()
    {
    }

    public void LogDebug(string message)
    {
        lock (_logger)
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine($"{DateTime.Now} -> Debug: {message}");
        }
    }

    public void LogInfo(string message)
    {
        lock (_logger)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"{DateTime.Now} -> Info: {message}");
        }
    }

    public void LogWarning(string message)
    {
        lock (_logger)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"{DateTime.Now} -> Warning: {message}");
        }
    }

    public void LogError(string message)
    {
        lock (_logger)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"{DateTime.Now} -> Error: {message}");
        }
    }

    public static ConsoleLogger Get() => _logger;

    public static void ResetColor()
    {
        lock (_logger)
        {
            Console.ForegroundColor = _defaultColor;
        }
    }
}
