using UnityEngine;
using UnityEngine.Profiling;

public class IsobarsDisplayRefresher : WeatherDisplayRefresher
{
    [SerializeField]
    private WeatherMapSettings settings;

    [SerializeField]
    private Isobars isobars;

    private void Awake()
    {
        isobars.Init(settings);
    }

    public override void Refresh()
    {
        RefreshIsobars();
    }

    private void RefreshIsobars()
    {
        var resolution = settings.pressureMapResolution;
        var pressureSdf = new float[resolution.x, resolution.y];
        var scale = 60f * Vector2.one / resolution.y;
        Profiler.BeginSample("Pressure Nosie");
        var position = isobars.transform.position;
        for (int x = 0; x < settings.pressureMapResolution.x; x++)
            for (int y = 0; y < settings.pressureMapResolution.y; y++)
            {
                pressureSdf[x, y] = settings.windPressure.GetPressure(
                    (x - resolution.x / 2f) * 2f * scale.x + position.x,
                    (y - resolution.y / 2f) * 2f * scale.y + position.z);
            }
        Profiler.EndSample();

        Profiler.BeginSample("Iso");
        isobars.FindContours(pressureSdf);
        Profiler.EndSample();
    }
}
