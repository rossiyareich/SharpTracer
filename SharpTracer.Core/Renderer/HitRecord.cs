﻿using System.Numerics;
using SharpTracer.Core.Material;

namespace SharpTracer.Core.Renderer;

public struct HitRecord
{
    public IMaterial Material;
    public Vector3 Position;
    public Vector3 Normals;
    public float T;
    public Vector2 UV;
    public bool IsFrontFace;

    public HitRecord(IMaterial material, Vector3 position, Vector3 normals, float t, Vector2 uv)
    {
        Material = material;
        Position = position;
        Normals = normals;
        T = t;
        IsFrontFace = false;
        UV = uv;
    }

    public void SetFaceNormal(Ray ray, Vector3 outwardNormal)
    {
        IsFrontFace = Vector3.Dot(ray.Direction, outwardNormal) < 0f;
        Normals = IsFrontFace ? outwardNormal : -outwardNormal;
    }
}
