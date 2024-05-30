using UnityEngine;

public abstract class WeatherMeasurePoint : MonoBehaviour
{
    protected WeatherMapSettings settings;

    public virtual void Init(WeatherMapSettings settings)
    {
        this.settings = settings;
    }
}
