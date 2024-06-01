using NaughtyAttributes;
using System.Collections.Generic;
using UnityEngine;

public class WindDisplayRefresher : WeatherDisplayRefresher
{
    [SerializeField]
    private WeatherMapSettings settings;
    [SerializeField]
    private Rect areaSize;

    [SerializeField]
    private WindMeasurePoint windPointPrefab;
    [SerializeField]
    private Transform windPointRoot;

    [SerializeField]
    private float windPointDensity = 0.1f;

    [SerializeField, ReadOnly]
    private List<WindMeasurePoint> windPoints = new List<WindMeasurePoint>();

    private void Awake()
    {
        windPoints.AddRange(SpawnWindPoints());
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

    public override void Refresh()
    {
        // refreshowanie kierunku każdej strzałki w obszarze widzenia
    }
}
