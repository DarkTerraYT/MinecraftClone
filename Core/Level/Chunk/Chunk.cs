using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MinecraftClone.Core.Model;
using MinecraftClone.Core.Numerics;

namespace MinecraftClone.Core.Level.Chunk;

public class Chunk : IDisposable, IDrawable
{
    public BlockState[,,] Blocks { get; set; }

    public readonly Vector3Int Position;

    private Mesh<VertexPositionColorTexture> mesh; // Position, Color, UV
    
    private bool updated = true;
    
    public Chunk(Vector3Int position)
    {
        Position = position;
        Blocks = new BlockState[16, 255, 16];
    }

    public bool TryGetBlock(Vector3Int localPosition, out BlockState block)
    {
        if (localPosition.X >= 16 || localPosition.Z >= 16 || localPosition.Y >= 16 || localPosition.X < 0 || localPosition.Z < 0 || localPosition.Y < 0)
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
        if (localPosition.X > 15 || localPosition.Y > 254 || localPosition.Z > 15)
        {
            Minecraft.Instance.Logger.Warn("Tried to set a block in a chunk outside of the bounds of the chunk.");
            return;
        }
        
        Blocks[localPosition.X, localPosition.Y, localPosition.Z] = block;
        updated = true;
    }

    private void Draw(Effect effect)
    {
        if (updated)
        {
            mesh = ChunkGenerator.GenerateChunkMesh(this);
            Minecraft.Instance.Logger.Debug("Chunk generated");
            Minecraft.Instance.Logger.Debug(mesh.Vertices.Length);
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