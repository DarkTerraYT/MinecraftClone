using System;
using System.Linq;

namespace MinecraftClone.Core;

public readonly struct Identifier
{
    public const char Separator = ':';
    
    public string Namespace { get; }

    public string Path { get; }

    private Identifier(string @namespace, string path)
    {
        Namespace = @namespace;
        Path = path;
        if ((Namespace + Path).Any(chr => chr == Separator || chr == ' '))
        {
            throw new FormatException($"Invalid Identifier {this}! You may not have spaces or {Separator}s in your namespace or path!");
        }
    }

    public static Identifier FromNamespaceAndPath(string @namespace, string path)
    {
        return new Identifier(@namespace, path);
    }

    public override string ToString()
    {
        return $"{Namespace}{Separator}{Path}";
    }
}