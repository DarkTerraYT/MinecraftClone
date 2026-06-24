using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MinecraftClone.Core.World;

public class Mesh<T> : IDisposable where T : struct
{
    public T[] Vertices;
    public ushort[] Indices;

    private VertexBuffer vertexBuffer;
    private IndexBuffer indexBuffer;

    private GraphicsDevice GraphicsDevice;

    public bool Empty => Vertices.Length == 0 || Indices.Length == 0;


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

    public void Update()
    {
        if (Empty) return;
        
        vertexBuffer = new VertexBuffer(GraphicsDevice, typeof(T), Vertices.Length, BufferUsage.WriteOnly);
        vertexBuffer.SetData(Vertices);
        indexBuffer = new IndexBuffer(GraphicsDevice,
            typeof(ushort),
            Indices.Length,
            BufferUsage.WriteOnly);
        indexBuffer.SetData(Indices);
    }

    public void StripVertexes()
    {
        
        Indices = new ushort[Vertices.Length];
        List<T> vertexList = new List<T>();
        Dictionary<T, ushort> vertexToIndex = new Dictionary<T, ushort>();
        for (int i = 0; i < Vertices.Length; i++)
        {
            T vertex = Vertices[i];
            if (vertexToIndex.TryGetValue(vertex, out ushort index))
            {
                Indices[i] = index;
            }
            else
            {
                ushort newIndex = (ushort)vertexList.Count;
                vertexToIndex.Add(vertex, newIndex);
                vertexList.Add(vertex);
                Indices[i] = newIndex;
            }
        }
        
        Vertices = vertexList.ToArray();
    }
    
    public Mesh(T[] vertices, ushort[] indices, bool indexed = true, bool strip = false)
    {
        Vertices = vertices;
        Indices = indices;

        GraphicsDevice = Minecraft.Instance.GraphicsDevice;

        if (strip && indexed)
        {
            StripVertexes();
        }
        else if (indexed && Indices.Length == 0)
        {
            Minecraft.Instance.Logger.Warn("Index mesh with empty indices list!");
        }
        else if (!indexed)
        {
            for (ushort i = 0; i < Vertices.Length; i++)
            {
                Indices[i] = i;
            }
        }
        
        Update();
    }

    public void Dispose()
    {
        vertexBuffer?.Dispose();
        indexBuffer?.Dispose();
    }
}