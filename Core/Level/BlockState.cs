using MinecraftClone.Core.Numerics;

namespace MinecraftClone.Core.Level;

public class BlockState(Block block) : WorldObject
{
    public Vector3Int ChunkPosition;

    public Block Block = block;
}