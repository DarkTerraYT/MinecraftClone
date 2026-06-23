using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using MinecraftClone.Core.Numerics;

namespace MinecraftClone.Core.Level;

public class Level: IDrawable, IDirtyable
{
    public Dictionary<Point, Chunk.Chunk> Chunks = new(9);
    public Player Player;

    private Random random = new(0);
    
    private FastNoiseLite noise;

    private const int SeaLevel = 60;

    private const int WorldWidth = 8;
    private const int WorldDepth = 8;

    private const float CaveThreshold = 0.05f;
    private const float ExtraWormChance = 0.01f;
    
    public bool TryGetBlock(Vector3Int worldPos, out BlockState block)
    {
        int chunkX = worldPos.X / 16;
        int chunkY = worldPos.Z / 16;

        if (Chunks.TryGetValue(new Point(chunkX, chunkY), out Chunk.Chunk chunk))
        {
            int blockLocalX = worldPos.X % 16;
            int blockLocalZ = worldPos.Z % 16;
            int blockLocalY = worldPos.Y;

            if (chunk.TryGetBlock(new Vector3(blockLocalX, blockLocalY, blockLocalZ), out block))
            {
                return true;
            }

            return false;
        }

        block = null;
        return false;
    }
    
    public Level()
    {
        var stopwatch = new System.Diagnostics.Stopwatch();
        stopwatch.Start();
        noise = new FastNoiseLite();
        noise.SetNoiseType(FastNoiseLite.NoiseType.OpenSimplex2);

        int highestY = 0;
        
        
        // Basic gen pass
        for (int i = 0; i < WorldWidth; i++)
        {
            for (int j = 0; j < WorldDepth; j++)
            {
                Chunk.Chunk chunk = new Chunk.Chunk(new Vector3Int(i * 16, 0, j * 16));
                
                for (int x = 0; x < 16; x++)
                {
                    for (int z = 0; z < 16; z++)
                    {
                        int yOffset = (int)(noise.GetNoise(x + (i * 16), z + (j * 16)) * 3);
                        if (yOffset + SeaLevel > highestY)
                        {
                            highestY = yOffset + SeaLevel;
                            logger.Debug(highestY);
                        }
                        for (int y = 0; y < SeaLevel + yOffset; y++)
                        {
                            if (y < 1 + random.Next(0, 3)) 
                                chunk.SetBlock(new Vector3Int(x,y,z), new BlockState(Minecraft.Instance.Bedrock, new Vector3(x + (i * 16), y, z + (j * 16))));
                            else if (y < SeaLevel - 4 + yOffset)
                                chunk.SetBlock(new Vector3Int(x,y,z), new BlockState(Minecraft.Instance.Stone, new Vector3(x + (i * 16), y, z + (j * 16))));
                            else if (y < SeaLevel - 1 + yOffset)
                                chunk.SetBlock(new Vector3Int(x,y,z), new BlockState(Minecraft.Instance.Dirt, new Vector3(x + (i * 16), y, z + (j * 16))));
                            else 
                                chunk.SetBlock(new Vector3Int(x,y,z), new BlockState(Minecraft.Instance.GrassBlock, new Vector3(x + (i * 16), y, z + (j * 16))));
                        }
                    }
                }
                
                Chunks.Add(new Point(i, j), chunk);
            }
        }
        
        // Worm Cave pass
        int worldSize = WorldWidth * WorldDepth;
        int numWorms = worldSize / 4;

        PerlinWorm createWorm()
        {
            return new PerlinWorm(
                new Vector3(random.Next(0, WorldWidth * 16), random.Next(10, highestY + 1),
                    (random.Next(0, WorldDepth * 16))), random.NextSingle() * 360 - 180, random.NextSingle() * 360 - 180, random.Next(2, 4),
                1 - MathF.Min(random.NextSingle() / 2, 0.2f), random.Next(30, 160));
        }
        
        Queue<PerlinWorm> worms = new Queue<PerlinWorm>();
        
        for (int i = 0; i <= numWorms; i++)
        {
            worms.Enqueue(createWorm());
        }

        while (worms.Count > 0)
        {
            PerlinWorm worm = worms.Dequeue();
            ClearBlocksInRadius(worm.Position, worm.Size);
            while (worm.StepsLeft > 0)
            {
                worm.Step(random);
                ClearBlocksInRadius(worm.Position, worm.Size);

                if (worm.CanDuplicate && random.NextSingle() <= ExtraWormChance)
                {
                    PerlinWorm newWorm = createWorm();
                    newWorm.CanDuplicate = false;
                    newWorm.Position = worm.Position;
                    worms.Enqueue(newWorm);
                }
            }
        }
        
        // Blobish cave pass
        FastNoiseLite noise2 = new FastNoiseLite(FastNoiseLite.NoiseType.Perlin);

        for (int x = 0; x < WorldWidth * 16; x++)
        {
            for (int z = 0; z < WorldDepth * 16; z++)
            {
                for (int y = 0; y < highestY + 1; y++)
                {
                    float chance = (noise.GetNoise(x + noise2.GetNoise(z, random.NextSingle()) * 100, y, z + noise2.GetNoise(x, random.NextSingle()) * 100) + 1) / 2;
                    chance *= GetCaveChance(y);

                    
                    if (chance <= CaveThreshold)
                    {
                        SetBlock(new Vector3Int(x, y, z), null);
                    }
                }
            }
        }
        
        
        
        int total = WorldWidth * WorldDepth;
        stopwatch.Stop();
        double time = stopwatch.Elapsed.TotalSeconds;
        long ms = stopwatch.ElapsedMilliseconds;
        logger.Log($"Generated {WorldWidth}x{WorldDepth} ({total}) chunks in {time}s, for an average of {ms / total} ms per chunk.");
        
        Player = new Player(new Vector3(WorldWidth / 2.0f * 16, 4 + SeaLevel, WorldDepth / 2.0f * 16));
    }

