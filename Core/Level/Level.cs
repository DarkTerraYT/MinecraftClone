using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using MinecraftClone.Core.Numerics;

namespace MinecraftClone.Core.Level;

public class Level: IDrawable, IDirtyable
{
    public Dictionary<Point, Chunk.Chunk> Chunks = new(9);

    private FastNoiseLite noise;

    private const int SeaLevel = 60;

    private const int WorldWidth = 5;
    private const int WorldDepth = 5;
    
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
        else
        {
            block = null;
            return false;
        }
    }
    
    public Level()
    {
        noise = new FastNoiseLite();
        noise.SetNoiseType(FastNoiseLite.NoiseType.OpenSimplex2);
        
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
                        for (int y = 0; y < SeaLevel + yOffset; y++)
                        {
                            float y2 = noise.GetNoise(x + (i * 16), y, z + (j * 16));
                            
                            if (y2 > 0.75f)
                                continue;
                            
                            if (y < SeaLevel - 4 + yOffset)
                                chunk.SetBlock(new Vector3Int(x,y,z), new BlockState(Minecraft.Instance.Cobblestone));
                            else if (y < SeaLevel - 1 + yOffset)
                                chunk.SetBlock(new Vector3Int(x,y,z), new BlockState(Minecraft.Instance.Dirt));
                            else 
                                chunk.SetBlock(new Vector3Int(x,y,z), new BlockState(Minecraft.Instance.GrassBlock));
                        }
                    }
                }
                
                Chunks.Add(new Point(i, j), chunk);
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