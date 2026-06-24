using System;

namespace MinecraftClone.Core.World;

[Flags]
public enum CullDirection : byte // Use byte to save memory and computation time
{
    None = 0,
    Front = 1 << 0, // 1
    Back = 1 << 1, // 2
    Left = 1 << 2, // 4
    Right = 1 << 3, // 8
    Top = 1 << 4, // 16
    Bottom = 1 << 5, // 32
    All = Front | Back | Left | Right | Top | Bottom
}