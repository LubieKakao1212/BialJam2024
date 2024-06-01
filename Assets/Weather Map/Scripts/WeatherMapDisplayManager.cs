using UnityEngine;

public abstract class WeatherDisplayRefresher : MonoBehaviour
{
    protected virtual void Start()
    { }

    public abstract void Refresh();
}

public class WeatherMapDisplayManager : MonoBehaviour
{
    [SerializeField]
    private WeatherDisplayRefresher[] mapDisplayRefreshers;

    private void Update()
    {
        foreach (var refresher in mapDisplayRefreshers)
            if (refresher && refresher.isActiveAndEnabled)
                refresher.Refresh();
    }
}
