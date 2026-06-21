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
    
    private readonly string _tag;

    public LogLevel Level;
    public void Debug(object message)
    {
        if (Level > LogLevel.DEBUG)
            return;
        string messageStr = message.ToString();
        if (!string.IsNullOrEmpty(_tag))
        {
            messageStr = _tag + ": " + messageStr;
        }
        
        Console.WriteLine($"[{DateTime.Now:HH:mm:ss:fff}] {(string.IsNullOrEmpty(_tag) ? "" : $"[{_tag}]")} [DEBUG]: " + message);
    }

    public void PrintList(IEnumerable list)
    {
        foreach (object o in list)
        {
            Debug(o);
        }
    }
    
    public void Info(object message)
    {
        if (Level > LogLevel.INFO)
            return;
        string messageStr = message.ToString();
        if (!string.IsNullOrEmpty(_tag))
        {
            messageStr = _tag + ": " + messageStr;
        }
        
        Console.WriteLine($"[{DateTime.Now:HH:mm:ss:fff}] {(string.IsNullOrEmpty(_tag) ? "" : $"[{_tag}]")} [INFO]: " + message);
    }
    
    public void Log(object message) => Info(message);

    public void Warn(object message)
    {
        if (Level > LogLevel.WARN)
            return;
        Console.WriteLine($"[{DateTime.Now:HH:mm:ss:fff}] {(string.IsNullOrEmpty(_tag) ? "" : $"[{_tag}]")} [WARNING]: " + message);
    }

    public void Error(object message)
    {
        if (Level > LogLevel.ERROR)
            return;
        
        Console.WriteLine($"[{DateTime.Now:HH:mm:ss:fff}] {(string.IsNullOrEmpty(_tag) ? "" : $"[{_tag}]")} [ERROR]: " + message);
    }
}