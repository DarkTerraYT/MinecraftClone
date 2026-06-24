using System;

namespace MinecraftClone.Core.Level;

[Flags]
public enum ChunkUpdateFlags
{
    None = 0, // 0
    OpaqueMesh = 1, // 1
    Lighting = 2, // 10 
    TransparentMesh = 4, // 100
    All = ~0
}