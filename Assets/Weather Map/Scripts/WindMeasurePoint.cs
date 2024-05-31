using NaughtyAttributes;
using UnityEngine;

[SelectionBase]
public class WindMeasurePoint : WeatherMeasurePoint<WindMeasurePoint>
{
    [SerializeField]
    private WindPressureSettings windSettings;

    [SerializeField]
    private bool updating;

    [SerializeField, ReadOnly]
    private Vector2 windVelocity;
    public Vector2 WindVelocity => windVelocity;

    private void Start()
    {
        //RefreshWindVelocity();
    }

    public override WindMeasurePoint Init(WeatherMapSettings settings)
    {
        windSettings = settings.windPressure;
        return base.Init(settings);
    }

    private void RefreshWindVelocity()
    {
        float x = transform.position.x;
        float y = transform.position.z;
        windVelocity = windSettings.GetVelocity(x, y);
    }

    private void Update()
    {
        if (updating)
            RefreshWindVelocity();
    }
}
