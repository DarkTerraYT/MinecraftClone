using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MinecraftClone.Core.World;

public class Mesh<T> : IDisposable where T : struct
{
    private int IndexCount = 0;
    private int VertexCount = 0;
    
    private VertexBuffer vertexBuffer;
    private IndexBuffer indexBuffer;

    private GraphicsDevice GraphicsDevice;

    public bool Empty =>
        VertexCount == 0 || IndexCount == 0 || vertexBuffer == null || indexBuffer == null;


    public void Draw(Effect effect)
    {
        if (Empty)
        {
            return;
        }
        try
        {
            GraphicsDevice.SetVertexBuffer(vertexBuffer);
            GraphicsDevice.Indices = indexBuffer;
        
            foreach (var pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                //GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleList, 0, vertexBuffer.VertexCount / 3);
                GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, indexBuffer.IndexCount / 3);
            }
        }
        catch (Exception e)
        {
            Minecraft.Instance.Logger.Error("Failed to draw mesh! Stacktrace: " + e);
        }
    }

    public void Update(T[] vertices, ushort[] indices)
    {
        VertexCount = vertices.Length;
        IndexCount = indices.Length;
        if (VertexCount == 0 || IndexCount == 0) return;
        
        vertexBuffer ??= new VertexBuffer(GraphicsDevice, typeof(T), vertices.Length, BufferUsage.WriteOnly);
        vertexBuffer.SetData(vertices);
        indexBuffer = new IndexBuffer(GraphicsDevice,
            typeof(ushort),
            indices.Length,
            BufferUsage.WriteOnly);
        indexBuffer.SetData(indices);
    }

    private static ushort[] StripVertexes(T[] vertices, out T[] newVertices)
    {
        var indices = new ushort[vertices.Length];
        List<T> vertexList = new List<T>();
        Dictionary<T, ushort> vertexToIndex = new Dictionary<T, ushort>();
        for (int i = 0; i < vertices.Length; i++)
        {
            T vertex = vertices[i];
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
        newVertices = vertexList.ToArray();
        return indices;
    }
    
    public Mesh(T[] vertices, ushort[] indices, bool indexed = true, bool strip = false)
    {
        GraphicsDevice = Minecraft.Instance.GraphicsDevice;

        if (strip && indexed)
        {
            indices = StripVertexes(vertices, out vertices);
        }
        else if (indexed && indices.Length == 0)
        {
            Minecraft.Instance.Logger.Warn("Index mesh with empty indices list!");
        }
        else if (!indexed)
        {
            for (ushort i = 0; i < vertices.Length; i++)
            {
                indices[i] = i;
            }
        }
        
        Update(vertices, indices);
    }

    public void Dispose()
    {
        vertexBuffer?.Dispose();
        indexBuffer?.Dispose();
    }
}