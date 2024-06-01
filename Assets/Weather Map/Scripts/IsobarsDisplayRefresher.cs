using UnityEngine;

public class IsobarsDisplayRefresher : WeatherDisplayRefresher
{
    [SerializeField]
    private WeatherMapSettings settings;

    [SerializeField]
    private Rect areaSize;

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
        var pressureSdf = new float[settings.pressureMapResolution.x, settings.pressureMapResolution.y];
        var scale = areaSize.size / settings.pressureMapResolution;
        for (int x = 0; x < settings.pressureMapResolution.x; x++)
            for (int y = 0; y < settings.pressureMapResolution.y; y++)
            {
                pressureSdf[x, y] = settings.windPressure.GetPressure(x * scale.x + areaSize.x, y * scale.y + areaSize.y);
            }

        isobars.FindContours(pressureSdf);
    }
}
