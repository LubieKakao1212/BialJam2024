using UnityEngine;

public class TemperatureMeasurePointGraphic : MonoBehaviour
{
    [SerializeField]
    private WeatherGraphicSettings graphicSettings;

    [SerializeField]
    private TemperatureMeasurePoint measurePoint;
    [SerializeField]
    private Transform visualHolder;

    private void Reset()
    {
        measurePoint = GetComponentInParent<TemperatureMeasurePoint>();
    }

    private void Awake()
    {
        visualHolder.forward = Vector3.down;
    }

    private void Start()
    {
        RefreshGraphic();
    }

    private void RefreshGraphic()
    {
        for (int i = 0; i < visualHolder.childCount; i++)
        {
            Destroy(visualHolder.GetChild(i).gameObject);
        }
        var visualPrototype = graphicSettings.GetGraphic(measurePoint.CurrentWeather);
        var visual = Instantiate(visualPrototype, visualHolder);
    }
}
