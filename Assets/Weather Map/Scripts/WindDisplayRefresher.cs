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

    [SerializeField]
    private Camera minimapCamera;

    private WindMeasurePoint[,] windPoints;

    private void Awake()
    {
        SpawnWindPoints();
    }

    private void SpawnWindPoints()
    {
        var gridCells = Vector2Int.RoundToInt(areaSize.size * windPointDensity);
        windPoints = new WindMeasurePoint[gridCells.y, gridCells.x];

        for (int y = 0; y < gridCells.y; y++)
        {
            for (int x = 0; x < gridCells.x; x++)
            {
                var offset = Random.insideUnitCircle * 0.5f;
                var pos = new Vector2(x + offset.x, y + offset.y) / windPointDensity + areaSize.position;

                var spawnedWindPoint = Instantiate(windPointPrefab, new Vector3(pos.x, 0, pos.y), Quaternion.identity, windPointRoot);
                windPoints[y, x] = spawnedWindPoint;
                spawnedWindPoint.Init(settings);
            }
        }
    }

    public override void Refresh()
    {
        float yExtent = minimapCamera.orthographicSize;
        float xExtent = yExtent * minimapCamera.aspect;
        
        int yStart = Mathf.FloorToInt(minimapCamera.transform.position.y - yExtent);
        int yEnd = Mathf.CeilToInt(minimapCamera.transform.position.y + yExtent);

        int xStart = Mathf.FloorToInt(minimapCamera.transform.position.x - xExtent);
        int xEnd = Mathf.CeilToInt(minimapCamera.transform.position.x + xExtent);

        int height = windPoints.GetLength(0);
        int width = windPoints.GetLength(1);

        for (int y = yStart; y <= yEnd; y++)
            for (int x = xStart; x <= xEnd; x++)
                if (x >= 0 && x < width && y >= 0 && y < height)
                    RefreshWindPoint(windPoints[y, x]);
    }

    private void RefreshWindPoint(WindMeasurePoint windPoint)
    {
        if (windPoint)
            windPoint.RefreshWindVelocity();
    }
}
