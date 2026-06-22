using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MinecraftClone.Core.Debug;
using MinecraftClone.Core.Level;
using MinecraftClone.Core.Level.Chunk;
using MinecraftClone.Core.World;
using MinecraftClone.Core.Mods;
using MinecraftClone.Core.Numerics;
using Model = MinecraftClone.Core.World.Model;

namespace MinecraftClone.Core;

public class Minecraft : Game
{
    public enum CameraMode
    {
        FreeCamera,
        FirstPerson
    }
    
    private GraphicsDeviceManager _graphics;
    public SpriteBatch SpriteBatch;
    public static Minecraft Instance;

    public Logger Logger;
    
    public BasicEffect BasicEffect => basicEffect;
    private BasicEffect basicEffect;

    SpriteFont font;
    
    private bool orbit;
    
    private Camera camera;

    //private Mesh<VertexPositionColorNormalTexture> cubeAll;
    
    private VertexBuffer vertexBuffer;
    private IndexBuffer indexBuffer;
    
    private ModLoader modLoader;
    
    public Minecraft()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
        Logger = new Logger("Minecraft");
        
        Window.AllowUserResizing = true;
        Window.ClientSizeChanged += OnWindowResized;
        
        TargetElapsedTime = TimeSpan.FromMilliseconds(1.0f);
        modLoader = new ModLoader();
    }

    ~Minecraft()
    {
        Window.ClientSizeChanged -= OnWindowResized;
    }
    
    private Chunk chunk;

    
    private Vector3 ambientLightColor = new Vector3(0.6f, 0.6f, 0.6f);
    protected override void Initialize()
    {
        Instance = this;
        Logger.Log("Initializing Minecraft...");
        camera = new Camera(new Vector3(2, 80, 20), 0f, 0);
        basicEffect = new BasicEffect(_graphics.GraphicsDevice);
        basicEffect.Alpha = 1;
        basicEffect.TextureEnabled = true;
        basicEffect.VertexColorEnabled = true;
        basicEffect.LightingEnabled = true;
        basicEffect.FogEnabled = false;
        basicEffect.AmbientLightColor = ambientLightColor;
        basicEffect.DirectionalLight0.Enabled = true;
        basicEffect.DirectionalLight0.Direction = new Vector3(1, -2, -0.35f);
        basicEffect.DirectionalLight0.DiffuseColor = new Vector3(0.4f, 0.4f, 0.4f);
        basicEffect.DirectionalLight0.SpecularColor = new Vector3(0.0f, 0.0f, 0.0f);
        
        int centerX = GraphicsDevice.Viewport.Width / 2;
        int centerY = GraphicsDevice.Viewport.Height / 2;
        
        Mouse.SetPosition(centerX, centerY);
        oldMouseState = Mouse.GetState();
        modLoader.Initialize();
        base.Initialize();
    }

    private void OnWindowResized(object sender, EventArgs e)
    {
        camera?.MarkDirty();
    }

    private float averageFPS;
    private float sinceLastFlush = 0;
    
    
    private static Identifier cobblestoneId = Identifier.WithDefaultNamespace("cobblestone");
    private static Identifier dirtId  = Identifier.WithDefaultNamespace("dirt");
    private static Identifier grassSideId  = Identifier.WithDefaultNamespace("grass_block_side");
    private static Identifier grassTopId  = Identifier.WithDefaultNamespace("grass_block_top");
    private static Identifier grassId  = Identifier.WithDefaultNamespace("grass");
    public Block Cobblestone;
    public Block Dirt;
    public Block GrassBlock;
    
    protected override void LoadContent()
    {
        SpriteBatch = new SpriteBatch(GraphicsDevice);
        font = Content.Load<SpriteFont>("fonts/font");
        
        Atlas = new TextureAtlas(Identifier.WithDefaultNamespace("atlas"), 16, 4, 4); // Start with a max of 16 textures
        Atlas.AddTexture(Content.Load<Texture2D>("textures/cobblestone"), cobblestoneId);
        Atlas.AddTexture(Content.Load<Texture2D>("textures/dirt"), dirtId);
        Atlas.AddTexture(Content.Load<Texture2D>("textures/grass_block_side"), grassSideId);
        Atlas.AddTexture(Content.Load<Texture2D>("textures/grass_block_top"), grassTopId);
        
        Dirt = new Block(Model.CubeAll(Vector3.Zero, Vector3.One, dirtId, Color.White, dirtId));
        Cobblestone = new Block(Model.CubeAll(Vector3.Zero, Vector3.One, cobblestoneId, Color.White, cobblestoneId));
        GrassBlock = new Block(Model.CubeBottomTopSides(Vector3.Zero, Vector3.One, grassTopId,dirtId, grassSideId, Color.White, grassId));

        Level = new();
    }
    
    protected override void UnloadContent()
    {
        //cubeAll.Dispose();
        basicEffect.Dispose();
    }

    private bool mouseLocked = true;
    private MouseState oldMouseState;

    private double fps;

    private float movementSpeed = 30;
    
    List<WorldObject> worldObjects = new List<WorldObject>();
    
    KeyboardState oldKeyboardState;
    KeyboardState newKeyboardState;
    
    public TextureAtlas Atlas;
    
    protected override void Update(GameTime gameTime)
    {
        if (IsActive)
        {
            newKeyboardState = Keyboard.GetState();

            if (newKeyboardState.IsKeyDown(Keys.Escape) && oldKeyboardState.IsKeyUp(Keys.Escape))
            {
                mouseLocked = !mouseLocked;
            }
            
            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

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

            if (KeyDown(Keys.OemPlus))
            {
                showAtlas = !showAtlas;
            }

            if (movementVector != Vector3.Zero)
            {
                movementVector = Vector3.Normalize(movementVector);
                movementVector = Vector3.TransformNormal(movementVector, camera.GetRotationMatrix());

                camera.Position += movementVector * dt * movementSpeed;
            }
            
            ModLoader.Instance.UpdateAllMods(gameTime);
            oldKeyboardState = Keyboard.GetState();
            
        }
        else
        {
            IsMouseVisible = true;
        }
        base.Update(gameTime);
    }

    FrameCounter frameCounter = new();
    
    private bool KeyDown(Keys key)
    {
        return newKeyboardState.IsKeyDown(key) && oldKeyboardState.IsKeyUp(key);
    }private bool KeyUp(Keys key)
    {
        return newKeyboardState.IsKeyUp(key) && oldKeyboardState.IsKeyDown(key);
    }

    private bool showAtlas = false;

    public Level.Level Level;
    
    protected override void Draw(GameTime gameTime)
    {
        if (Atlas.Dirty)
        {
            Atlas.Bake();
        }
        
        frameCounter.Update((float)gameTime.ElapsedGameTime.TotalSeconds);
        
        GraphicsDevice.Clear(Color.CornflowerBlue);
        GraphicsDevice.DepthStencilState = DepthStencilState.Default;
        GraphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;
        
        SamplerState samplerState = new SamplerState();
        samplerState.MaxMipLevel = 5;
        samplerState.Filter = TextureFilter.Point;
        samplerState.AddressU = TextureAddressMode.Clamp;
        samplerState.AddressV = TextureAddressMode.Clamp;
        samplerState.AddressW = TextureAddressMode.Clamp;
        
        GraphicsDevice.SamplerStates[0] = samplerState;
        
        basicEffect.Projection = camera.GetProjectionMatrix();
        basicEffect.View = camera.GetViewMatrix();
        basicEffect.Texture = Atlas._renderTarget;
        
        /*foreach (WorldObject worldObject in worldObjects)
        {
            basicEffect.World = camera.GetWorldMatrix(worldObject);

            //cubeAll.Draw(basicEffect);
        }*/

        Level.Draw(gameTime);
        
        ModLoader.Instance.DrawAllModsMesh(gameTime);
        
        /*GraphicsDevice.SetVertexBuffer(vertexBuffer);
        GraphicsDevice.Indices = indexBuffer;

        GraphicsDevice.SamplerStates[0] = SamplerState.PointClamp;

        foreach (var pass in basicEffect.CurrentTechnique.Passes)
        {
            pass.Apply();
            //GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleList, 0, vertexBuffer.VertexCount / 3);
            GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, indexBuffer.IndexCount / 3);
        }*/
        
        SpriteBatch.Begin();
        // FPS Counter
        SpriteBatch.DrawString(font, frameCounter.AverageFramesPerSecond.ToString("F1"), new Vector2(0, 0), Color.White);
        SpriteBatch.DrawString(font, camera.Position.ToString(), new Vector2(0, 15), Color.White);
        SpriteBatch.DrawString(font, $"yaw: {camera.Yaw}, pitch: {camera.Pitch}", new Vector2(0, 30), Color.White);
        // Atlas
        if (showAtlas)
        {
            Color[] rawPixels = new Color[Atlas.AtlasWidth * Atlas.AtlasHeight];
            Atlas._renderTarget.GetData(rawPixels);
            if (rawPixels.Any(color => color.A > 0))
            {
                Logger.Debug("Has colors!");
            }
            Logger.Debug("Drawing atlas");
            SpriteBatch.Draw(Atlas._renderTarget, new Rectangle(0, 0, Atlas.AtlasWidth * 2, Atlas.AtlasHeight * 2), Color.White);
        }
        
        SpriteBatch.End();
        
        
        ModLoader.Instance.DrawAllModsUi(gameTime);
            
        base.Draw(gameTime);
    }
}