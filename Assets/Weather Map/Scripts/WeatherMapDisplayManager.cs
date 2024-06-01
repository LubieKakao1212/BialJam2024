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

    //[SerializeField, ReadOnly]
   // private List<TemperatureMeasurePoint> temperaturePoints = new List<TemperatureMeasurePoint>();

    [SerializeField]
    private float refreshTime = 1;
    private float refreshTimer;

    private void Awake()
    {
        ///var allMeasurePoints = FindObjectsByType<WeatherMeasurePoint>(FindObjectsSortMode.None);
        /*foreach (var point in allMeasurePoints)
        {
            point.Init(settings);
            if (point is TemperatureMeasurePoint temperaturePoint)
            {
                temperaturePoints.Add(temperaturePoint);
            }
            else if (point is WindMeasurePoint windPoint)
            {
                windPoints.Add(windPoint);
            }
        }*/
    }

    private void Update()
    {
        refreshTimer += Time.deltaTime;
        if (refreshTimer > refreshTime)
        {
            refreshTimer = 0;
            RefreshMap();
        }
    }

    private void RefreshMap()
    {
        foreach (var refresher in mapDisplayRefreshers)
            if (refresher && refresher.enabled)
                refresher.Refresh();
    }
}
