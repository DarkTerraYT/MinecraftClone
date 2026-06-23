using System;
using System.Collections;

namespace MinecraftClone.Core;

public enum LogLevel
{
    DEBUG = 0,
    INFO = 1, 
    WARN = 2,
    ERROR = 3,
}

public class Logger
{
    public Logger(string tag, LogLevel level = LogLevel.DEBUG)
    {
        _tag = tag;
        Level = level;
    }
    
    public static readonly Logger Global = new("");
    
    private readonly string _tag;

    public LogLevel Level;
    
    /// <summary>
    /// Logs an object to the console with the DEBUG tag and without saving to the log file. For verbose.
    /// </summary>
    public void Debug(object message)
    {
        if (Level > LogLevel.DEBUG)
            return;
        Console.WriteLine($"[{DateTime.Now:HH:mm:ss:fff}]{(string.IsNullOrEmpty(_tag) ? "" : $" [{_tag}]")} [DEBUG]: " + message);
    }

    /// <summary>
    /// Debug logs every element of a list into its own line. Uses Debug to avoid saving to the log file
    /// </summary>
    public void PrintList(IEnumerable list)
    {
        foreach (object o in list)
        {
            Debug(o);
        }
    }
    
    /// <summary>
    /// Logs an object to the console with the INFO tag
    /// </summary>
    public void Info(object message)
    {
        if (Level > LogLevel.INFO)
            return;
        Console.WriteLine($"[{DateTime.Now:HH:mm:ss:fff}]{(string.IsNullOrEmpty(_tag) ? "" : $" [{_tag}]")} [INFO]: " + message);
    }
    
    public void Log(object message) => Info(message);

    /// <summary>
    /// Logs an object to the console with the WARNING tag
    /// </summary>
    public void Warn(object message)
    {
        if (Level > LogLevel.WARN)
            return;
        Console.WriteLine($"[{DateTime.Now:HH:mm:ss:fff}]{(string.IsNullOrEmpty(_tag) ? "" : $" [{_tag}]")} [WARNING]: " + message);
    }

    /// <summary>
    /// Logs an object to the console with the ERROR tag
    /// </summary>
    public void Error(object message)
    {
        if (Level > LogLevel.ERROR)
            return;
        
        Console.WriteLine($"[{DateTime.Now:HH:mm:ss:fff}]{(string.IsNullOrEmpty(_tag) ? "" : $" [{_tag}]")} [ERROR]: " + message);
    }
}