using System;
using UnityEngine;
using static Unity.Mathematics.math;
using static Unity.Mathematics.noise;

[CreateAssetMenu]
public class WindPressureSettings : ScriptableObject
{
    /*[SerializeField]
    private Vector2 xVelocityNoiseOffset = new Vector2(2.1f, 3.7f);
    [SerializeField]
    private Vector2 yVelocityNoiseOffset = new Vector2(6.9f, 420);*/
    [SerializeField]
    public float noiseScale = 0.05f;

    [SerializeField]
    public float timeScale = 0.1f;

    [SerializeField]
    private float rotate = 0f;

    public Vector2 GetVelocity(float x, float z)
    {
        var time = Time.time;

        /*new Vector2(
            Mathf.PerlinNoise(x / noiseScale + xVelocityNoiseOffset.x, z / noiseScale + xVelocityNoiseOffset.y) * 2 - 1,
            Mathf.PerlinNoise(x / noiseScale + yVelocityNoiseOffset.x, z / noiseScale + yVelocityNoiseOffset.y) * 2 - 1);*/

        var noise = snoise(float3(x * noiseScale, z * noiseScale, time * timeScale), out var gradient);

        sincos(radians(rotate), out float s, out float c);

        return new Vector2(gradient.x * c - gradient.y * s, gradient.x * s + gradient.y * c);
    }

    public float GetPressure(float x, float z)
    {
        var time = Time.time;

        /*new Vector2(
            Mathf.PerlinNoise(x / noiseScale + xVelocityNoiseOffset.x, z / noiseScale + xVelocityNoiseOffset.y) * 2 - 1,
            Mathf.PerlinNoise(x / noiseScale + yVelocityNoiseOffset.x, z / noiseScale + yVelocityNoiseOffset.y) * 2 - 1);*/

        var noise = snoise(float3(x * noiseScale, z * noiseScale, time * timeScale));

        return noise;
    }
}
