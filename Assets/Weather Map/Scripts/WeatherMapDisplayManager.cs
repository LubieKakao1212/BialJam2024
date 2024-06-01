using NaughtyAttributes;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;

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

    [Header("Point Spawning")]
    [SerializeField]
    private Rect areaSize;

    [SerializeField]
    private WindMeasurePoint windPointPrefab;
    [SerializeField]
    private Transform windPointRoot;
    [SerializeField]
    private float windPointDensity = 0.2f;

    [SerializeField]
    private Isobars isobars;

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
        windPoints.AddRange(SpawnWindPoints());
        isobars.Init(settings);
    }

    private void Update()
    {
        RefreshMap();
        refreshTimer += Time.deltaTime;
        if (refreshTimer > refreshTime)
        {
            refreshTimer = 0;
            //RefreshMap();
        }
    }

    private void RefreshMap()
    {
        var resolution = settings.pressureMapResolution;
        var pressureSdf = new float[resolution.x, resolution.y];
        var scale = 60f * Vector2.one / resolution.y;
        Profiler.BeginSample("Pressure Nosie");
        var position = isobars.transform.position;
        for (int x = 0; x < settings.pressureMapResolution.x; x++)
            for (int y = 0; y < settings.pressureMapResolution.y; y++)
            {
                pressureSdf[x, y] = settings.windPressure.GetPressure(
                    (x - resolution.x / 2f) * 2f * scale.x + position.x, 
                    (y - resolution.y / 2f) * 2f * scale.y + position.z);
            }
        Profiler.EndSample();

        Profiler.BeginSample("Iso");
        isobars.FindContours(pressureSdf);
        Profiler.EndSample();
    }

    private IEnumerable<WindMeasurePoint> SpawnWindPoints()
    {
        var gridCells = areaSize.size * windPointDensity;

        for (int x = 0; x < gridCells.x; x++)
            for (int y = 0; y < gridCells.y; y++)
            {
                var offset = Random.insideUnitCircle * 0.5f;
                var pos = new Vector2(x + offset.x, y + offset.y) / windPointDensity + areaSize.position;

                yield return Instantiate(windPointPrefab, new Vector3(pos.x, 0, pos.y), Quaternion.identity, windPointRoot).Init(settings);
            }
    }
}
