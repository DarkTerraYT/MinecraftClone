using System;
using Microsoft.Xna.Framework;
using MinecraftClone.Core.Numerics;

namespace MinecraftClone.Core.Level.WorldGeneration;

public abstract class OrePass : GenPass
{
    public abstract Block Block { get; }
    
    public virtual Block OverrideBlock => Minecraft.Instance.Stone;
    
    
    public abstract float Frequency { get; }
    public abstract float Threshold { get; }

    public abstract int MaxY { get; }

    private float YMultiplier = -1;
    
    public virtual float GetChance(float y)
    {
        if (YMultiplier == -1) YMultiplier = 1 / (float)(MaxY * MaxY);

        return Math.Max(1 - (YMultiplier * y * y), 0);
    }
    
    protected abstract int Seed { get; }
    protected virtual FastNoiseLite.NoiseType NoiseType => FastNoiseLite.NoiseType.Cellular;

    public override void Pass(Level level, FastNoiseLite globalNoise)
    {
        FastNoiseLite noise = new FastNoiseLite(level.Seed ^ Seed, NoiseType); // Merge the two seeds

        int maxZ = Level.WorldDepth * Chunk.Depth;
        int maxY = Level.WorldHeight * Chunk.Height;
        
        float[] warpX = new float[maxZ];
        for (int z = 0; z < maxZ; z++) {
            warpX[z] = globalNoise.GetNoise(z, 0) * 1.45f;
        }

        float[] warpZ = new float[maxY];
        for (int y = 0; y < maxY; y++) {
            warpZ[y] = globalNoise.GetNoise(y, 0) * 1.45f;
        }
        

        for (int chunkX = 0; chunkX < Level.WorldWidth; chunkX++)
        {
            int baseX = chunkX * Chunk.Width;
            for (int chunkY = 0; chunkY < Level.WorldHeight; chunkY++)
            {
                int baseY = chunkY * Chunk.Height;
                for (int chunkZ = 0; chunkZ < Level.WorldDepth; chunkZ++)
                {
                    int baseZ = chunkZ * Chunk.Depth;

                    if (!level.TryGetChunk(new Vector3(chunkX * 16, chunkY * 16, chunkZ * 16), out var chunk)) continue;
                    
                    for (int x = 0; x < Chunk.Width; x++)
                    {
                        int worldX = baseX + x;
                        for (int y = 0; y < Chunk.Height; y++)
                        {
                            int worldY = baseY + y;
                            float sampleY = worldY * Frequency;
                            for (int z = 0; z < Chunk.Width; z++)
                            {
                                int worldZ = baseZ + z;
                                
                                float result = noise.GetNoise((worldX + warpX[z]) * Frequency, sampleY, (worldZ + warpZ[y]) * Frequency);

                                var localPos = new Vector3Int(x, y, z);
                                
                                float localThreshold = Threshold * GetChance(worldY);
                                float adjustedThreshold = (localThreshold * 2.0f) - 1.0f;
                                
                                if (result <= adjustedThreshold)
                                {
                                    BlockState block = chunk.GetBlock(localPos);
                                    if (block.Test(OverrideBlock))
                                    {
                                        chunk.SetBlock(localPos, Block);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}