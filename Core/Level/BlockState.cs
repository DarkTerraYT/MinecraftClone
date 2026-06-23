using Microsoft.Xna.Framework;
using MinecraftClone.Core.Numerics;

namespace MinecraftClone.Core.Level;

public class BlockState : WorldObject
{
    public BlockState(Block block, Vector3 position) : base(position, CubeShape.Full)
    {
        Block = block;
    }
    
    public Vector3Int ChunkPosition;

    public Block Block;
}