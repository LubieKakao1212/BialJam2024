using UnityEngine;

public class WindMeasurePointGraphic : MonoBehaviour
{
    [SerializeField]
    private WindMeasurePoint measurePoint;
    [SerializeField]
    private Transform visual;
    [SerializeField]
    private Transform visualScaleRoot;

    private void Reset()
    {
        measurePoint = GetComponentInParent<WindMeasurePoint>();
    }

    private void OnEnable()
    {
        measurePoint.OnVelocityChanged += RefreshGraphic;
    }

    private void Start()
    {
        RefreshGraphic(measurePoint.WindVelocity);
    }

    [ContextMenu("Refresh Graphic")]
    private void RefreshGraphic(Vector2 windVelocity)
    {
        var measurePointForward = measurePoint.transform.forward;
        float angle = Vector2.SignedAngle(Vector2.up, windVelocity);
        float pointAngle = Vector2.SignedAngle(Vector2.up, new Vector2(measurePointForward.x, measurePointForward.z));
        
        visual.localEulerAngles = new Vector3(0, 0, angle - pointAngle);
        if (visualScaleRoot != null)
        {
            float m = Mathf.Log(windVelocity.magnitude + 2f) / 2f;

            visualScaleRoot.localScale = new Vector3(m, m, m);
        }
    }

    private void OnDisable()
    {
        measurePoint.OnVelocityChanged -= RefreshGraphic;
    }
}
