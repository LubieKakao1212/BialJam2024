using UnityEngine;
using UnityEngine.Rendering;

public class WindMeasurePointGraphic : MonoBehaviour
{
    [SerializeField]
    private WindMeasurePoint measurePoint;
    [SerializeField]
    private Transform graphic;

    private void Reset()
    {
        measurePoint = GetComponentInParent<WindMeasurePoint>();
        graphic = GetComponentInChildren<SpriteRenderer>()?.transform;
    }

    private void Start()
    {
        RefreshGraphic();
    }

    private void Update()
    {
        RefreshGraphic();
    }

    [ContextMenu("Refresh Graphic")]
    private void RefreshGraphic()
    {
        var windVelocity = measurePoint.WindVelocity;
        float angle = Vector2.SignedAngle(Vector3.up, windVelocity);
        transform.eulerAngles = new Vector3(90, angle, 0);
    }
}
