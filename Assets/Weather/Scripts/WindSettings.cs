using UnityEngine;

[CreateAssetMenu]
public class WindSettings : ScriptableObject
{
    [SerializeField]
    private Vector2 xVelocityNoiseOffset = new Vector2(2.1f, 3.7f);
    [SerializeField]
    private Vector2 yVelocityNoiseOffset = new Vector2(6.9f, 420);
    [SerializeField]
    private float noiseScale = 20;

    public Vector2 GetVelocity(float x, float z) => new Vector2(
        Mathf.PerlinNoise(x / noiseScale + xVelocityNoiseOffset.x, z / noiseScale + xVelocityNoiseOffset.y) * 2 - 1,
        Mathf.PerlinNoise(x / noiseScale + yVelocityNoiseOffset.x, z / noiseScale + yVelocityNoiseOffset.y) * 2 - 1);
}
