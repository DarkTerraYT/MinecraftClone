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

    private BoxCollider collider;

    public BoxCollider CollisionBox
    {
        get
        {
            if (collider == null)
            {
                collider = Block.Collider.Clone();
                collider.Position = WorldPosition;
            }
            return collider;
        }
    }
}