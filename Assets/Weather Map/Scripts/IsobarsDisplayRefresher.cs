using UnityEngine;
using UnityEngine.Profiling;

public class IsobarsDisplayRefresher : WeatherDisplayRefresher
{
    [SerializeField]
    private WeatherMapSettings settings;

    [SerializeField]
    private Isobars isobars;

    private float[,] pressureMap;

    private void Awake()
    {
        isobars.Init(settings); 
        var resolution = settings.pressureMapResolution;
        pressureMap = new float[resolution.x, resolution.y];
    }

    public override void Refresh()
    {
        RefreshIsobars();
    }

    private void RefreshIsobars()
    {
        var resolution = settings.pressureMapResolution;
        var scale = 60f * Vector2.one / resolution.y;
        var position = isobars.transform.position;
        for (int x = 0; x < settings.pressureMapResolution.x; x++)
            for (int y = 0; y < settings.pressureMapResolution.y; y++)
            {
                pressureMap[x, y] = settings.windPressure.GetPressure(
                    (x - resolution.x / 2f) * 2f * scale.x + position.x,
                    (y - resolution.y / 2f) * 2f * scale.y + position.z);
            }

        isobars.FindContours(pressureMap);
    }
}
