using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MinecraftClone.Core.Extension;
using MinecraftClone.Core.Numerics;
using MinecraftClone.Core.World;

namespace MinecraftClone.Core.Level;

public class Player : Entity
{
    public enum CameraMode
    {
        FreeCamera,
        FirstPerson
    }
    
    public Player(Vector3 position, Level level) : base(position, new BoxCollider(new Vector3(0.6f, 2, 0.6f), Vector3.Zero), level)
    {
        camera = new Camera(Position, 0f, 0);
    }
    
    private float jumpPower = 7;
    private float movementSpeed = 4.317f;
    private float freeCamSpeed = 30.0f;
    
    public CameraMode cameraMode = CameraMode.FirstPerson;
    
    public Camera camera;
    
    public void Update(GameTime gameTime)
    {
        float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
        float oldVelocityY = Velocity.Y;

        if (Minecraft.Instance.KeyDown(Keys.LeftAlt))
        {
            if (cameraMode == CameraMode.FirstPerson)
            {
                cameraMode = CameraMode.FreeCamera;
                Collidable = false;
            }
            else
            {
                cameraMode = CameraMode.FirstPerson;
                Collidable = true;
            }
        }
        
        Velocity.X = 0;
        Velocity.Z = 0;
        Velocity.Y = 0;
        
        if (Keyboard.GetState().IsKeyDown(Keys.W))
        {
            Velocity += new Vector3(0, 0, -1);
        }
        if (Keyboard.GetState().IsKeyDown(Keys.S))
        {
            Velocity += new Vector3(0, 0, 1);
        }  
        if (Keyboard.GetState().IsKeyDown(Keys.A))
        {
            Velocity += new Vector3(-1, 0, 0);
        }
        if (Keyboard.GetState().IsKeyDown(Keys.D))
        {
            Velocity += new Vector3(1, 0, 0);
        }

            
        if (Velocity != Vector3.Zero)
        {
            Velocity = Vector3.Normalize(Velocity);
        }
        
        Velocity = Vector3.TransformNormal(Velocity, cameraMode == CameraMode.FreeCamera ? camera.GetRotationMatrix() : camera.GetHorizontalRotationMatrix());
        Velocity *= cameraMode == CameraMode.FirstPerson ? movementSpeed : freeCamSpeed;
        if (cameraMode == CameraMode.FirstPerson) // Jumping and gravity shouldn't be in free cam
        {
            if (!IsGrounded)
            {
                oldVelocityY += Physics.Gravity * deltaTime;
            }
            else if (IsGrounded)
            {
                oldVelocityY = 0;
                if (Keyboard.GetState().IsKeyDown(Keys.Space))
                {
                    oldVelocityY = jumpPower;
                    IsGrounded = false;
                }
            }

            Velocity.Y = oldVelocityY;
        } 

        base.Update(gameTime); // Run the base entity movement and collision code
        
        camera.Position = Position + new Vector3(CollisionBox.Width / 2, CollisionBox.Height, CollisionBox.Depth / 2); // Keep camera at the same position as the player
    }
    
    public bool Enabled { get; }
    public int UpdateOrder { get; }
    public event EventHandler<EventArgs> EnabledChanged;
    public event EventHandler<EventArgs> UpdateOrderChanged;
}