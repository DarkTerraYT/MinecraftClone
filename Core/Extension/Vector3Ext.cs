using Microsoft.Xna.Framework;
using MinecraftClone.Core.Numerics;

namespace MinecraftClone.Core.Extension;

public static class Vector3Ext
{
    public static Vector3Int ToChunkCoords(this Vector3 v)
    {
        return v / 16;
    }

    public static string ToString(this Vector3 v, string format)
    {
        return "{" + v.X.ToString(format) + ", " + v.Y.ToString(format) + ", " + v.Z.ToString(format) + "}";
    }
}