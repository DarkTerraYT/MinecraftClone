using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MinecraftClone.Core.Numerics;
using MinecraftClone.Core.World;

namespace MinecraftClone.Core.Level;

public class Chunk : IDisposable
{
    public const int Width = 16;
    public const int Height = 16;
    public const int Depth = 16;

    private ushort[] Blocks;

    public readonly Vector3Int Position;

    private Mesh<VertexPositionColorNormalTexture> mesh; // Position, Color, UV
    
    public Chunk(Vector3Int position)
    {
        Position = position;
        Blocks = new ushort[Width + Height * Width + Depth * Width * Height];
    }
    
    private int ToArrayIndex(Vector3Int localPosition) => localPosition.X + localPosition.Y * Width + localPosition.Z *  Width * Height;
    
    public bool TryGetBlock(Vector3Int localPosition, out BlockState block)
    {
        if (localPosition.X >= Width || localPosition.Z >= Depth || localPosition.Y >= Height || localPosition.X < 0 || localPosition.Z < 0 || localPosition.Y < 0)
        {
            block = default;
            return false;
        }
        
        block = GetBlock(localPosition);
        if (block.IsAir())
        {
            return false;
        }
        return true;
    }

    public BlockState GetBlock(Vector3Int localPosition)
    {
        return new BlockState(Blocks[ToArrayIndex(localPosition)], Position + localPosition);
    }

    public void SetBlock(Vector3Int localPosition, Block block)
    {
        if (localPosition.X >= Width || localPosition.Z >= Depth || localPosition.Y >= Height || localPosition.X < 0 || localPosition.Z < 0 || localPosition.Y < 0)
        {
            return;
        }
        
        //Minecraft.Instance.Logger.Debug($"Setting block at {localPosition}");
        
        Blocks[ToArrayIndex(localPosition)] = Minecraft.Instance.Level.GetIdOrRegister(block);
    }

    private void Draw(Effect effect)
    {
        mesh.Draw(effect);
    }

    public void Draw() => Draw(Minecraft.Instance.BasicEffect);

    public void Update(ChunkUpdateFlags flags, Block ignoredBlock = null)
    {
        if (flags.HasFlag(ChunkUpdateFlags.OpaqueMesh))
        {
            mesh = ChunkGenerator.GenerateChunkMesh(this, ignoredBlock);
        }
    }

    public override string ToString() => $"Chunk {Position / 16}";
    public void Dispose()
    {
        mesh?.Dispose();
    }

    public int DrawOrder { get; }
    public bool Visible { get; }
    public event EventHandler<EventArgs> DrawOrderChanged;
    public event EventHandler<EventArgs> VisibleChanged;
}