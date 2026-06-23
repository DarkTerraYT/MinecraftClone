using Microsoft.Xna.Framework;

namespace MinecraftClone.Core.Level;

public class WorldObject
{
    public WorldObject(Vector3 position, CubeShape shape)
    {
        CollisionBox = shape;
        Position = position;
        CollisionBox.Position = position;
    }
    public CubeShape CollisionBox { get; }

    public Vector3 Position;
}