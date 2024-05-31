using System;
using Unity.VisualScripting;
using UnityEngine;

public class MovingCloud : MonoBehaviour
{
    [SerializeField]
    private float movementSpeed;

    [SerializeField]
    private float amplitude;

    private void Update()
    {
        float x = amplitude * Mathf.Sin(Time.time * movementSpeed * Mathf.PI);
        transform.localPosition = new Vector3(x, 0);
    }
}
