using Microsoft.Xna.Framework;

namespace MinecraftClone.Core.Level;

public class WorldObject
{
    public WorldObject(Vector3 position, BoxCollider collider, Level level)
    {
        CollisionBox = collider;
        Position = position;
        CollisionBox.Position = position;
        Level = level;
    }
    public BoxCollider CollisionBox { get; }

    public Level Level { get; }
    
    private Vector3 position;

    public Vector3 Position
    {
        get => position;
        set
        {
            position = value;
            CollisionBox.Position = value; // Move the collision box to the object's position
        }
    }

    public bool Collidable = true;
    
    public bool Intersects(WorldObject other) => CollisionBox.Intersects(other.CollisionBox);
}