    private float GetCaveChance(int y)
    {
        return -0.0002f * (y - 50) * (y - 50) + 1;
    }
    
    public void SetBlock(Vector3Int worldPos, BlockState block)
    {
        int chunkX = worldPos.X / 16;
        int chunkY = worldPos.Z / 16;

        if (Chunks.TryGetValue(new Point(chunkX, chunkY), out Chunk.Chunk chunk))
        {
            int blockLocalX = worldPos.X % 16;
            int blockLocalZ = worldPos.Z % 16;
            int blockLocalY = worldPos.Y;

            if (chunk.TryGetBlock(new Vector3(blockLocalX, blockLocalY, blockLocalZ), out var block2))
            {
                if (block2.Block == Minecraft.Instance.Bedrock)
                    return;
            }
            
            chunk.SetBlock(new Vector3(blockLocalX, blockLocalY, blockLocalZ), block);
        }
    }
    
    private void ClearBlocksInRadius(Vector3Int worldPos, int radius)
    { 
        int radiusSquared = radius * radius;
        
        int centerX = worldPos.X;
        int centerZ = worldPos.Z;
        int centerY = worldPos.Y;

        for (int x = -radius; x < radius + 1; x++)
        {
            int blockX = centerX + x;
                
            int maxY = (int)Math.Sqrt(radiusSquared - x * x);

            for (int y = -maxY; y < maxY + 1; y++)
            {
                int blockY = centerY + y;
                    
                int maxZ = (int)Math.Sqrt(radiusSquared - y * y);

                for (int z = -maxZ; z < maxZ + 1; z++)
                {
                    int blockZ = centerZ + z;

                    SetBlock(new Vector3Int(blockX, blockY, blockZ), null);
                }
            }
        }
    }
    
    private Logger logger = new Logger("Level");
    
    public void Draw(GameTime gameTime)
    {
        foreach (var chunk in Chunks.Values)
        {
            chunk.Draw(gameTime);
        }
    }

    public int DrawOrder { get; }
    public bool Visible { get; }
    
    
    public event EventHandler<EventArgs> DrawOrderChanged;
    public event EventHandler<EventArgs> VisibleChanged;
    
    
    public bool Dirty => isDirty;

    private bool isDirty = false;
    
    public void MarkDirty()
    {
        isDirty = true;
    }
}