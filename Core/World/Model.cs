using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;

namespace MinecraftClone.Core.World;

public class Model
{
    private Model(Identifier id)
    {
        Id = id;
    }

    public Model(Identifier id, params Face[] faces)
    {
        Id = id;
        Faces = faces.ToList();
    }
    
    public List<Face> Faces = new List<Face>();

    public Identifier Id;

    public static Model CubeAll(Identifier textureId, Color tint, Identifier id)
    {
        Vector3 min = Vector3.Zero;
        Vector3 max = Vector3.One;

        Model model = new Model(id);
        // Front
        model.Faces.Add(Face.Generate(
            new Vector3(min.X, min.Y, min.Z), // BL
            new Vector3(max.X, min.Y, min.Z), // BR
            new Vector3(min.X, max.Y, min.Z), // TL
            new Vector3(max.X, max.Y, min.Z), // TR
            textureId, tint, Face.Direction.Front
        ));
    
        // Back
        model.Faces.Add(Face.Generate(
            new Vector3(max.X, min.Y, max.Z), // BL
            new Vector3(min.X, min.Y, max.Z), // BR
            new Vector3(max.X, max.Y, max.Z), // TL
            new Vector3(min.X, max.Y, max.Z), // TR
            textureId, tint, Face.Direction.Back
        ));
    
        // Top
        model.Faces.Add(Face.Generate(
            new Vector3(min.X, max.Y, min.Z), // BL
            new Vector3(max.X, max.Y, min.Z), // BR
            new Vector3(min.X, max.Y, max.Z), // TL
            new Vector3(max.X, max.Y, max.Z), // TR
            textureId, tint, Face.Direction.Top
        ));
    
        // Bottom
        model.Faces.Add(Face.Generate(
            new Vector3(min.X, min.Y, max.Z), // BL
            new Vector3(max.X, min.Y, max.Z), // BR
            new Vector3(min.X, min.Y, min.Z), // TL
            new Vector3(max.X, min.Y, min.Z), // TR
            textureId, tint, Face.Direction.Bottom
        ));
        
        // Left
        model.Faces.Add(Face.Generate(
            new Vector3(max.X, min.Y, min.Z), // BL
            new Vector3(max.X, min.Y, max.Z), // BR
            new Vector3(max.X, max.Y, min.Z), // TL
            new Vector3(max.X, max.Y, max.Z), // TR
            textureId, tint, Face.Direction.Left
        ));
    
        // Right
        model.Faces.Add(Face.Generate(
            new Vector3(min.X, min.Y, max.Z), // BL
            new Vector3(min.X, min.Y, min.Z), // BR
            new Vector3(min.X, max.Y, max.Z), // TL
            new Vector3(min.X, max.Y, min.Z), // TR
            textureId, tint, Face.Direction.Right
        ));

        return model;
    }
    
    public static Model CubeBottomTopSides(Vector3 from, Vector3 to, Identifier topTexture, Identifier bottomTexture, Identifier sideTexture, Color tint, Identifier id)
    {
        Vector3 min = new Vector3(Math.Min(from.X, to.X), Math.Min(from.Y, to.Y), Math.Min(from.Z, to.Z));
        Vector3 max = new Vector3(Math.Max(from.X, to.X), Math.Max(from.Y, to.Y), Math.Max(from.Z, to.Z));


        Model model = new Model(id);
        // Front
        model.Faces.Add(Face.Generate(
            new Vector3(min.X, min.Y, min.Z), // BL
            new Vector3(max.X, min.Y, min.Z), // BR
            new Vector3(min.X, max.Y, min.Z), // TL
            new Vector3(max.X, max.Y, min.Z), // TR
            sideTexture, tint, Face.Direction.Front
        ));
    
        // Back
        model.Faces.Add(Face.Generate(
            new Vector3(max.X, min.Y, max.Z), // BL
            new Vector3(min.X, min.Y, max.Z), // BR
            new Vector3(max.X, max.Y, max.Z), // TL
            new Vector3(min.X, max.Y, max.Z), // TR
            sideTexture, tint, Face.Direction.Back
        ));
    
        // Top
        model.Faces.Add(Face.Generate(
            new Vector3(min.X, max.Y, min.Z), // BL
            new Vector3(max.X, max.Y, min.Z), // BR
            new Vector3(min.X, max.Y, max.Z), // TL
            new Vector3(max.X, max.Y, max.Z), // TR
            topTexture, tint, Face.Direction.Top
        ));
    
        // Bottom
        model.Faces.Add(Face.Generate(
            new Vector3(min.X, min.Y, max.Z), // BL
            new Vector3(max.X, min.Y, max.Z), // BR
            new Vector3(min.X, min.Y, min.Z), // TL
            new Vector3(max.X, min.Y, min.Z), // TR
            bottomTexture, tint, Face.Direction.Bottom
        ));
        
        // Left
        model.Faces.Add(Face.Generate(
            new Vector3(max.X, min.Y, min.Z), // BL
            new Vector3(max.X, min.Y, max.Z), // BR
            new Vector3(max.X, max.Y, min.Z), // TL
            new Vector3(max.X, max.Y, max.Z), // TR
            sideTexture, tint, Face.Direction.Left
        ));
    
        // Right
        model.Faces.Add(Face.Generate(
            new Vector3(min.X, min.Y, max.Z), // BL
            new Vector3(min.X, min.Y, min.Z), // BR
            new Vector3(min.X, max.Y, max.Z), // TL
            new Vector3(min.X, max.Y, min.Z), // TR
            sideTexture, tint, Face.Direction.Right
        ));

        return model;
    }
}