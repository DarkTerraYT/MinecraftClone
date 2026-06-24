using MinecraftClone.Core.Numerics;

namespace MinecraftClone.Core.Level.WorldGeneration.Passes;

public class BlobCavePass : GenPass
{
    private const float CaveThreshold = 0.005f;
    
    public override int Order => CAVES + 1;
    
    public override void Pass(Level level, FastNoiseLite globalNoise)
    {
        FastNoiseLite noise = new FastNoiseLite(level.Seed);
        FastNoiseLite noise2 = new FastNoiseLite(level.Seed ^ 1031235412);

        for (int x = 0; x < Level.WorldWidth * 16; x++)
        {
            for (int z = 0; z < Level.WorldDepth * 16; z++)
            {
                for (int y = 0; y < level.HighestY + 1; y++)
                {
                    float chance = (noise.GetNoise(x + noise2.GetNoise(z, level.Random.NextSingle()) * 100, y, z + noise2.GetNoise(x, level.Random.NextSingle()) * 100) + 1) / 2;
                    chance *= GetCaveChance(y);
                    
                    if (chance <= CaveThreshold)
                    {
                        level.SetBlock(new Vector3Int(x, y, z), null);
                    }
                }
            }
        }
    }
    
    private float GetCaveChance(int y)
    {
        return -0.0002f * (y - 50) * (y - 50) + 1;
    }
}