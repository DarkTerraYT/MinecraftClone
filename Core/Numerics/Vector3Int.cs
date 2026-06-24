using System;
using Microsoft.Xna.Framework;

namespace MinecraftClone.Core.Numerics;

public struct Vector3Int(int x, int y, int z) : IEquatable<Vector3Int>
{
    public int X = x;
    public int Y = y;
    public int Z = z;

    public static bool operator ==(Vector3Int @this, Vector3Int other)
    {
        return @this.Equals(other);
    }

    public static bool operator !=(Vector3Int @this, Vector3Int other)
    {
        return !(@this == other);
    }

    public static Vector3Int operator + (Vector3Int left, Vector3Int right)
    {
        return new Vector3Int(left.X + right.X, left.Y + right.Y, left.Z + right.Z);
    }
    public static Vector3Int operator - (Vector3Int left, Vector3Int right)
    {
        return new Vector3Int(left.X - right.X, left.Y - right.Y, left.Z - right.Z);
    }
    public static Vector3Int operator * (Vector3Int left, Vector3Int right)
    {
        return new Vector3Int(left.X * right.X, left.Y * right.Y, left.Z * right.Z);
    }
    public static Vector3Int operator * (Vector3Int left, int right)
    {
        return new Vector3Int(left.X * right, left.Y * right, left.Z * right);
    }
    public static Vector3Int operator / (Vector3Int left, Vector3Int right)
    {
        return new Vector3Int(left.X / right.X, left.Y / right.Y, left.Z / right.Z);
    }
    public static Vector3Int operator / (Vector3Int left, int right)
    {
        return new Vector3Int(left.X / right, left.Y / right, left.Z / right);
    }
    
    public static implicit operator Vector3(Vector3Int v)
    {
        return new Vector3(v.X, v.Y, v.Z);
    }

    public static implicit operator Vector3Int(Vector3 v)
    {
        return new Vector3Int((int)v.X, (int)v.Y, (int)v.Z);
    }

    public bool Equals(Vector3Int other)
    {
        return X == other.X && Y == other.Y && Z == other.Z;
    }

    public override bool Equals(object obj)
    {
        return obj is Vector3Int other && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(X, Y, Z);
    }

    public override string ToString()
    {
        return $"({X}, {Y}, {Z})";
    }
}