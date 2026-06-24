using System.Collections.Generic;
using MinecraftClone.Core.World;

namespace MinecraftClone.Core;

public class Block(Model model)
{
    public Model Model = model;
    public CullDirection CulledFaces = CullDirection.All;
}