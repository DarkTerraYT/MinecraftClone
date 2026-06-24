using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace MinecraftClone.Core.Level.WorldGeneration.Passes;

public class WormCavePass : GenPass
{
    private const float ExtraWormChance = 0.01f;
    
    public override int Order => CAVES;
    public override void Pass(Level level, FastNoiseLite globalNoise)
    {
        int worldSize = Level.WorldWidth * Level.WorldDepth;
        int numWorms = worldSize / 4;
        
        Queue<PerlinWorm> worms = new Queue<PerlinWorm>();
        for (int i = 0; i <= numWorms; i++)
        {
            worms.Enqueue(CreateWorm(level));
        }

        while (worms.Count > 0)
        {
            PerlinWorm worm = worms.Dequeue();
            level.ClearBlocksInRadius(worm.Position, worm.Size);
            while (worm.StepsLeft > 0)
            {
                worm.Step(level.Random);
                level.ClearBlocksInRadius(worm.Position, worm.Size);

                if (worm.CanDuplicate && level.Random.NextSingle() <= ExtraWormChance)
                {
                    PerlinWorm newWorm = CreateWorm(level);
                    newWorm.CanDuplicate = false;
                    newWorm.Position = worm.Position;
                    worms.Enqueue(newWorm);
                }
            }
        }
    }

    private static PerlinWorm CreateWorm(Level level)
    {
        return new PerlinWorm(
            new Vector3(level.Random.Next(0, Level.WorldWidth * Chunk.Width), level.Random.Next(10, level.HighestY + 1),
                (level.Random.Next(0, Level.WorldDepth * Chunk.Depth))), level.Random.NextSingle() * 360 - 180, level.Random.NextSingle() * 360 - 180, level.Random.Next(2, 4),
            1 - MathF.Min(level.Random.NextSingle() / 2, 0.2f), level.Random.Next(30, 160));
    }
}