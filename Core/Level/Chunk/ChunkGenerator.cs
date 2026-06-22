using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MinecraftClone.Core.World;
using MinecraftClone.Core.Numerics;

namespace MinecraftClone.Core.Level.Chunk;

public static class ChunkGenerator
{
    private static Logger Logger = new Logger("ChunkGenerator");
    
    public static Mesh<VertexPositionColorNormalTexture> GenerateChunkMesh(Chunk chunk)
    {
        List<VertexPositionColorNormalTexture> allVertices = new List<VertexPositionColorNormalTexture>();
        
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
                        
                        Vector3 worldPos = new Vector3(worldX, worldY, worldZ);

                        bool cullFront = chunk.TryGetBlockWorld(new Vector3Int(x, worldY, z - 1), out var front) && !front.Block.NonCulledFaces.Contains(Face.Direction.Front);
                        bool cullBack = chunk.TryGetBlockWorld(new Vector3Int(x, worldY, z + 1), out var back) && !back.Block.NonCulledFaces.Contains(Face.Direction.Back);
                        bool cullRight = chunk.TryGetBlockWorld(new Vector3Int(x - 1, worldY, z), out var right) && !right.Block.NonCulledFaces.Contains(Face.Direction.Right);
                        bool cullLeft = chunk.TryGetBlockWorld(new Vector3Int(x + 1, worldY, z), out var left) && !left.Block.NonCulledFaces.Contains(Face.Direction.Left);
                        bool cullTop = chunk.TryGetBlockWorld(new Vector3Int(x, worldY + 1, z), out var top) && !top.Block.NonCulledFaces.Contains(Face.Direction.Top);
                        bool cullBottom = chunk.TryGetBlockWorld(new Vector3Int(x, worldY - 1, z), out var bottom) && !bottom.Block.NonCulledFaces.Contains(Face.Direction.Bottom);
                        
                        List<VertexPositionColorNormalTexture> faces = new List<VertexPositionColorNormalTexture>();

                        foreach (var face in block.Block.Model.Faces)
                        {
                            switch (face.FaceDirection)
                            {
                                case Face.Direction.Front:
                                    if (!cullFront) faces.AddRange(face.WithOffset(worldPos).Vertices);
                                    continue;
                                case Face.Direction.Back:
                                    if (!cullBack) faces.AddRange(face.WithOffset(worldPos).Vertices);
                                    continue;
                                case Face.Direction.Right:
                                    if (!cullRight) faces.AddRange(face.WithOffset(worldPos).Vertices);
                                    continue;
                                case Face.Direction.Left:
                                    if (!cullLeft) faces.AddRange(face.WithOffset(worldPos).Vertices);
                                    continue;
                                case Face.Direction.Top:
                                    if (!cullTop) faces.AddRange(face.WithOffset(worldPos).Vertices);
                                    continue;
                                case Face.Direction.Bottom:
                                    if (!cullBottom) faces.AddRange(face.WithOffset(worldPos).Vertices);
                                    continue;
                            }
                        }
                        
                        allVertices.AddRange(faces);
                    }
                }
            }
        }
        
        Mesh<VertexPositionColorNormalTexture> mesh = new Mesh<VertexPositionColorNormalTexture>(allVertices.ToArray(), [], strip: true);
        return mesh;
    }
}