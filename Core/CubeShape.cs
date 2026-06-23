using System;
using Microsoft.Xna.Framework;
using MinecraftClone.Core.World;

namespace MinecraftClone.Core;

public class CubeShape
{
    public Vector3 Min { get; set; }
    public Vector3 Max { get; set; }
    public Vector3 Position { get; set; }
    
    public Vector3 WorldMin => Position + Min;
    public Vector3 WorldMax => Position + Max;
    public static CubeShape Full => new CubeShape(Vector3.Zero, new Vector3(1, 1, 1), Vector3.Zero);

    public CubeShape(Vector3 min, Vector3 max, Vector3 position)
    {
        Min = min;
        Max = max;
        Position = position;
    }

    public bool Intersects(Ray ray)
    {
        return false;
    }

    public bool Intersects(CubeShape other)
    {
        return (WorldMin.X <= other.WorldMax.X && WorldMax.X >= other.WorldMin.X) && // X
               (WorldMin.Y <= other.WorldMax.Y && WorldMax.Y >= other.WorldMin.Y) && // Y
               (WorldMin.Z <= other.WorldMax.Z && WorldMax.Z >= other.WorldMin.Z); // Z
    }

    public Face.Direction IntersectionDirection(CubeShape other)
    {
        float overlapX = Math.Min(WorldMax.X, other.WorldMax.X) - Math.Max(WorldMin.X, other.WorldMin.X);
        float overlapY = Math.Min(WorldMax.Y, other.WorldMax.Y) - Math.Max(WorldMin.Y, other.WorldMin.Y);
        float overlapZ = Math.Min(WorldMax.Z, other.WorldMax.Z) - Math.Max(WorldMin.Z, other.WorldMin.Z);

        if (overlapX <= 0 || overlapY <= 0 || overlapZ <= 0)
        {
            return Face.Direction.None;
        }

        float xPlane = overlapY * overlapZ;
        float yPlane = overlapX * overlapZ;
        float zPlane = overlapX * overlapY;

        if (xPlane >= yPlane && xPlane >= zPlane)
        {
            float centerX = WorldMin.X + WorldMax.X;
            float otherCenterX = other.WorldMin.X + other.WorldMax.X;

            return (centerX > otherCenterX) ? Face.Direction.Left : Face.Direction.Right;
        }

        if (yPlane >= zPlane && yPlane >= xPlane)
        {
            float centerY = WorldMin.Y + WorldMax.Y;
            float otherCenterY = other.WorldMin.Y + other.WorldMax.Y;

            return (centerY > otherCenterY) ? Face.Direction.Bottom : Face.Direction.Top;
        }

        float centerZ = WorldMin.Z + WorldMax.Z;
        float otherCenterZ = other.WorldMin.Z + other.WorldMax.Z;

        return (centerZ > otherCenterZ) ? Face.Direction.Back : Face.Direction.Front;
    }
}