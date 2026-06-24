using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using MinecraftClone.Core.Level.WorldGeneration;
using MinecraftClone.Core.Numerics;

namespace MinecraftClone.Core.Level;

public class Level: IDrawable, IDirtyable
{
    public Dictionary<Vector3Int, Chunk> Chunks = new(9);
    public Player Player;

    public readonly int Seed;
    
    public readonly Random Random;
    
    private FastNoiseLite noise;

    private const int SeaLevel = 60;

    public const int WorldWidth = 8;
    public const int WorldDepth = 8;
    public const int WorldHeight = 16;

    private int highestY;
    public int HighestY => highestY;
    
    public Level(string seed = null)
    {
        Minecraft.Instance.Level = this;
        Seed = seed?.GetHashCode() ?? (int)DateTimeOffset.Now.ToUnixTimeSeconds();
        Random = new Random(Seed);
        
        var stopwatch = new System.Diagnostics.Stopwatch();
        stopwatch.Start();
        noise = new FastNoiseLite(Seed);
        noise.SetNoiseType(FastNoiseLite.NoiseType.OpenSimplex2);
        
        // Basic gen pass
        for (int i = 0; i < WorldWidth; i++)
        {
            for (int j = 0; j < WorldDepth; j++)
            {
                for (int k = 0; k < WorldHeight; k++)
                {
                    Chunk chunk = new Chunk(new Vector3(i * Chunk.Width, k * Chunk.Height, j * Chunk.Depth));
                    if (!Chunks.TryAdd(new Vector3Int(i, k, j), chunk)) continue;

                    for (int x = 0; x < Chunk.Width; x++)
                    {
                        for (int z = 0; z < Chunk.Depth; z++)
                        {
                            int yOffset = (int)(noise.GetNoise(x + i * Chunk.Width, z + j * Chunk.Depth) * 3);
                            int surface = SeaLevel + yOffset;
                            
                            if (highestY < surface) highestY = surface;
                            
                            for (int y = 0; y < Chunk.Height; y++)
                            {
                                int worldY = k * 16 + y;
                                if (worldY > surface) continue;
                                int worldX = i * Chunk.Width + x;
                                int worldZ = j * Chunk.Depth + z;
                                Vector3Int worldPos = new Vector3Int(worldX, worldY, worldZ);
                                
                                if (worldY < 1 + Random.Next(1, 3))
                                {
                                    SetBlock(worldPos, new BlockState(Minecraft.Instance.Bedrock, worldPos, this));
                                    continue;
                                }
                                if (worldY < surface - 4)
                                {
                                    SetBlock(worldPos, new BlockState(Minecraft.Instance.Stone, worldPos, this));
                                    continue;
                                }
                                if (worldY < surface)
                                {
                                    SetBlock(worldPos, new BlockState(Minecraft.Instance.Dirt, worldPos, this));
                                    continue;
                                }
                                
                                SetBlock(worldPos, new BlockState(Minecraft.Instance.GrassBlock, worldPos, this));
                            }
                        }
                    }
                }
            }
        }
        
        // Other passes
        foreach (GenPass pass in Minecraft.Instance.GenPasses.OrderBy(pass => pass.Order))
        {
            pass.Pass(this, noise);
        }
        
        // Generate chunk meshes
        foreach (var chunk in Chunks.Values)
        {
            chunk.Update(ChunkUpdateFlags.All);
        }
        
        
        int total = WorldWidth * WorldDepth * WorldHeight;
        stopwatch.Stop();
        double time = stopwatch.Elapsed.TotalSeconds;
        logger.Log($"Generated {WorldWidth}x{WorldDepth}x{WorldHeight} ({total}) chunks in {time}s, for an average of {time * 1000 / total:F2} ms per chunk.");
        
        Player = new Player(new Vector3(WorldWidth / 2.0f * 16, 4 + SeaLevel, WorldDepth / 2.0f * 16), this);
    }

    public bool TryGetChunk(Vector3 position, out Chunk chunk)
    {
        int chunkX = (int)position.X / Chunk.Width;
        int chunkY = (int)position.Y / Chunk.Height;
        int chunkZ = (int)position.Z / Chunk.Depth;
        
        return Chunks.TryGetValue(new Vector3Int(chunkX, chunkY,  chunkZ), out chunk);
    }
    
    public bool TryGetBlock(Vector3Int worldPos, out BlockState block)
    {
        if (TryGetChunk(worldPos, out Chunk chunk))
        {
            int blockLocalX = worldPos.X % Chunk.Width;
            int blockLocalZ = worldPos.Z % Chunk.Depth;
            int blockLocalY = worldPos.Y % Chunk.Height;

            if (chunk.TryGetBlock(new Vector3(blockLocalX, blockLocalY, blockLocalZ), out block))
            {
                return true;
            }

            return false;
        }

        block = null;
        return false;
    }
    

    public List<BlockState> GetBlocksInArea(Vector3Int min, Vector3Int max)
    {
        List<BlockState> blocks = new List<BlockState>();
        
        for (int x = min.X; x <= max.X; x++)
        {
            for (int z = min.Z; z <= max.Z; z++)
            {
                for (int y = min.Y; y <= max.Y; y++)
                {
                    if (TryGetBlock(new  Vector3Int(x, y, z), out BlockState block))
                        blocks.Add(block);
                }
            }
        }
        
        return blocks;
    }
    public List<BlockState> GetCollidableBlocksInArea(BoxCollider box)
    {
        List<BlockState> blocks = new List<BlockState>();
        
        int minX = (int)Math.Floor(box.WorldMin.X);
        int maxX = (int)box.WorldMax.X;
        int minZ = (int)Math.Floor(box.WorldMin.Z);
        int maxZ = (int)box.WorldMax.Z;
        int minY = (int)Math.Floor(box.WorldMin.Y);
        int maxY = (int)box.WorldMax.Y;
        
        for (int x = minX; x <= maxX; x++)
        {
            for (int z = minZ; z <= maxZ; z++)
            {
                for (int y = minY; y <= maxY; y++)
                {
                    if (TryGetBlock(new  Vector3Int(x, y, z), out BlockState block) && block.Collidable)
                        blocks.Add(block);
                }
            }
        }
        
        return blocks;
    }
    
    public void SetBlock(Vector3Int worldPos, BlockState block, ChunkUpdateFlags updateFlags = ChunkUpdateFlags.None)
    {
        if (TryGetChunk(worldPos, out Chunk chunk))
        {
            int blockLocalX = worldPos.X % Chunk.Width;
            int blockLocalZ = worldPos.Z % Chunk.Depth;
            int blockLocalY = worldPos.Y % Chunk.Height;

            if (chunk.TryGetBlock(new Vector3(blockLocalX, blockLocalY, blockLocalZ), out var block2))
            {
                if (block2.Block == Minecraft.Instance.Bedrock)
                    return;
            }
            
            chunk.SetBlock(new Vector3(blockLocalX, blockLocalY, blockLocalZ), block);
            chunk.Update(updateFlags);
        }
    }
    
    public void ClearBlocksInRadius(Vector3Int worldPos, int radius)
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
            chunk.Draw();
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