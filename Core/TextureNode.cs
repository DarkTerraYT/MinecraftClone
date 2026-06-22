using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MinecraftClone.Core.Numerics;

namespace MinecraftClone.Core;

public class TextureNode
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
        float x = atlasSize.X / (float)PixelCoords.X;
        float y = atlasSize.Y / (float)PixelCoords.Y;
        float width =  atlasSize.X /  (float)Size.X;
        float height =  atlasSize.Y / (float)Size.Y;
        UvMin = new Vector2(x, y);
        UvMax = new Vector2(x + width, y + height);
    }
}