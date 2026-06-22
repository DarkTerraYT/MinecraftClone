using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RenderTargetUsage = Microsoft.Xna.Framework.Graphics.RenderTargetUsage;
using SpriteBatch = Microsoft.Xna.Framework.Graphics.SpriteBatch;

namespace MinecraftClone.Core;

public sealed class TextureAtlas : IDirtyable
{
    public sealed class TextureNode
    {
        public readonly Texture2D OriginalTexture;
    
        public Vector2 UvMin;
        public Vector2 UvMax;
    
        public Identifier Id;
        public Identifier AtlasId;
    
        public Point PixelCoords;
        public Point Size;

        public TextureNode(Texture2D texture, Point pixelCoords, Identifier id, Identifier atlas)
        {
            OriginalTexture = texture;
            PixelCoords = pixelCoords;
            Id = id;
            AtlasId = atlas;
            Size = new Point(texture.Width, texture.Height);
        }

        public void CalculateUv(Point atlasSize)
        {
            
            float x = (float)PixelCoords.X / atlasSize.X;
            float y =  (float)PixelCoords.Y / atlasSize.Y;
            float width = (float)Size.X / atlasSize.X;
            float height = (float)Size.Y / atlasSize.Y;
            UvMin = new Vector2(x, y);
            UvMax = new Vector2(x + width, y + height);
            Logger.Global.Debug(UvMin);
            Logger.Global.Debug(UvMax);
        }
    }
    public Identifier Id { get; }

    private Logger logger;
    
    private SpriteBatch _spriteBatch => Minecraft.Instance.SpriteBatch;
    
    public RenderTarget2D _renderTarget;

    public int AtlasWidth => rows * SpriteSize;
    public int AtlasHeight => columns * SpriteSize;

    private readonly int SpriteSize;
    
    private int rows;
    private int columns;

    private int row;
    private int column;

    public readonly bool Mip;
    
    public TextureAtlas(Identifier id, int spriteSize, int rows, int columns, bool mip = false)
    {
        Id = id;
        this.rows = rows;
        this.columns = columns;
        SpriteSize = spriteSize;_renderTarget = new RenderTarget2D(
            Minecraft.Instance.GraphicsDevice, 
            rows * SpriteSize, 
            columns * SpriteSize, 
            mip, // Allocates Level 0, 1, and 2. It physically cannot downscale further.
            SurfaceFormat.Color, 
            DepthFormat.None, 
            0, 
            RenderTargetUsage.PreserveContents
        );
        logger = new Logger(ToString());

        _samplerState = new SamplerState()
        {
            AddressU = TextureAddressMode.Wrap,
            AddressV = TextureAddressMode.Wrap,
            AddressW = TextureAddressMode.Wrap,
            Filter = TextureFilter.Point,
            MaxAnisotropy = 16,
            MaxMipLevel = 0
        };
        
        Mip = mip;
    }

    public override string ToString()
    {
        return "Texture Atlas: " + Id;
    }

    private Dictionary<Identifier, TextureNode> nodesById = new();

    private readonly SamplerState _samplerState;
    
    private void DrawTexture(int row, int column)
    {
        if (row * columns + column >= nodesById.Count)
            return;
        this.row = row;
        this.column = column;
        int index = row * columns + column;
        Identifier id = nodesById.Keys.ToList()[index];
        
        TextureNode texture = nodesById[id];
        
        Rectangle drawRect = new Rectangle(texture.PixelCoords.X, texture.PixelCoords.Y, SpriteSize, SpriteSize);
        
        _spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, _samplerState);
        _spriteBatch.Draw(texture.OriginalTexture, drawRect, Color.White);
        _spriteBatch.End();
        
    }

    private void RelcalcuteUvs()
    {
        Point atlasSizePoint = new Point(AtlasWidth, AtlasHeight);
        for (int column = 0; column < columns; column++)
        {
            for (int row = 0; row < rows; row++)
            {
                if (row * columns + column >= nodesById.Count)
                    return;
                
                this.row = row;
                this.column = column;
                
                int index = row * columns + column;
                Identifier id = nodesById.Keys.ToList()[index];

                int x = column * SpriteSize;
                int y = row * SpriteSize;
        
                TextureNode texture = nodesById[id];
                texture.PixelCoords = new Point(x, y);
                texture.CalculateUv(atlasSizePoint);
            }
        }
    }

    private void Resize()
    {
        if (rows * columns + column < nodesById.Count) return;
        
        Logger.Global.Debug("Resizing atlas " + Id);
        columns = (int)Math.Ceiling((Math.Sqrt(nodesById.Count)));
        rows = (int)Math.Ceiling((double)nodesById.Count / columns);
        
        Logger.Global.Debug($"New size: indexes({columns},{rows}) size:({AtlasWidth},{AtlasHeight})" );

        _renderTarget?.Dispose();
        _renderTarget = new RenderTarget2D(Minecraft.Instance.GraphicsDevice, rows * SpriteSize, columns * SpriteSize, Mip, SurfaceFormat.Color, DepthFormat.None, 0, RenderTargetUsage.PreserveContents);
        RelcalcuteUvs();
        MarkDirty();
    }
    
    public void Bake()
    {
        Minecraft.Instance.GraphicsDevice.SetRenderTarget(_renderTarget);
        Minecraft.Instance.GraphicsDevice.Clear(Color.Transparent);

        for (int i = 0; i < columns; i++)
        {
            for (int j = 0; j < rows; j++)
            {
                DrawTexture(j, i);
            }
        }
        Minecraft.Instance.GraphicsDevice.SetRenderTarget(null);
        isDirty = false;
    }
    
    public TextureNode AddTexture(Texture2D texture, Identifier name)
    {
        TextureNode textureNode = new TextureNode(texture, new Point(), name, Id);
        textureNode.Size = new Point(SpriteSize, SpriteSize);
        nodesById.Add(name, textureNode);
        if (nodesById.Count > rows * columns)
        {
            Resize();
            return textureNode;
        }
        
        Logger.Global.Debug("Adding texture " + name);
        
        textureNode.PixelCoords = new Point(column * SpriteSize, row * SpriteSize);
        textureNode.CalculateUv(new Point(AtlasWidth, AtlasHeight));

        column++;
        if (column >= columns)
        {
            column = 0;
            rows++;
        }
        
        MarkDirty();
        
        return textureNode;
    }

    private bool isDirty = false;
    public bool Dirty => isDirty;

    public void MarkDirty()
    {
        isDirty = true;
    }
    
    public TextureNode GetTexture(Identifier spriteName)
    {
        return nodesById.GetValueOrDefault(spriteName);
    }
}