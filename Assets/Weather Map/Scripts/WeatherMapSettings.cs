using UnityEngine;

[CreateAssetMenu]
public class WeatherMapSettings : ScriptableObject
{
    [SerializeField]
    private float unitsPerCell = 2f;

    [SerializeField]
    public Vector2Int pressureMapResolution;

    [SerializeField]
    public WindPressureSettings windPressure;
}
