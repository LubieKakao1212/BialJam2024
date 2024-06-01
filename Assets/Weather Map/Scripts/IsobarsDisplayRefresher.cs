using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using static Unity.Mathematics.math;
using static Unity.Mathematics.noise;

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

        /*for (int x = 0; x < settings.pressureMapResolution.x; x++)
            for (int y = 0; y < settings.pressureMapResolution.y; y++)
            {
                pressureMap[x, y] = settings.windPressure.GetPressure(
                    (x - resolution.x / 2f) * 2f * scale.x + position.x,
                    (y - resolution.y / 2f) * 2f * scale.y + position.z);
            }
        //-resolution + positoion
        //(float2(x, y) + offset) * scale
*/
        var noiseJob = new GenNoiseJob() {
            w = resolution.x, h = resolution.y,
            time = Time.time * settings.windPressure.timeScale,
            offset = offset, scale = compoundScale,
            output = pressureMap
        };

        noiseJob.Schedule(resolution.x * resolution.y, 512).Complete();

        //pressureMap.CopyTo(pressureMapOut);

        isobars.FindContours(pressureMap);
    }

    private void OnDestroy()
    {
        pressureMap.Dispose();
    }

    [BurstCompile]
    private struct GenNoiseJob : IJobParallelFor
    {
        [WriteOnly]
        public NativeArray<float> output;

        public float time;

        public int w, h;

        public float2 scale, offset;

        public void Execute(int index)
        {
            var x = index % w;
            var y = index / w;

            output[index] = snoise(float3((float2(x, y) + offset) * scale, time));
        }
    }
}
