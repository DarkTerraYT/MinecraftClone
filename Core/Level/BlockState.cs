using Microsoft.Xna.Framework;
using MinecraftClone.Core.Numerics;

namespace MinecraftClone.Core.Level;

public struct BlockState(ushort id, Vector3Int worldPos)
{
    public ushort BlockId { get; set; } = id;

    public Vector3Int WorldPosition { get; set; } = worldPos;

    public bool IsAir() => BlockId == 0;
    public bool Test(Block block) => BlockId == Minecraft.Instance.Level.GetIdOrRegister(block);

    public Block Block => Minecraft.Instance.Level.GetBlock(BlockId);

    public bool Intersects(WorldObject other) => CollisionBox.Intersects(other.CollisionBox);

    public BoxCollider CollisionBox => Block.Collider;
}