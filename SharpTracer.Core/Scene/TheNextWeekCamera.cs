using System.Numerics;
using SharpTracer.Core.SceneCamera;

namespace SharpTracer.Core.Scene;

public class TheNextWeekCamera : IEyeView
{
    public Camera GetCamera()
    {
        Vector3 lookFrom = new(478f, 278f, -600f);
        Vector3 lookAt = new(278f, 278f, 0f);
        float fov = 40f;
        float distToFocus = Vector3.Distance(lookFrom, lookAt);
        float aperture = 0.001f;
        Camera camera = new(800, 800, lookFrom, lookAt, fov, aperture, distToFocus, 0f, 1f);

        return camera;
    }
}
