using Microsoft.Xna.Framework;
using MinecraftClone.Core.Numerics;

namespace MinecraftClone.Core.Level;

public class BlockState : WorldObject
{
    public BlockState(Block block, Vector3 position, Level level) : base(position, BoxCollider.Full, level)
    {
        Block = block;
    }
    
    public Vector3Int ChunkPosition;

    public Block Block;
}