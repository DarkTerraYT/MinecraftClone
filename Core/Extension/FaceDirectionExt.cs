using Microsoft.Xna.Framework;
using MinecraftClone.Core.World;

namespace MinecraftClone.Core.Extension;

public static class FaceDirectionExt
{
    public static Vector3 GetNormalDirection(this Face.Direction direction)
    {
        return direction switch
        {
            Face.Direction.Front => Vector3.Forward,
            Face.Direction.Back => Vector3.Backward,
            Face.Direction.Left => Vector3.Right,
            Face.Direction.Right => Vector3.Left,
            Face.Direction.Top => Vector3.Up,
            Face.Direction.Bottom => Vector3.Down,
            _ => Vector3.UnitX
        };
    }
}