using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MinecraftClone.Core.Level;
using MinecraftClone.Core.Level.Chunk;
using MinecraftClone.Core.Model;
using MinecraftClone.Core.Numerics;

namespace MinecraftClone.Core;

public class Minecraft : Game
{
    public enum CameraMode
    {
        FreeCamera,
        FirstPerson
    }
    
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _uiSpriteBatch;
    public static Minecraft Instance;

    public Logger Logger;
    
    private BasicEffect basicEffect;

    SpriteFont font;
    
    private bool orbit;
    
    private Camera camera;

    private Texture2D cobblestone;

    //private Mesh<VertexPositionColorTexture> cubeAll;
    
    private VertexBuffer vertexBuffer;
    private IndexBuffer indexBuffer;
    
    public Minecraft()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
        Logger = new Logger("Minecraft");
        
        TargetElapsedTime = TimeSpan.FromSeconds(0.0045f);
    }

    private Chunk chunk;

    protected override void Initialize()
    {
        Instance = this;
        Logger.Log("Initializing Minecraft...");
        camera = new Camera(new Vector3(2, 1, 5), -22.5f, 10.5f);
        basicEffect = new BasicEffect(_graphics.GraphicsDevice);
        basicEffect.Alpha = 1;
        basicEffect.TextureEnabled = true;
        basicEffect.VertexColorEnabled = true;
        basicEffect.LightingEnabled = false;
        basicEffect.FogEnabled = false;
        
        int centerX = GraphicsDevice.Viewport.Width / 2;
        int centerY = GraphicsDevice.Viewport.Height / 2;

        chunk = new Chunk(Vector3.Zero);
        
        for (int x = 0; x < 16; x++)
        {
            for (int z = 0; z < 16; z++)
            {
                for (int y = 0; y < 16; y++)
                {
                    chunk.SetBlock(new Vector3Int(x,y,z), new BlockState());
                }
            }
        }
        
        Mouse.SetPosition(centerX, centerY);
        oldMouseState = Mouse.GetState();
        base.Initialize();
    }

    protected override void LoadContent()
    {
        _uiSpriteBatch = new SpriteBatch(GraphicsDevice);
        font = Content.Load<SpriteFont>("fonts/font");
        cobblestone = Content.Load<Texture2D>("textures/cobblestone");
        
        /*VertexPositionColor[] vertices =
        [
            // FRONT
            new (new Vector3(-0.5f, 0.5f, -0.5f), Color.White),
            new (new Vector3(-0.5f, -0.5f, -0.5f), Color.White),
            new (new Vector3(0.5f, -0.5f, -0.5f), Color.White),
            new (new Vector3(0.5f, -0.5f, -0.5f), Color.White),
            new (new Vector3(0.5f, 0.5f, -0.5f), Color.White),
            new (new Vector3(-0.5f, 0.5f, -0.5f), Color.White),
            // TOP
            new (new Vector3(-0.5f, 0.5f, 0.5f), Color.White),
            new (new Vector3(-0.5f, 0.5f, -0.5f), Color.White ),
            new (new Vector3(0.5f, 0.5f, -0.5f), Color.White),
            new (new Vector3(0.5f, 0.5f, -0.5f), Color.White),
            new (new Vector3(0.5f, 0.5f, 0.5f), Color.White),
            new (new Vector3(-0.5f, 0.5f, 0.5f), Color.White),
            // BOTTOM
            new (new Vector3(0.5f, -0.5f, -0.5f), Color.White),
            new (new Vector3(-0.5f, -0.5f, -0.5f), Color.White),
            new (new Vector3(-0.5f, -0.5f, 0.5f), Color.White),
            new (new Vector3(-0.5f, -0.5f, 0.5f), Color.White),
            new (new Vector3(0.5f, -0.5f, 0.5f), Color.White),
            new (new Vector3(0.5f, -0.5f, -0.5f), Color.White),
            // BACK
            new (new Vector3(-0.5f, 0.5f, 0.5f), Color.White),
            new (new Vector3(0.5f, 0.5f, 0.5f), Color.White),
            new (new Vector3(0.5f, -0.5f, 0.5f), Color.White),
            new (new Vector3(0.5f, -0.5f, 0.5f), Color.White),
            new (new Vector3(-0.5f, -0.5f, 0.5f), Color.White),
            new (new Vector3(-0.5f, 0.5f, 0.5f), Color.White),
            // RIGHT
            new (new Vector3(-0.5f, 0.5f, 0.5f), Color.White),
            new (new Vector3(-0.5f, -0.5f, 0.5f), Color.White),
            new (new Vector3(-0.5f, -0.5f, -0.5f), Color.White),
            new (new Vector3(-0.5f, -0.5f, -0.5f), Color.White),
            new (new Vector3(-0.5f, 0.5f, -0.5f), Color.White),
            new (new Vector3(-0.5f, 0.5f, 0.5f), Color.White),
            // LEFT
            new (new Vector3(0.5f, 0.5f, 0.5f), Color.White),
            new (new Vector3(0.5f, 0.5f, -0.5f), Color.White),
            new (new Vector3(0.5f, -0.5f, -0.5f), Color.White),
            new (new Vector3(0.5f, -0.5f, -0.5f), Color.White),
            new (new Vector3(0.5f, -0.5f, 0.5f), Color.White),
            new (new Vector3(0.5f, 0.5f, 0.5f), Color.White),
        ];
        
        ushort[] indices = new ushort[vertices.Length];
        List<VertexPositionColor> vertexList = new List<VertexPositionColor>();
        Dictionary<VertexPositionColor, ushort> vertexToIndex = new Dictionary<VertexPositionColor, ushort>();
        for (int i = 0; i < vertices.Length; i++)
        {
            VertexPositionColor vertex = vertices[i];
            if (vertexToIndex.TryGetValue(vertex, out ushort index))
            {
                indices[i] = index;
            }
            else
            {
                ushort newIndex = (ushort)vertexList.Count;
                vertexToIndex.Add(vertex, newIndex);
                vertexList.Add(vertex);
                indices[i] = newIndex;
            }
        }
        
        vertexBuffer = new VertexBuffer(GraphicsDevice, typeof(VertexPositionColor), vertices.Length, BufferUsage.WriteOnly);
        vertexBuffer.SetData(vertices);
        indexBuffer = new IndexBuffer(GraphicsDevice, typeof(ushort), indices.Length, BufferUsage.WriteOnly);
        indexBuffer.SetData(indices);*/
        
        //cubeAll = Mesh.GetCubeAll();
        
        basicEffect.Texture = cobblestone;
    }

    protected override void UnloadContent()
    {
        //cubeAll.Dispose();
        basicEffect.Dispose();
    }

    private bool mouseLocked = true;
    private MouseState oldMouseState;

    private double fps;

    private float movementSpeed = 5;
    
    List<WorldObject> worldObjects = new List<WorldObject>();
    
    protected override void Update(GameTime gameTime)
    {
        if (IsActive)
        {
            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
            
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            if (mouseLocked)
            {
                IsMouseVisible = false;
                MouseState currentMouseState = Mouse.GetState();
                
                float mouseDeltaX = currentMouseState.X - oldMouseState.X;
                float mouseDeltaY = currentMouseState.Y - oldMouseState.Y;

                camera.Yaw += mouseDeltaX * camera.CameraSensitivity;
                camera.Pitch += mouseDeltaY * camera.CameraSensitivity;

                if (currentMouseState.X < 0 || currentMouseState.Y < 0 ||
                    currentMouseState.X > GraphicsDevice.Viewport.Width ||
                    currentMouseState.Y > GraphicsDevice.Viewport.Height)
                {
                    Mouse.SetPosition(GraphicsDevice.Viewport.Width / 2, GraphicsDevice.Viewport.Height / 2);
                    oldMouseState = new MouseState(
                        GraphicsDevice.Viewport.Width / 2,
                        GraphicsDevice.Viewport.Height / 2,
                        currentMouseState.ScrollWheelValue,
                        currentMouseState.LeftButton,
                        currentMouseState.RightButton,
                        currentMouseState.MiddleButton,
                        currentMouseState.XButton1,
                        currentMouseState.XButton2
                        );
                }
                else
                {
                    oldMouseState = currentMouseState;
                }
            }
            else
            {
                IsMouseVisible = true;
            }
            
            Vector3 movementVector = new Vector3(0, 0, 0);
            if (Keyboard.GetState().IsKeyDown(Keys.W))
            {
                movementVector += new Vector3(0, 0, -1);
            }
            if (Keyboard.GetState().IsKeyDown(Keys.S))
            {
                movementVector += new Vector3(0, 0, 1);
            }
            if (Keyboard.GetState().IsKeyDown(Keys.A))
            {
                movementVector += new Vector3(-1, 0, 0);
            }
            if (Keyboard.GetState().IsKeyDown(Keys.D))
            {
                movementVector += new Vector3(1, 0, 0);
            }
            

            if (movementVector != Vector3.Zero)
            {
                movementVector = Vector3.Normalize(movementVector);
                movementVector = Vector3.TransformNormal(movementVector, camera.GetRotationMatrix());

                camera.Position += movementVector * dt * movementSpeed;
            }
        }
        else
        {
            IsMouseVisible = true;
        }
        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        fps = 1.0 / gameTime.ElapsedGameTime.TotalSeconds;
        GraphicsDevice.Clear(Color.CornflowerBlue);
        GraphicsDevice.DepthStencilState = DepthStencilState.Default;
        GraphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;
        
        basicEffect.Projection = camera.GetProjectionMatrix();
        basicEffect.View = camera.GetViewMatrix();

        /*foreach (WorldObject worldObject in worldObjects)
        {
            basicEffect.World = camera.GetWorldMatrix(worldObject);

            //cubeAll.Draw(basicEffect);
        }*/
        
        chunk.Draw(basicEffect);

        /*GraphicsDevice.SetVertexBuffer(vertexBuffer);
        GraphicsDevice.Indices = indexBuffer;

        GraphicsDevice.SamplerStates[0] = SamplerState.PointClamp;

        foreach (var pass in basicEffect.CurrentTechnique.Passes)
        {
            pass.Apply();
            //GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleList, 0, vertexBuffer.VertexCount / 3);
            GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, indexBuffer.IndexCount / 3);
        }*/
        
        _uiSpriteBatch.Begin();
        // FPS Counter
        _uiSpriteBatch.DrawString(font, fps.ToString("F1"), new Vector2(0, 0), Color.White);
        _uiSpriteBatch.DrawString(font, camera.Position.ToString(), new Vector2(0, 15), Color.White);
        _uiSpriteBatch.End();
            
        base.Draw(gameTime);
    }
}