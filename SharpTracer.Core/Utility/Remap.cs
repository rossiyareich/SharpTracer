﻿namespace SharpTracer.Core.Utility;

public static class Remap
{
    public static long Map(long x, long inMin, long inMax, long outMin, long outMax) =>
        (x - inMin) * (outMax - outMin) / (inMax - inMin) + outMin;

    public static int Map(int x, int inMin, int inMax, int outMin, int outMax) =>
        (x - inMin) * (outMax - outMin) / (inMax - inMin) + outMin;

    public static double Map(double x, double inMin, double inMax, double outMin, double outMax) =>
        (x - inMin) * (outMax - outMin) / (inMax - inMin) + outMin;

    public static float Map(float x, float inMin, float inMax, float outMin, float outMax) =>
        (x - inMin) * (outMax - outMin) / (inMax - inMin) + outMin;
}
