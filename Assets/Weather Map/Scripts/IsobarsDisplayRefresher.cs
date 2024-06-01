using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using static Unity.Mathematics.math;

public class IsobarsDisplayRefresher : WeatherDisplayRefresher
{
    [SerializeField]
    private WeatherMapSettings settings;

    [SerializeField]
    private Isobars isobars;

    private NativeArray<float> pressureMap;

    private float[] pressureMapOut;

    private void Awake()
    {
        isobars.Init(settings);
        var resolution = settings.pressureMapResolution;
        pressureMap = new NativeArray<float>(resolution.x * resolution.y, Allocator.Persistent);
        pressureMapOut = new float[resolution.x * resolution.y];
    }

    public override void Refresh()
    {
        RefreshIsobars();
    }

    private void RefreshIsobars()
    {
        var resolution = new int2(settings.pressureMapResolution.x, settings.pressureMapResolution.y);
        var scale = 60f * float2(1f) / resolution.y;
        var position = (float3)isobars.transform.position;

        var offset = float2(-resolution) / 2f + position.xz / (scale * 2f);
        var compoundScale = 2f * scale * settings.windPressure.noiseScale;

        var noiseJob = new GenNoiseJob()
        {
            w = resolution.x,
            h = resolution.y,
            time = Time.time * settings.windPressure.timeScale,
            offset = offset,
            scale = compoundScale,
            output = pressureMap
        };

        noiseJob.Schedule(resolution.x * resolution.y, 512).Complete();
        isobars.FindContours(pressureMap);
    }

    private void OnDestroy()
    {
        pressureMap.Dispose();
    }
}
