using NaughtyAttributes;
using System.Collections.Generic;
using UnityEngine;

public class WeatherMapDisplayManager : MonoBehaviour
{
    [SerializeField]
    protected WeatherMapSettings settings;

    [SerializeField, ReadOnly]
    private List<TemperatureMeasurePoint> temperaturePoints = new List<TemperatureMeasurePoint>();

    [SerializeField, ReadOnly]
    private List<WindMeasurePoint> windPoints = new List<WindMeasurePoint>();

    [SerializeField]
    private float refreshTime = 1;
    private float refreshTimer;

    [SerializeField]
    private List<GameObject> displayElements;

    private void Awake()
    {
        var allMeasurePoints = FindObjectsByType<WeatherMeasurePoint>(FindObjectsSortMode.None);
        foreach (var point in allMeasurePoints)
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
        }
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

    }
}
