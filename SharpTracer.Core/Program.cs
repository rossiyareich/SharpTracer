using System.Diagnostics;
using System.Drawing;
using System.Numerics;
using System.Text.Json;
using SharpTracer.Core.Logging;
using SharpTracer.Core.Renderer;
using SharpTracer.Core.Scene;
using SharpTracer.Core.SceneCamera;
using SharpTracer.Core.Settings;
using SharpTracer.Core.Utility;
using SimpleImageIO;

namespace SharpTracer.Core;

internal class Program
{
    private static async Task Main(string[] args)
    {
        const string pathSettings = "settings.json";

        RenderSettings settings;
        try
        {
            string jsonText = await File.ReadAllTextAsync(pathSettings);
            settings = JsonSerializer.Deserialize<RenderSettings>(jsonText);
            if (settings is null)
            {
                throw new Exception();
            }
        }
        catch
        {
            ConsoleLogger.Get().LogError("Failed to load settings");
            throw;
        }

        string fullPath = Path.Combine(settings.FolderPath, settings.FileName);
        const int maxDepth = 20;

        IScene scene = new CornellBoxScene();
        // HittableGroup world = scene.Render();
        BvhNode world = new(scene.Render(), 0f, 1f);

        IEyeView eye = new CornellCamera();
        Camera camera = eye.GetCamera();

        // Should really be called scene ambient light and not background
        // ICameraBackground background = new SolidBackground(Color.LightBlue);
        ICameraBackground background = new SolidBackground(Color.Black);

        // Render
        RgbImage img = new(camera.Width, camera.Height);
        const float gamma = 0.8f;

        // Execute
        Stopwatch sw = Stopwatch.StartNew();
        ConsoleLogger.Get().LogInfo("Start pathtracing");
        int scanlinesLeft = camera.Height;
        Parallel.For(0, camera.Height, new ParallelOptions { MaxDegreeOfParallelism = settings.NumTasks },
            y => CalculateScanline(y, camera, background, world, maxDepth, img, gamma, ref scanlinesLeft));
        ConsoleLogger.Get().LogInfo("Done pathtracing");

        RgbImage image = img;
        if (settings.Denoise)
        {
            ConsoleLogger.Get().LogInfo("Start denoising");
            using (Denoiser denoiser = new())
            {
                image = denoiser.Denoise(img);
            }

            ConsoleLogger.Get().LogInfo("Done denoising");
        }

        ConsoleLogger.Get().LogInfo("Start writing to disk");
        image.WriteToFile(fullPath);
        ConsoleLogger.Get().LogInfo("Done writing to disk");
        sw.Stop();
        ConsoleLogger.Get().LogInfo($"Rendering done, time elapsed: {sw.Elapsed}");
        sw.Reset();

        ConsoleLogger.Get().LogInfo("Opening");
        ProcessStartInfo info = new(fullPath) { UseShellExecute = true };
        Process.Start(info);
        ConsoleLogger.Get().LogInfo("Done");
    }

    private static void CalculateScanline(
        int y,
        Camera camera,
        ICameraBackground background,
        IHittable world,
        int maxDepth,
        RgbImage renderer,
        float gamma,
        ref int scanlineCount)
    {
        for (int x = 0; x < camera.Width; x++)
        {
            Vector3 colorVec = Vector3.Zero;
            for (int s = 0; s < ColorHelper.SamplesPerPixel; s++)
            {
                float u = (x + Random.Shared.NextSingle()) / camera.Width;
                float v = (y + Random.Shared.NextSingle()) / camera.Height;
                Ray ray = camera.GetRay(u, v);
                colorVec += RayColor(ray, world, maxDepth, background);
            }

            renderer.SetPixel(x, camera.Height - 1 - y, ColorHelper.WriteColor(colorVec, gamma));
        }

        ConsoleLogger.Get().LogInfo($"Scanlines remaining: {scanlineCount--}");
    }

    private static Vector3 RayColor(Ray ray, IHittable world, int stackDepth, ICameraBackground background)
    {
        if (stackDepth <= 0)
        {
            return Vector3.Zero;
        }

        HitRecord hit = default;

        if (world.Hit(ray, 0.001f, float.PositiveInfinity, ref hit))
        {
            Vector3 emitted = hit.Material.Emitted(hit.UV, hit.Position);
            if (hit.Material.Scatter(ray, hit, out Color attenuation, out Ray scattered))
            {
                return emitted + attenuation.ToVector3() * RayColor(scattered, world, stackDepth - 1, background);
            }

            return emitted;
        }

        return background.GetScreenSpaceColor(ray);
    }
}
