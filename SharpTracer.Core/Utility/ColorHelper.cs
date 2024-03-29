﻿using System.Drawing;
using System.Numerics;

namespace SharpTracer.Core.Utility;

public static class ColorHelper
{
    public static Vector3 ToVector3(this Color color) => new(color.R / 255f, color.G / 255f, color.B / 255f);

    public static Color ToColor(this Vector3 vec) =>
        Color.FromArgb((byte)(vec.X * 255f), (byte)(vec.Y * 255f), (byte)(vec.Z * 255f));

    public static Vector3 WriteColor(Vector3 vecColor, float gamma, float samplesPerPixel)
    {
        var r = vecColor.X;
        var g = vecColor.Y;
        var b = vecColor.Z;

        var scale = 1f / samplesPerPixel;
        r = Math.Clamp(MathF.Pow(scale * r, 1f / gamma), 0f, 1f);
        g = Math.Clamp(MathF.Pow(scale * g, 1f / gamma), 0f, 1f);
        b = Math.Clamp(MathF.Pow(scale * b, 1f / gamma), 0f, 1f);

        return new Vector3(r, g, b);
    }

    public static Color FromRandom() =>
        FromRGBAF(Random.Shared.NextSingle(), Random.Shared.NextSingle(), Random.Shared.NextSingle());

    public static Color FromRGBAF(float r, float g, float b, float a = 1.0f) =>
        Color.FromArgb((byte)(a * 255f), (byte)(r * 255f), (byte)(g * 255f), (byte)(b * 255f));
}
