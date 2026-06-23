using Microsoft.Xna.Framework;
using MinecraftClone.Core.Numerics;

namespace MinecraftClone.Core.Extension;

public static class Vector3Ext
{
    public static Vector3Int ToChunkCoords(this Vector3 v)
    {
        return v / 16;
    }
}