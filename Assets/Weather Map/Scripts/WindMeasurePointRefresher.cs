using UnityEngine;

[RequireComponent(typeof(WindMeasurePoint))]
public class WindMeasurePointRefresher : MonoBehaviour
{
    private WindMeasurePoint windMeasurePoint;

    private void Awake()
    {
        windMeasurePoint = GetComponent<WindMeasurePoint>();
    }

    private void Update()
    {
        windMeasurePoint.RefreshWindVelocity();
    }
}