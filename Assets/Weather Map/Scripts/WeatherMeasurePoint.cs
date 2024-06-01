using UnityEngine;

public abstract class WeatherMeasurePoint<Self> : MonoBehaviour where Self : WeatherMeasurePoint<Self>
{
    protected WeatherMapSettings settings;

    public virtual Self Init(WeatherMapSettings settings)
    {
        this.settings = settings;
        return this as Self;
    }   
}
