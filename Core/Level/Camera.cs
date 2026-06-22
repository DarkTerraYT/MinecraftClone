using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MinecraftClone.Core.Level;

public class Camera : IDirtyable
{
    public Camera(Vector3 position, float pitch, float yaw, float fovY = 60.0f)
    {
        Yaw = yaw;
        Pitch = pitch;
        Position = position;
        FovY = fovY;
        
        this.graphicsDevice = Minecraft.Instance.GraphicsDevice;
    }

    public bool Dirty => projectionChanged || viewChanged || rotationChanged;

    public void MarkDirty()
    {
        projectionChanged = true;
        viewChanged = true;
        rotationChanged = true;
    }

    private Vector3 target;

    private bool projectionChanged = false;
    private bool viewChanged = false;
    private bool rotationChanged = false;
    
    private float pitch = 0.0f;

    public float Pitch
    {
        get => pitch;
        set
        {
            pitch = Math.Clamp(value, -80.0f, 80.0f);
            rotationChanged = true;
            viewChanged = true;
        }
    } 
    private float yaw = 0.0f;

    public float Yaw
    {
        get => yaw;
        set
        {
            yaw = value;
            rotationChanged = true;
            viewChanged = true;
        }
    }

    public float CameraSensitivity = 0.2f;
    
    private Vector3 position { get; set; }
    public Vector3 Position
    {
        get => position;
        set
        {
            position = value;
            viewChanged = true;
        }
    }

    private Vector2 oldMousePosition = Vector2.Zero;

    private readonly GraphicsDevice graphicsDevice;
    
    private float fovY;
    
    public float FovY
    {
        get => fovY;
        set
        {
            fovY = value;
            projectionChanged = true;
        }
    }

    private Matrix projectionMatrix;
    private Matrix viewMatrix;
    private Matrix rotationMatrix;
    
    public Matrix GetWorldMatrix(WorldObject worldObject)
    {
        return Matrix.CreateWorld(worldObject.Position, Vector3.Forward, Vector3.Up);
    }

    public Matrix GetRotationMatrix()
    {
        if (rotationChanged)
        {
            rotationMatrix = Matrix.CreateFromYawPitchRoll(MathHelper.ToRadians(Yaw), MathHelper.ToRadians(Pitch), 0.0f);
            rotationChanged = false;
        }
        return rotationMatrix;
    }
    
    public Matrix GetViewMatrix()
    {
        if (viewChanged)
        {
            Vector3 direction = Vector3.TransformNormal(Vector3.Forward, GetRotationMatrix());
            target = Position + direction;
            
            viewMatrix = Matrix.CreateLookAt(Position, target, Vector3.Up);
            viewChanged = false;
        }
        
        return viewMatrix;
    }
    public Matrix GetProjectionMatrix()
    {
        if (projectionChanged)
        {
            projectionMatrix = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(FovY), graphicsDevice.Viewport.AspectRatio, 0.05f, 1000.0f);
            projectionChanged = false;
        }
        
        return projectionMatrix;
    }

    public void Update(GameTime gameTime)
    {
    }
}