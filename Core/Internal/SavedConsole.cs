using System;
using System.IO;
using System.Text;

namespace MinecraftClone.Core.Internal;

internal class SavedConsole : TextWriter
{
    private readonly TextWriter _consoleWriter;
    private readonly StreamWriter _fileWriter;
    
    public override Encoding Encoding => Encoding.UTF8;

    private static readonly string LogDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "LerpMcGerk", "Minecraft", "logs");
    private static readonly string LogPath = Path.Combine(LogDirectory, "Latest.log");
    private string _closedPath = Path.Combine(LogDirectory, DateTime.Now.ToString("yy-MM-dd_HH-mm-ss") + ".log");
    
    public SavedConsole()
    {
        
        if (!Directory.Exists(LogDirectory))
        {
            Directory.CreateDirectory(LogDirectory);
        }
        _consoleWriter = Console.Out;
        _fileWriter = File.CreateText(LogPath);
        _fileWriter.AutoFlush = true;
    }

    public override void Write(string value)
    {
        _consoleWriter.Write(value);
        if (value != null && !value.Contains("[DEBUG]"))
        {
            _fileWriter.Write(value);
        }
    }

    public override void Write(char value)
    {
        _consoleWriter.Write(value);
        _fileWriter.Write(value);
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _fileWriter?.Dispose();
            File.WriteAllText(_closedPath, File.ReadAllText(LogPath, Encoding.UTF8), Encoding.UTF8);
        }
        base.Dispose(disposing);
    }
}