using System;

namespace MinecraftClone.Core.Level.WorldGeneration;

public abstract class GenPass : IComparable<GenPass>
{
    public const int ORES = 1;
    public const int CAVES = 100;
    
    public abstract int Order { get; }
    
    public virtual string Name => GetType().Name;

    public abstract void Pass(Level level, FastNoiseLite globalNoise);
    
    public int CompareTo(GenPass other)
    {
        if (ReferenceEquals(this, other)) return 0;
        if (other is null) return 1;
        return Order.CompareTo(other.Order);
    }
}