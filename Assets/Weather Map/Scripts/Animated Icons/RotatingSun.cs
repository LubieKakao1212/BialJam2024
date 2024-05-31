using NaughtyAttributes;
using UnityEngine;

public class RotatingSun : MonoBehaviour
{
    [SerializeField]
    private float rotationSpeed;

    [SerializeField, ReadOnly]
    private float angle;

    private void Update()
    {
        angle += Time.deltaTime * rotationSpeed;

        int tooMuchRotations = (int)angle / 360;
        if (tooMuchRotations != 0)
            angle -= tooMuchRotations * 360;

        transform.localRotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }
}
