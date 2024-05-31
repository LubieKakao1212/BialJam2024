using UnityEngine;
using TMPro;

public class WindSpeedLabel : MonoBehaviour
{
    [SerializeField]
    private WindMeasurePoint measurePoint;
    [SerializeField]
    private TextMeshProUGUI speedLabel;

    private void Update()
    {
        speedLabel.text = $"{(int)Mathf.LerpUnclamped(0, 60, measurePoint.WindVelocity.magnitude)} km/h";
    }
}
