using NaughtyAttributes;
using UnityEngine;

[SelectionBase]
public class WindMeasurePoint : WeatherMeasurePoint
{
    [SerializeField]
    private WindSettings windSettings;

    [SerializeField]
    private float maxWindSpeed = 2;

    [SerializeField]
    private bool updating;

    [SerializeField, ReadOnly]
    private Vector2 windVelocity;
    public Vector2 WindVelocity => windVelocity;

    private void Start()
    {
        RefreshWindVelocity();
    }

    private void RefreshWindVelocity()
    {
        float x = transform.position.x;
        float y = transform.position.z;
        windVelocity = maxWindSpeed * windSettings.GetVelocity(x, y);
    }

    private void Update()
    {
        if (updating)
            RefreshWindVelocity();
    }
}
