using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MinecraftClone.Core.Extension;

namespace MinecraftClone.Core.World;

public class Face(VertexPositionColorNormalTexture[] vertices, Face.Direction direction)
{
    public enum Direction
    {
        None,
        Front,
        Back,
        Left,
        Right,
        Top,
        Bottom
        
    }
    public VertexPositionColorNormalTexture[] Vertices = vertices;

    public Direction FaceDirection = direction;
    
    public static Face Generate(Vector3 bottomLeft, Vector3 bottomRight, Vector3 topLeft, Vector3 topRight, Identifier textureName, Color tint, Direction direction)
    {
        TextureAtlas.TextureNode texture = Minecraft.Instance.Atlas.GetTexture(textureName);
        Vector2 uvTl = texture.UvMin;
        Vector2 uvTr = new Vector2(texture.UvMax.X, texture.UvMin.Y);
        Vector2 uvBl = new Vector2(texture.UvMin.X, texture.UvMax.Y);
        Vector2 uvBr = texture.UvMax;
        VertexPositionColorNormalTexture[] vertices = new VertexPositionColorNormalTexture[6];

        Vector3 normal = direction.GetNormalDirection();
        
        vertices[0].Position = bottomLeft;
        vertices[0].Color = tint;
        vertices[0].TextureCoordinate = uvBl;
        vertices[0].Normal = normal;
        
        vertices[1].Position = bottomRight;
        vertices[1].Color = tint;
        vertices[1].TextureCoordinate = uvBr;
        vertices[1].Normal = normal;
        
        vertices[2].Position = topLeft;
        vertices[2].Color = tint;
        vertices[2].TextureCoordinate = uvTl;
        vertices[2].Normal = normal;
        
        vertices[3].Position = bottomRight;
        vertices[3].Color = tint;
        vertices[3].TextureCoordinate = uvBr;
        vertices[3].Normal = normal;

        vertices[4].Position = topRight;
        vertices[4].Color = tint;
        vertices[4].TextureCoordinate = uvTr;
        vertices[4].Normal = normal;

        vertices[5].Position = topLeft;
        vertices[5].Color = tint;
        vertices[5].TextureCoordinate = uvTl;
        vertices[5].Normal = normal;
        
        return new Face(vertices, direction);
    }

    public Face WithOffset(Vector3 offset)
    {
        VertexPositionColorNormalTexture[] vertices = new VertexPositionColorNormalTexture[Vertices.Length];
        for (int i = 0; i < vertices.Length; i++)
        {
            vertices[i] = Vertices[i];
            vertices[i].Position += offset;
        }
        return new Face(vertices, FaceDirection);
    }
    
    public static implicit operator VertexPositionColorNormalTexture[](Face face) => face.Vertices;
}