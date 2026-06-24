using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MinecraftClone.Core.Numerics;
using MinecraftClone.Core.World;

namespace MinecraftClone.Core.Level;

public static class ChunkGenerator
{
    private static Logger Logger = new Logger("Chunk Generator");
    public static Mesh<VertexPositionColorNormalTexture> GenerateChunkMesh(Chunk chunk)
    {
        List<VertexPositionColorNormalTexture> allFaces = new List<VertexPositionColorNormalTexture>();
        var level = Minecraft.Instance.Level;

        // Cache the 6 neighboring chunks at the start of meshing to save time 🤑🤑🤑
        bool frontChunkNeighbor = level.TryGetChunk(new Vector3(chunk.Position.X, chunk.Position.Y, chunk.Position.Z - Chunk.Depth), out Chunk neighborFront);
        bool backChunkNeighbor = level.TryGetChunk(new Vector3(chunk.Position.X, chunk.Position.Y, chunk.Position.Z + Chunk.Depth), out Chunk neighborBack);
        bool rightChunkNeighbor = level.TryGetChunk(new Vector3(chunk.Position.X - Chunk.Width, chunk.Position.Y, chunk.Position.Z), out Chunk neighborRight);
        bool leftChunkNeighbor =  level.TryGetChunk(new Vector3(chunk.Position.X + Chunk.Width, chunk.Position.Y, chunk.Position.Z), out Chunk neighborLeft);
        bool topChunkNeighbor = level.TryGetChunk(new Vector3(chunk.Position.X, chunk.Position.Y + Chunk.Height, chunk.Position.Z), out Chunk neighborTop);
        bool bottomChunkNeighbor = level.TryGetChunk(new Vector3(chunk.Position.X, chunk.Position.Y - Chunk.Height, chunk.Position.Z), out Chunk neighborBottom);
        
        for (int x = 0; x < Chunk.Width; x++)
        {
            for (int y = 0; y < Chunk.Height; y++)
            {
                for (int z = 0; z < Chunk.Depth; z++)
                {
                    if (chunk.TryGetBlock(new Vector3Int(x, y, z), out BlockState block))
                    {
                        Vector3Int worldPos = new Vector3Int(x, y, z) + chunk.Position;


                        BlockState front = null;
                        BlockState back = null;
                        BlockState right = null;
                        BlockState left = null;
                        BlockState top = null;
                        BlockState bottom = null;
                        bool hasFrontNeighbor = z > 0 ? chunk.TryGetBlock(new Vector3Int(x, y, z - 1), out front)
                            : frontChunkNeighbor && neighborFront.TryGetBlock(new Vector3Int(x, y, Chunk.Depth - 1), out front);
                        bool hasBackNeighbor = z < Chunk.Depth - 1 ? chunk.TryGetBlock(new Vector3Int(x, y, z + 1), out back)
                            : backChunkNeighbor && neighborBack.TryGetBlock(new Vector3Int(x, y, 0), out back);
                        bool hasRightNeighbor = x > 0 ? chunk.TryGetBlock(new Vector3Int(x - 1, y, z), out right) 
                            : rightChunkNeighbor && neighborRight.TryGetBlock(new Vector3Int(Chunk.Width - 1, y, z), out right);
                        bool hasLeftNeighbor = x < Chunk.Width - 1 ? chunk.TryGetBlock(new Vector3Int(x + 1, y, z), out left) 
                            : leftChunkNeighbor && neighborLeft.TryGetBlock(new Vector3Int(0, y, z), out left);
                        bool hasBottomNeighbor = y > 0 ? chunk.TryGetBlock(new Vector3Int(x, y - 1, z), out bottom)
                            : bottomChunkNeighbor && neighborBottom.TryGetBlock(new Vector3Int(x, Chunk.Height - 1, z), out bottom);
                        bool hasTopNeighbor = y < Chunk.Height - 1 ? chunk.TryGetBlock(new Vector3Int(x, y + 1, z), out top) 
                            : topChunkNeighbor && neighborTop.TryGetBlock(new Vector3Int(x, 0, z), out top);

                        List<VertexPositionColorNormalTexture> faces = new List<VertexPositionColorNormalTexture>();

                        foreach (var face in block.Block.Model.Faces)
                        {
                            if (face.FaceDirection == Face.Direction.Front && hasFrontNeighbor && (front.Block.CulledFaces & CullDirection.Front) != 0) continue;
                            if (face.FaceDirection == Face.Direction.Back && hasBackNeighbor && (back.Block.CulledFaces & CullDirection.Back) != 0) continue;
                            if (face.FaceDirection == Face.Direction.Left && hasLeftNeighbor && (left.Block.CulledFaces & CullDirection.Left) != 0) continue;
                            if (face.FaceDirection == Face.Direction.Right && hasRightNeighbor && (right.Block.CulledFaces & CullDirection.Right) != 0) continue;
                            if (face.FaceDirection == Face.Direction.Top && hasTopNeighbor && (top.Block.CulledFaces & CullDirection.Top) != 0) continue;
                            if (face.FaceDirection == Face.Direction.Bottom && hasBottomNeighbor && (bottom.Block.CulledFaces & CullDirection.Bottom) != 0) continue;
                            
                            allFaces.AddRange(face.WithOffset(worldPos).Vertices);
                        }

                    }
                }
            }
        }
        
        return new Mesh<VertexPositionColorNormalTexture>(allFaces.ToArray(), [], strip: true);
    }
}