using System;
using Microsoft.Xna.Framework;

namespace MinecraftClone.Core;

public struct Color32(byte r, byte g, byte b, byte a)
{
    public byte R = r;
    public byte G = g;
    public byte B = b;
    public byte A = a;

    public Vector3 ToVector3() => new Vector3(R / 255.0f, G / 255.0f, B / 255.0f);

    public static Color32 FromInt(int c)
    {
        Color32 color = new Color32();
        color.R = (byte)(c >> 24);
        color.G = (byte)(c >> 16);
        color.B = (byte)(c >> 8);
        color.A = (byte)(c >> 0);
        
        return color;
    }

    public static readonly Color32 White = new Color32(255, 255, 255, 255);
    
    public static Color32 FromHex(string s)
    {
        string hex = s.Replace("#", "").ToLower();

        try
        {
            if (hex.Length == 6)
            {
                Color32 color = new Color32
                {
                    R = byte.Parse(hex.Substring(0, 2), System.Globalization.NumberStyles.HexNumber),
                    G = byte.Parse(hex.Substring(2, 2), System.Globalization.NumberStyles.HexNumber),
                    B = byte.Parse(hex.Substring(4, 2), System.Globalization.NumberStyles.HexNumber),
                    A = 255
                };

                return color;
            }

            if (hex.Length == 8)
            {
                Color32 color = new Color32
                {
                    R = byte.Parse(hex.Substring(0, 2), System.Globalization.NumberStyles.HexNumber),
                    G = byte.Parse(hex.Substring(2, 2), System.Globalization.NumberStyles.HexNumber),
                    B = byte.Parse(hex.Substring(4, 2), System.Globalization.NumberStyles.HexNumber),
                    A = byte.Parse(hex.Substring(6, 2), System.Globalization.NumberStyles.HexNumber),
                };

                return color;
            }

            throw new ArgumentException($"Invalid length in hex string: {s}. Should contain 6 or 8 characters excluding the #.");
        }
        catch (Exception e)
        {
            Logger.Global.Error($"Failed to create Color from hex {s}. {e}");
            return White;
        }
    }
    
    public static implicit operator Color(Color32 c)
    {
        return new Color(c.R / 255.0f, c.G / 255.0f, c.B / 255.0f, c.A / 255.0f);
    }
}