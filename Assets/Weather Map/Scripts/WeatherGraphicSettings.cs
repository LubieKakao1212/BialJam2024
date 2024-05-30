using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class WeatherGraphicSettings : ScriptableObject
{
    [System.Serializable]
    private class WeatherToSpriteMapping
    {
        public WeatherType weather;
        public Sprite sprite;
    }

    [SerializeField]
    private WeatherToSpriteMapping[] weathersToSprites;
    private Dictionary<WeatherType, Sprite> spritesByWeather;
    public Dictionary<WeatherType, Sprite> SpritesByWeather
    {
        get
        {
            if (spritesByWeather == null)
            {
                spritesByWeather = new Dictionary<WeatherType, Sprite>();
                foreach (var mapping in weathersToSprites)
                    spritesByWeather[mapping.weather] = mapping.sprite;
            }
            return spritesByWeather;
        }
    }

    public Sprite GetGraphic(WeatherType weather) => SpritesByWeather.TryGetValue(weather, out Sprite sprite) ? sprite : null;
}
