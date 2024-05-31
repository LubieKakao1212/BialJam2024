using UnityEngine;
using UnityEngine.Rendering;

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
        var windVelocity = measurePoint.WindVelocity;
        float angle = Vector2.SignedAngle(Vector3.up, windVelocity);
        visual.localEulerAngles = new Vector3(0, 0, angle);
    }
}
