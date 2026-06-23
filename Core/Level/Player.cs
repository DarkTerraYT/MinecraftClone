using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MinecraftClone.Core.Extension;
using MinecraftClone.Core.Numerics;
using MinecraftClone.Core.World;

namespace MinecraftClone.Core.Level;

public class Player : WorldObject, IUpdateable
{
    public enum CameraMode
    {
        FreeCamera,
        FirstPerson
    }
    
    public Player(Vector3 position) : base(position, new CubeShape(new Vector3(0.2f, -1, 0.2f), new Vector3(0.8f, 1, 0.8f), Vector3.Zero))
    {
        camera = new Camera(Position, 0f, 0);
    }
    
    private Vector3 velocity;
    public Vector3 Velocity => velocity;
    private const float gravity = -15;
    private const float jumpPower = 7;
    private float movementSpeed = 4.317f;
    
    public CameraMode cameraMode = CameraMode.FirstPerson;
    
    public Camera camera;

    private bool isGrounded = false;
    
    public void Update(GameTime gameTime)
    {
        float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
        float oldVelocityY = velocity.Y;

        if (Minecraft.Instance.KeyDown(Keys.LeftAlt))
        {
            if (cameraMode == CameraMode.FirstPerson)
            {
                cameraMode = CameraMode.FreeCamera;
            }
            else
            {
                cameraMode = CameraMode.FirstPerson;
            }
        }
        
        velocity.X = 0;
        velocity.Z = 0;
        velocity.Y = 0;
        if (Keyboard.GetState().IsKeyDown(Keys.W))
        {
            velocity += new Vector3(0, 0, -1);
        }
        if (Keyboard.GetState().IsKeyDown(Keys.S))
        {
            velocity += new Vector3(0, 0, 1);
        }  
        if (Keyboard.GetState().IsKeyDown(Keys.A))
        {
            velocity += new Vector3(-1, 0, 0);
        }
        if (Keyboard.GetState().IsKeyDown(Keys.D))
        {
            velocity += new Vector3(1, 0, 0);
        }

            
        if (velocity != Vector3.Zero)
        {
            velocity = Vector3.Normalize(velocity);
        }
        
        velocity = Vector3.TransformNormal(velocity, cameraMode == CameraMode.FreeCamera ? camera.GetRotationMatrix() : camera.GetHorizontalRotationMatrix());
        velocity *= cameraMode == CameraMode.FirstPerson ? movementSpeed : 30;
        if (cameraMode == CameraMode.FirstPerson)
        {
            if (!isGrounded)
            {
                oldVelocityY += gravity * dt;
            }
            else if (isGrounded)
            {
                oldVelocityY = 0;
                if (Keyboard.GetState().IsKeyDown(Keys.Space))
                {
                    oldVelocityY = jumpPower;
                    isGrounded = false;
                }
            }

            velocity.Y = oldVelocityY;
        }

        Position += velocity * dt;
        camera.Position = Position + new Vector3(0, 1, 0);
        CollisionBox.Position = Position;

        if (cameraMode == CameraMode.FirstPerson)
        {
            //isGrounded = Minecraft.Instance.Level.TryGetBlock(intPos - new Vector3Int(0, 1, 0), out var _);

            isGrounded = false;

            for (int x = -1; x < 2; x++)
            {
                for (int z = -1; z < 2; z++)
                {
                    for (int y = -1; y < 2; y++)
                    {
                        Vector3Int intPos = Position;
                        if (Minecraft.Instance.Level.TryGetBlock(intPos + new Vector3Int(x, y, z),
                                out BlockState block))
                        {
                            Face.Direction collisionDir = block.CollisionBox.IntersectionDirection(CollisionBox);
                            switch (collisionDir)
                            {
                                case Face.Direction.None:
                                    continue;
                                case Face.Direction.Top:
                                    Position.Y = block.Position.Y + 1 - CollisionBox.Min.Y;
                                    CollisionBox.Position = Position;
                                    isGrounded = true;
                                    continue;
                                case Face.Direction.Bottom:
                                    Position.Y = block.Position.Y - 1 - CollisionBox.Max.Y;
                                    CollisionBox.Position = Position;
                                    continue;
                                case Face.Direction.Right:
                                    Position.X = block.Position.X + 1 + CollisionBox.Min.X;
                                    CollisionBox.Position = Position;
                                    continue;
                                case Face.Direction.Left:
                                    Position.X = block.Position.X - CollisionBox.Max.X;
                                    CollisionBox.Position = Position;
                                    continue;
                                case Face.Direction.Back:
                                    Position.Y = block.Position.Z + 1 + CollisionBox.Max.Z;
                                    CollisionBox.Position = Position;
                                    continue;
                                case Face.Direction.Front:
                                    Position.Y = block.Position.Z - CollisionBox.Min.Z;
                                    CollisionBox.Position = Position;
                                    continue;
                            }
                        }
                    }
                }
            }
        }
    }
    
    public bool Enabled { get; }
    public int UpdateOrder { get; }
    public event EventHandler<EventArgs> EnabledChanged;
    public event EventHandler<EventArgs> UpdateOrderChanged;
}