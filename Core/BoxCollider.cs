using System;
using Microsoft.Xna.Framework;
using MinecraftClone.Core.World;

namespace MinecraftClone.Core;

public class BoxCollider
{
    public Vector3 Min { get; }
    public Vector3 Max { get; }
    public Vector3 Position { get; set; }
    
    public float Width => Max.X - Min.X;
    public float Height => Max.Y - Min.Y;
    public float Depth => Max.Z - Min.Z;
    
    public Vector3 WorldMin => Position + Min;
    public Vector3 WorldMax => Position + Max;
    public static BoxCollider Full => new BoxCollider(new Vector3(1, 1, 1), Vector3.Zero);

    public BoxCollider(Vector3 max, Vector3 position)
    {
        Min = Vector3.Zero;
        Max = max;
        Position = position;
    }

    public bool Intersects(Ray ray)
    {
        Logger.Global.Warn("Ray intersection is not implemented.");
        return false;
    }

    public bool IntersectsX(BoxCollider other)
    {
        return WorldMin.X < other.WorldMax.X && WorldMax.X > other.WorldMin.X;
    }
    public bool IntersectsY(BoxCollider other)
    {
        return WorldMin.Y < other.WorldMax.Y && WorldMax.Y > other.WorldMin.Y;
    }
    public bool IntersectsZ(BoxCollider other)
    {
        return WorldMin.Z < other.WorldMax.Z && WorldMax.Z > other.WorldMin.Z;
    }
    
    public bool Intersects(BoxCollider other)
    {
        return IntersectsX(other) && IntersectsY(other) && IntersectsZ(other);
    }
}