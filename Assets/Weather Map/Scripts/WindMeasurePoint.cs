using NaughtyAttributes;
using UnityEngine;

[SelectionBase]
public class WindMeasurePoint : WeatherMeasurePoint<WindMeasurePoint>
{
    [SerializeField]
    private WindPressureSettings windSettings;

    [SerializeField, ReadOnly]
    private Vector2 windVelocity;
    public Vector2 WindVelocity => windVelocity;

    [SerializeField]
    private bool autoRefresh;

    public override WindMeasurePoint Init(WeatherMapSettings settings)
    {
        windSettings = settings.windPressure;
        return base.Init(settings);
    }

    public void RefreshWindVelocity()
    {
        float x = transform.position.x;
        float y = transform.position.z;
        windVelocity = windSettings.GetVelocity(x, y);
    }
}
