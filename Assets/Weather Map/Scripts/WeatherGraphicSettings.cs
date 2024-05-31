using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class WeatherGraphicSettings : ScriptableObject
{
    [System.Serializable]
    private class WeatherToVisualMapping
    {
        public WeatherType weather;
        public GameObject visual;
    }

    [SerializeField]
    private WeatherToVisualMapping[] weathersToSprites;
    private Dictionary<WeatherType, GameObject> spritesByWeather;
    public Dictionary<WeatherType, GameObject> SpritesByWeather
    {
        get
        {
            if (spritesByWeather == null)
            {
                spritesByWeather = new Dictionary<WeatherType, GameObject>();
                foreach (var mapping in weathersToSprites)
                    spritesByWeather[mapping.weather] = mapping.visual;
            }
            return spritesByWeather;
        }
    }

    public GameObject GetGraphic(WeatherType weather) => SpritesByWeather.TryGetValue(weather, out var visual) ? visual : null;
}
