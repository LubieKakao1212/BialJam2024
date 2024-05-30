using NaughtyAttributes;
using UnityEngine;

[SelectionBase]
public class WindMeasurePoint : WeatherMeasurePoint
{
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
        windVelocity = maxWindSpeed * new Vector2(
            Mathf.PerlinNoise(x + 2.1f, y + 3.7f) * 2 - 1 ,
            Mathf.PerlinNoise(x - 6.9f, y - 0.420f) * 2 - 1);
    }

    private void Update()
    {
        if (updating)
            RefreshWindVelocity();
    }
}
