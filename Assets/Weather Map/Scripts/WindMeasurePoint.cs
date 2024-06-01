using NaughtyAttributes;
using UnityEngine;

[SelectionBase]
public class WindMeasurePoint : WeatherMeasurePoint<WindMeasurePoint>
{
    public event System.Action<Vector2> OnVelocityChanged;

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
        var newVelocity = windSettings.GetVelocity(x, y);
        bool changed = windVelocity != newVelocity;
        windVelocity = newVelocity;
        if (changed)
            OnVelocityChanged?.Invoke(windVelocity);
    }
}
