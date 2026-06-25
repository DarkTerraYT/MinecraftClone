using System;
using Microsoft.Xna.Framework;

namespace MinecraftClone.Core.Level;

public class Entity : WorldObject, IUpdateable
{
    public Entity(Vector3 position, BoxCollider collider, Level level) : base(position, collider, level)
    {
    }

    public void Update(GameTime gameTime)
    {
        float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

        if (Collidable)
        {
            // Move on X
            Position = new Vector3(Velocity.X * deltaTime + Position.X, Position.Y, Position.Z);

            // Then check collisions
            foreach (var block in Level.GetCollidableBlocksInArea(CollisionBox))
            {
                if (block.Intersects(this))
                {
                    if (Velocity.X > 0)
                    {
                        Position = new Vector3(block.CollisionBox.WorldMin.X - CollisionBox.Width, Position.Y,
                            Position.Z);
                    }
                    else if (Velocity.X < 0)
                    {
                        Position = new Vector3(block.CollisionBox.WorldMax.X, Position.Y,
                            Position.Z);
                    }
                }
            }

            // Then move on Y 
            Position = new Vector3(Position.X, Position.Y + Velocity.Y * deltaTime, Position.Z);

            IsGrounded = false;

            // Then check collisions
            foreach (var block in Level.GetCollidableBlocksInArea(CollisionBox))
            {
                if (block.Intersects(this))
                {
                    if (Velocity.Y > 0)
                    {
                        Position = new Vector3(Position.X, block.CollisionBox.WorldMin.Y - CollisionBox.Height, Position.Z);
                    }
                    else if (Velocity.Y < 0)
                    {
                        Position = new Vector3(Position.X, block.CollisionBox.WorldMax.Y, Position.Z);
                        IsGrounded = true;
                    }

                    Velocity.Y = 0;
                }
                break;
            }

            // Then move on Z
            Position = new Vector3(Position.X, Position.Y, Position.Z + Velocity.Z * deltaTime);

            // Then check collisions
            foreach (var block in Level.GetCollidableBlocksInArea(CollisionBox))
            {
                if (block.Intersects(this))
                {
                    if (Velocity.Z > 0)
                    {
                        Position = new Vector3(Position.X, Position.Y,
                            block.CollisionBox.WorldMin.Z - CollisionBox.Depth);
                    }
                    else if (Velocity.Z < 0)
                    {
                        Position = new Vector3(Position.X, Position.Y,
                            block.CollisionBox.WorldMax.Z);
                    }

                    Velocity.Z = 0;
                }
                break;
            }
        }
        else
        {
            Position = new Vector3(Position.X + Velocity.X * deltaTime, Position.Y + Velocity.Y * deltaTime, Position.Z + Velocity.Z * deltaTime);
        }
    }

    public Vector3 Velocity = Vector3.Zero;
    
    public bool IsGrounded = false;
    
    public bool Enabled { get; }
    public int UpdateOrder { get; }
    public event EventHandler<EventArgs> EnabledChanged;
    public event EventHandler<EventArgs> UpdateOrderChanged;
}