using Microsoft.Xna.Framework;
using MinecraftClone.Core.Numerics;

namespace MinecraftClone.Core.Level.WorldGeneration.Passes;

public class IronGenPass : GenPass
{
    private const float IronFrequency = 7;
    private const float IronThreshold = 0.0005f * IronFrequency;
    
    public override int Order => ORES + 5; // Do Iron last as it's the most common
    public override void Pass(Level level, FastNoiseLite globalNoise)
    {
        FastNoiseLite noise = new FastNoiseLite(level.Seed ^ -1955584132, FastNoiseLite.NoiseType.Cellular); // Merge the two seeds

        for (int x = 0; x < Level.WorldWidth * Chunk.Width; x++)
        {
            for (int y = 0; y < Level.WorldDepth * Chunk.Height; y++)
            {
                for (int z = 0; z < Level.WorldDepth * Chunk.Width; z++)
                {
                    float result = (noise.GetNoise((x + globalNoise.GetNoise(z, 0)) * IronFrequency, y * IronFrequency, (z + globalNoise.GetNoise(y, 0)) * IronFrequency) + 1) / 2.0f;

                    if (result <= IronThreshold)
                    {
                        if (level.TryGetBlock(new Vector3Int(x, y, z), out BlockState block))
                        {
                            if (block.Block == Minecraft.Instance.Stone)
                            {
                                level.SetBlock(new Vector3Int(x, y, z), new BlockState(Minecraft.Instance.IronOre, new Vector3(x, y, z), level));
                            }
                        }
                    }
                }
            }
        }
    }
}