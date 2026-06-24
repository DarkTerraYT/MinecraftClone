using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MinecraftClone.Core.Extension;
using MinecraftClone.Core.Level;
using MinecraftClone.Core.Level.WorldGeneration;
using MinecraftClone.Core.Level.WorldGeneration.Passes;
using MinecraftClone.Core.World;
using MinecraftClone.Core.Mods;
using MinecraftClone.Core.Numerics;
using Model = MinecraftClone.Core.World.Model;

namespace MinecraftClone.Core;

public class Minecraft : Game
{
    
    private GraphicsDeviceManager _graphics;
    public SpriteBatch SpriteBatch;
    public static Minecraft Instance;

    public Logger Logger;
    
    public BasicEffect BasicEffect => basicEffect;
    private BasicEffect basicEffect;

    SpriteFont font;
    
    private bool orbit;

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
        
        _graphics.SynchronizeWithVerticalRetrace = false;
        TargetElapsedTime = TimeSpan.FromMilliseconds(0.1);
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
        
        basicEffect = new BasicEffect(_graphics.GraphicsDevice);
        basicEffect.Alpha = 1;
        basicEffect.TextureEnabled = true;
        basicEffect.VertexColorEnabled = true;
        basicEffect.LightingEnabled = true;
        basicEffect.FogEnabled = true;
        basicEffect.FogColor = Color32.FromHex("#c0d8ff").ToVector3();
        basicEffect.FogStart = 64.0f; // At 16 render distance, fog starts 4 chunks away (16x4 = 64)
        basicEffect.FogEnd = 256.0f; // At 16 render distance, fog ends 16 chunks away (16x16 = 256)
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
        Level.Player.camera?.MarkDirty();
    }

    private float averageFPS;
    private float sinceLastFlush = 0;
    
    
    private static Identifier cobblestoneId = Identifier.WithDefaultNamespace("cobblestone");
    private static Identifier stoneId = Identifier.WithDefaultNamespace("stone");
    private static Identifier dirtId  = Identifier.WithDefaultNamespace("dirt");
    private static Identifier grassSideId  = Identifier.WithDefaultNamespace("grass_block_side");
    private static Identifier grassTopId  = Identifier.WithDefaultNamespace("grass_block_top");
    private static Identifier grassId  = Identifier.WithDefaultNamespace("grass");
    private static Identifier bedrockId  = Identifier.WithDefaultNamespace("bedrock");
    private static Identifier coalOreId  = Identifier.WithDefaultNamespace("coal_ore");
    private static Identifier ironOreId  = Identifier.WithDefaultNamespace("iron_ore");
    private static Identifier goldOreId  = Identifier.WithDefaultNamespace("gold_ore");
    private static Identifier diamondOreId  = Identifier.WithDefaultNamespace("diamond_ore");
    public Block Cobblestone;
    public Block Stone;
    public Block Dirt;
    public Block GrassBlock;
    public Block Bedrock;
    public Block CoalOre;
    public Block IronOre;
    public Block GoldOre;
    public Block DiamondOre;
    
    public List<GenPass> GenPasses = new List<GenPass>();
    
    protected override void LoadContent()
    {
        SpriteBatch = new SpriteBatch(GraphicsDevice);
        font = Content.Load<SpriteFont>("fonts/font");
        
        Atlas = new TextureAtlas(Identifier.WithDefaultNamespace("atlas"), 16, 4, 4); // Start with a max of 16 textures
        Atlas.AddTexture(Content.Load<Texture2D>("textures/cobblestone"), cobblestoneId);
        Atlas.AddTexture(Content.Load<Texture2D>("textures/stone"), stoneId);
        Atlas.AddTexture(Content.Load<Texture2D>("textures/dirt"), dirtId);
        Atlas.AddTexture(Content.Load<Texture2D>("textures/grass_block_side"), grassSideId);
        Atlas.AddTexture(Content.Load<Texture2D>("textures/grass_block_top"), grassTopId);
        Atlas.AddTexture(Content.Load<Texture2D>("textures/bedrock"), bedrockId);
        Atlas.AddTexture(Content.Load<Texture2D>("textures/coal_ore"), coalOreId);
        //Atlas.AddTexture(Content.Load<Texture2D>("textures/iron_ore"), ironOreId);
        //Atlas.AddTexture(Content.Load<Texture2D>("textures/gold_ore"), coalOreId);
        //Atlas.AddTexture(Content.Load<Texture2D>("textures/diamond_ore"), ironOreId);
        
        Dirt = new Block(Model.CubeAll(dirtId, Color.White, dirtId));
        Cobblestone = new Block(Model.CubeAll(cobblestoneId, Color.White, cobblestoneId));
        GrassBlock = new Block(Model.CubeBottomTopSides(Vector3.Zero, Vector3.One, grassTopId,dirtId, grassSideId, Color.White, grassId));
        Stone = new Block(Model.CubeAll(stoneId, Color.White, stoneId));
        Bedrock = new Block(Model.CubeAll(bedrockId, Color.White, bedrockId));
        CoalOre = new Block(Model.CubeAll(coalOreId, Color.White, coalOreId));
        IronOre = new Block(Model.CubeAll(coalOreId, Color.White, coalOreId));
        
        GenPasses.Add(new WormCavePass());
        GenPasses.Add(new BlobCavePass());
        GenPasses.Add(new CoalGenPass());
        GenPasses.Add(new IronGenPass());
        
        Level = new();
    }
    
    private static readonly SamplerState UISamplerState = SamplerState.PointClamp;
    
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

    public readonly Color SkyColor = Color32.FromHex("#6EB1FF");
    
    protected override void Update(GameTime gameTime)
    {
        if (IsActive)
        {
            newKeyboardState = Keyboard.GetState();

            if (newKeyboardState.IsKeyDown(Keys.Escape) && oldKeyboardState.IsKeyUp(Keys.Escape))
            {
                mouseLocked = !mouseLocked;
            }
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (mouseLocked)
            {
                IsMouseVisible = false;
                MouseState currentMouseState = Mouse.GetState();
                
                float mouseDeltaX = currentMouseState.X - oldMouseState.X;
                float mouseDeltaY = currentMouseState.Y - oldMouseState.Y;

                Level.Player.camera.Yaw -= mouseDeltaX * Level.Player.camera.CameraSensitivity;
                Level.Player.camera.Pitch -= mouseDeltaY * Level.Player.camera.CameraSensitivity;

                if (currentMouseState.X < 10 || currentMouseState.Y < 10 ||
                    currentMouseState.X > GraphicsDevice.Viewport.Width - 10||
                    currentMouseState.Y > GraphicsDevice.Viewport.Height - 10)
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
            
            Level.Player.Update(gameTime);
            
            if (KeyDown(Keys.OemPlus))
            {
                showAtlas = !showAtlas;
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
    
    public bool KeyDown(Keys key)
    {
        return newKeyboardState.IsKeyDown(key) && oldKeyboardState.IsKeyUp(key);
    }
    public bool KeyUp(Keys key)
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
        
        basicEffect.Projection = Level.Player.camera.GetProjectionMatrix();
        basicEffect.View = Level.Player.camera.GetViewMatrix();
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
        
        SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, UISamplerState);
        // FPS Counter
        SpriteBatch.DrawString(font, "FPS: " + frameCounter.AverageFramesPerSecond.ToString("F1"), new Vector2(2, 2), Color.White);
        SpriteBatch.DrawString(font, "Position: " + Level.Player.camera.Position.ToString("F1"), new Vector2(2, 17), Color.White);
        SpriteBatch.DrawString(font, $"yaw: {Level.Player.camera.Yaw:F0}, pitch: {Level.Player.camera.Pitch:F0}", new Vector2(2, 32), Color.White);
        SpriteBatch.DrawString(font, $"velocity: {Level.Player.Velocity.ToString("F1")}", new Vector2(2, 47), Color.White);
        // Atlas
        if (showAtlas)
        {
            Logger.Log(new Point(Atlas.AtlasWidth, Atlas.AtlasHeight));
            SpriteBatch.Draw(Atlas._renderTarget, new Rectangle(0, 0, Atlas.AtlasWidth, Atlas.AtlasWidth), Color.White);
        }
        
        SpriteBatch.End();
        
        
        ModLoader.Instance.DrawAllModsUi(gameTime);
            
        base.Draw(gameTime);
    }
}