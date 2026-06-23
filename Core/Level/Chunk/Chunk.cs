using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MinecraftClone.Core.World;
using MinecraftClone.Core.Numerics;

namespace MinecraftClone.Core.Level.Chunk;

public class Chunk : IDisposable, IDrawable
{
    public BlockState[,,] Blocks { get; set; }

    public readonly Vector3Int Position;

    private Mesh<VertexPositionColorNormalTexture> mesh; // Position, Color, UV
    
    private bool updated = true;
    
    public Chunk(Vector3Int position)
    {
        Position = position;
        Blocks = new BlockState[16, 255, 16];
    }

    public bool TryGetBlockWorld(Vector3Int localPosition, out BlockState block)
    {
        return Minecraft.Instance.Level.TryGetBlock(Position + localPosition, out block);
    }
    
    public bool TryGetBlock(Vector3Int localPosition, out BlockState block)
    {
        if (localPosition.X >= 16 || localPosition.Z >= 16 || localPosition.Y >= 255 || localPosition.X < 0 || localPosition.Z < 0 || localPosition.Y < 0)
        {
            block = null;
            return false;
        }
        
        block = GetBlock(localPosition);
        if (block == null)
        {
            return false;
        }
        return true;
    }

    public BlockState GetBlock(Vector3Int localPosition)
    {
        return Blocks[localPosition.X, localPosition.Y, localPosition.Z];
    }

    public void SetBlock(Vector3Int localPosition, BlockState block)
    {
        if (localPosition.X >= 16 || localPosition.Z >= 16 || localPosition.Y >= 255 || localPosition.X < 0 || localPosition.Z < 0 || localPosition.Y < 0)
        {
            return;
        }
        
        //Minecraft.Instance.Logger.Debug($"Setting block at {localPosition}");
        
        Blocks[localPosition.X, localPosition.Y, localPosition.Z] = block;
        updated = true;
    }

    private void Draw(Effect effect)
    {
        if (updated)
        {
            mesh = ChunkGenerator.GenerateChunkMesh(this);
            updated = false;
        }
        
        mesh.Draw(effect);
    }

    public void Dispose()
    {
        mesh?.Dispose();
    }

    public void Draw(GameTime gameTime)
    {
        Draw(Minecraft.Instance.BasicEffect);
    }

    public int DrawOrder { get; }
    public bool Visible { get; }
    public event EventHandler<EventArgs> DrawOrderChanged;
    public event EventHandler<EventArgs> VisibleChanged;
}