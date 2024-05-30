using UnityEngine;

public class TemperatureMeasurePointGraphic : MonoBehaviour
{
    [SerializeField]
    private WeatherGraphicSettings graphicSettings;

    [SerializeField]
    private TemperatureMeasurePoint measurePoint;
    [SerializeField]
    private SpriteRenderer spriteRenderer;

    private void Reset()
    {
        measurePoint = GetComponentInParent<TemperatureMeasurePoint>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    private void Awake()
    {
        spriteRenderer.transform.forward = Vector3.down;
    }

    private void Start()
    {
        RefreshGraphic();
    }

    private void RefreshGraphic()
    {
        spriteRenderer.sprite = graphicSettings.GetGraphic(measurePoint.CurrentWeather);
    }
}
