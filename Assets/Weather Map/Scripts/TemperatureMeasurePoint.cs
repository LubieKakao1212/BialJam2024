using UnityEngine;

public enum WeatherType
{
    SunnyClouds,
    SunnyRain,
    SunnySnow,
    Sunny,
    Cloudy,
    Rainy,
    Snowy,
    //Storm,
}

public class TemperatureMeasurePoint : WeatherMeasurePoint
{
    public static readonly int weathersCount = System.Enum.GetValues(typeof(WeatherType)).Length;

    [SerializeField]
    private WeatherType currentWeather;
    public WeatherType CurrentWeather => currentWeather;

    public float Temperature => default;
    
    private void Awake()
    {
        RandomizeWeather();
    }

    [ContextMenu("Randomize Weather")]
    private void RandomizeWeather()
    {
        currentWeather = (WeatherType)Random.Range(0, weathersCount);
    }
}
