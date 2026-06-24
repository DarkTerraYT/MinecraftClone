using Microsoft.Xna.Framework;
using MinecraftClone.Core.Numerics;

namespace MinecraftClone.Core.Level.WorldGeneration.Passes;

public class IronGenPass : GenPass
{
    private const float IronFrequency = 7;
    private const float IronThreshold = 0.0005f * IronFrequency;
    
    public override int Order => ORES + 5; // Do Iron last as it's the most common
    public override void Pass(Level level, FastNoiseLite globalNoise)
    {FastNoiseLite noise = new FastNoiseLite(level.Seed ^ -1095584316, FastNoiseLite.NoiseType.Cellular); // Merge the two seeds

        int maxZ = Level.WorldDepth * Chunk.Depth;
        int maxY = Level.WorldHeight * Chunk.Height;
        
        float[] warpX = new float[maxZ];
        for (int z = 0; z < maxZ; z++) {
            warpX[z] = globalNoise.GetNoise(z, 0);
        }

        float[] warpZ = new float[maxY];
        for (int y = 0; y < maxY; y++) {
            warpZ[y] = globalNoise.GetNoise(y, 0);
        }
        
        float adjustedThreshold = (IronThreshold * 2.0f) - 1.0f;
        
        for (int x = 0; x < Level.WorldWidth * Chunk.Width; x++)
        {
            for (int y = 0; y < maxY; y++)
            {
                float sample = y * IronFrequency;
                for (int z = 0; z < maxZ; z++)
                {
                    float result = noise.GetNoise((x + warpX[z]) * IronFrequency, sample, (z + warpZ[y]) * IronFrequency);

                    if (result <= adjustedThreshold)
                    {
                        if (level.TryGetBlock(new Vector3Int(x, y, z), out BlockState block))
                        {
                            if (block.Block == Minecraft.Instance.Stone)
                            {
                                level.SetBlock(new Vector3Int(x, y, z), Minecraft.Instance.CoalOre);
                            }
                        }
                    }
                }
            }
        }
    }
}