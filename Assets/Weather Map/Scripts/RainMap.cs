using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using static Unity.Mathematics.math;

public class RainMap : MonoBehaviour
{
    private const string wetnessMapName = "_Wetness";

    [SerializeField]
    private Vector2Int resolution;

    [SerializeField]
    private float changeSpeed;
    [SerializeField]
    private float noiseScale;

    [SerializeField]
    private Material material;
    [SerializeField]
    private ParticleSystem rainParticleSystem;

    private Vector2 randomOffset;
    private NativeArray<float> rainNativeArray;

    [SerializeField, NaughtyAttributes.ReadOnly]
    private Texture2D rainMap;

    private void Awake()
    {
        randomOffset = new Vector2(
            UnityEngine.Random.Range(-10000, 10000),
            UnityEngine.Random.Range(-10000, 10000));

        rainNativeArray = new NativeArray<float>(resolution.x * resolution.y, Allocator.Persistent);
        rainMap = new Texture2D(resolution.x, resolution.y, TextureFormat.RFloat, false);
    }   

    private void Start()
    {
        material.SetTexture(wetnessMapName, rainMap);
        var particleSystemShape = rainParticleSystem.shape;
        var rainGenerationSprite = Sprite.Create(rainMap, new Rect(Vector2.zero, resolution), new Vector2(0.5f, 0.5f));
        particleSystemShape.sprite = rainGenerationSprite;
    }

    private void Update()
    {
        var position = (float3)transform.position;
        var scale = 60f * float2(1f) / resolution.y;
        var offset = float2(-(Vector2)resolution) / 2f + position.xz / (scale * 2f);
        var compoundScale = 2f * scale * noiseScale;

        var noiseJob = new GenNoiseJob()
        {
            w = resolution.x,
            h = resolution.y,
            time = Time.time * changeSpeed,
            offset = offset, 
            scale = compoundScale,
            output = rainNativeArray,
        };

        noiseJob.Schedule(resolution.x * resolution.y, 512).Complete();
        rainMap.SetPixelData(rainNativeArray, 0);
        rainMap.Apply();
    }

    private void OnDestroy()
    {
        rainNativeArray.Dispose();
    }
}
