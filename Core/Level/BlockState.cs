using MinecraftClone.Core.Numerics;

namespace MinecraftClone.Core.Level;

public class BlockState : WorldObject
{
    public Vector3Int ChunkPosition;

    private Identifier blockId;
}