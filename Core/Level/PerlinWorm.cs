using System;
using Microsoft.Xna.Framework;

namespace MinecraftClone.Core.Level;

public struct PerlinWorm(Vector3 pos, float yaw, float pitch, int size, float gravityBias, int steps)
{
    /// <summary>
    /// The current world positon of the worm
    /// </summary>
    public Vector3 Position = pos;
    
    /// <summary>
    /// Current direction
    /// </summary>
    public float Yaw = yaw, Pitch = pitch;
    
    /// <summary>
    /// The radius of blocks to carve out
    /// </summary>
    public readonly int Size = size;

    /// <summary>
    /// How often the worm goes down
    /// </summary>
    public readonly float GravityBias = gravityBias;

    public bool CanDuplicate = true; // If the worm can spawn more worms

    public int StepsLeft = steps;

    public void Step(Random random)
    {
        Position.X += MathF.Cos(MathHelper.ToRadians(Yaw));
        Position.Z += MathF.Cos(MathHelper.ToRadians(Pitch));

        Yaw += (random.NextSingle() * 180) - 90;
        Pitch += (random.NextSingle() * 180) - 90;

        float vertical = (random.NextSingle() * 2) - 1;
        if (vertical <= -GravityBias)
        {
            Position.Y--;
        }
        else if (vertical >= GravityBias)
        {
            Position.Y++;
        }
        StepsLeft--;
    }
}