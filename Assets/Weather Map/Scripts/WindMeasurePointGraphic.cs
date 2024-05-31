using UnityEngine;

public class WindMeasurePointGraphic : MonoBehaviour
{
    [SerializeField]
    private WindMeasurePoint measurePoint;
    [SerializeField]
    private Transform visual;

    private void Reset()
    {
        measurePoint = GetComponentInParent<WindMeasurePoint>();
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
        var measurePointForward = measurePoint.transform.forward;
        var windVelocity = measurePoint.WindVelocity;
        float angle = Vector2.SignedAngle(Vector2.up, windVelocity);
        float pointAngle = Vector2.SignedAngle(Vector2.up, new Vector2(measurePointForward.x, measurePointForward.z));

        visual.localEulerAngles = new Vector3(0, 0, angle - pointAngle);
    }
}
