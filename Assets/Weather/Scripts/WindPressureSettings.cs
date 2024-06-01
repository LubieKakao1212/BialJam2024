using UnityEngine;
using static Unity.Mathematics.math;
using static Unity.Mathematics.noise;

[CreateAssetMenu]
public class WindPressureSettings : ScriptableObject
{
    [SerializeField]
    public float noiseScale = 0.05f;

    [SerializeField]
    public float timeScale = 0.1f;

    [SerializeField]
    private float rotate = 0f;

    public Vector2 GetVelocity(float x, float z)
    {
        var time = Time.time;
        var noise = snoise(float3(x * noiseScale, z * noiseScale, time * timeScale), out var gradient);

        sincos(radians(rotate), out float s, out float c);

        return new Vector2(gradient.x * c - gradient.y * s, gradient.x * s + gradient.y * c);
    }
}
