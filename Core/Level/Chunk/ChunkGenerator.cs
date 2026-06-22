using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MinecraftClone.Core.Model;
using MinecraftClone.Core.Numerics;

namespace MinecraftClone.Core.Level.Chunk;

public static class ChunkGenerator
{
    private static Logger Logger = new Logger("ChunkGenerator");
    
    public static Mesh<VertexPositionColorTexture> GenerateChunkMesh(Chunk chunk)
    {
        List<VertexPositionColorTexture> allVertices = new List<VertexPositionColorTexture>();
        
        for (int x = 0; x < 16; x++)
        {
            for (int z = 0; z < 16; z++)
            {
                for (int y = 0; y < 255; y++)
                {
                    if (chunk.TryGetBlock(new Vector3Int(x, y, z), out var block))
                    {
                        int worldX = chunk.Position.X + x;
                        int worldY = chunk.Position.Y + y;
                        
                        int worldZ = chunk.Position.Z + z;
                        
                        Vector2 bottomLeft = new Vector2(0, 1);
                        Vector2 bottomRight = new Vector2(1, 1);
                        Vector2 topLeft = new Vector2(0, 0);
                        Vector2 topRight = new Vector2(1, 0);

                        VertexPositionColorTexture[] front = [
                            new (new Vector3(0 + worldX, 1 + worldY, 0 + worldZ), Color.White, topLeft),
                            new (new Vector3(0 + worldX, 0 + worldY, 0 + worldZ), Color.White, bottomLeft),
                            new (new Vector3(1 + worldX, 0 + worldY, 0 + worldZ), Color.White, bottomRight),
                            new (new Vector3(1 + worldX, 0 + worldY, 0 + worldZ), Color.White, bottomRight),
                            new (new Vector3(1 + worldX, 1 + worldY, 0 + worldZ), Color.White, topRight),
                            new (new Vector3(0 + worldX, 1 + worldY, 0 + worldZ), Color.White, topLeft),
                        ];
                        VertexPositionColorTexture[] top = [
                            new (new Vector3(0 + worldX, 1 + worldY, 1 + worldZ), Color.White, topLeft),
                            new (new Vector3(0 + worldX, 1 + worldY, 0 + worldZ), Color.White, bottomLeft),
                            new (new Vector3(1 + worldX, 1 + worldY, 0 + worldZ), Color.White, bottomRight),
                            new (new Vector3(1 + worldX, 1 + worldY, 0 + worldZ), Color.White, bottomRight),
                            new (new Vector3(1 + worldX, 1 + worldY, 1 + worldZ), Color.White, topRight),
                            new (new Vector3(0 + worldX, 1 + worldY, 1 + worldZ), Color.White, topLeft),
                        ];
                        VertexPositionColorTexture[] bottom = [
                            new (new Vector3(1 + worldX, 0 + worldY, 0 + worldZ), Color.White, topRight),
                            new (new Vector3(0 + worldX, 0 + worldY, 0 + worldZ), Color.White, topLeft),
                            new (new Vector3(0 + worldX, 0 + worldY, 1 + worldZ), Color.White, bottomLeft),
                            new (new Vector3(0 + worldX, 0 + worldY, 1 + worldZ), Color.White, bottomLeft),
                            new (new Vector3(1 + worldX, 0 + worldY, 1 + worldZ), Color.White, bottomRight),
                            new (new Vector3(1 + worldX, 0 + worldY, 0 + worldZ), Color.White, topRight),
                        ];
                        VertexPositionColorTexture[] back = [
                            new (new Vector3(0 + worldX, 1 + worldY, 1 + worldZ), Color.White, topRight),
                            new (new Vector3(1 + worldX, 1 + worldY, 1 + worldZ), Color.White, topLeft),
                            new (new Vector3(1 + worldX, 0 + worldY, 1 + worldZ), Color.White, bottomLeft),
                            new (new Vector3(1 + worldX, 0 + worldY, 1 + worldZ), Color.White, bottomLeft),
                            new (new Vector3(0 + worldX, 0 + worldY, 1 + worldZ), Color.White, bottomRight),
                            new (new Vector3(0 + worldX, 1 + worldY, 1 + worldZ), Color.White, topRight),
                        ];
                        VertexPositionColorTexture[] right = [
                            new (new Vector3(0 + worldX, 1 + worldY, 1 + worldZ), Color.White, topLeft),
                            new (new Vector3(0 + worldX, 0 + worldY, 1 + worldZ), Color.White, bottomLeft),
                            new (new Vector3(0 + worldX, 0 + worldY, 0 + worldZ), Color.White, bottomRight),
                            new (new Vector3(0 + worldX, 0 + worldY, 0 + worldZ), Color.White, bottomRight),
                            new (new Vector3(0 + worldX, 1 + worldY, 0 + worldZ), Color.White, topRight),
                            new (new Vector3(0 + worldX, 1 + worldY, 1 + worldZ), Color.White, topLeft),
                        ];
                        VertexPositionColorTexture[] left = [
                            new (new Vector3(1 + worldX, 1 + worldY, 1 + worldZ), Color.White, topRight),
                            new (new Vector3(1 + worldX, 1 + worldY, 0 + worldZ), Color.White, topLeft),
                            new (new Vector3(1 + worldX, 0 + worldY, 0 + worldZ), Color.White, bottomLeft),
                            new (new Vector3(1 + worldX, 0 + worldY, 0 + worldZ), Color.White, bottomLeft),
                            new (new Vector3(1 + worldX, 0 + worldY, 1 + worldZ), Color.White, bottomRight),
                            new (new Vector3(1 + worldX, 1 + worldY, 1 + worldZ), Color.White, topRight),
                        ];
                        
                        List<VertexPositionColorTexture> faces = new List<VertexPositionColorTexture>();
                        if (!chunk.TryGetBlock(new Vector3Int(x, worldY + 1, z), out var _))
                        {
                            faces.AddRange(top);
                        }
                        if (!chunk.TryGetBlock(new Vector3Int(x, worldY - 1, z), out var _))
                        {
                            faces.AddRange(bottom);
                        }
                        if (!chunk.TryGetBlock(new Vector3Int(x + 1, worldY, z), out var _))
                        {
                            faces.AddRange(left);
                        }
                        if (!chunk.TryGetBlock(new Vector3Int(x - 1, worldY, z), out var _))
                        {
                            faces.AddRange(right);
                        }
                        if (!chunk.TryGetBlock(new Vector3Int(x, worldY, z + 1), out var _))
                        {
                            faces.AddRange(back);
                        }
                        if (!chunk.TryGetBlock(new Vector3Int(x, worldY, z - 1), out var _))
                        {
                            faces.AddRange(front);
                        }

                        
                        allVertices.AddRange(faces);
                    }
                }
            }
        }
        
        Mesh<VertexPositionColorTexture> mesh = new Mesh<VertexPositionColorTexture>(allVertices.ToArray(), [], strip: true);
        return mesh;
    }
}