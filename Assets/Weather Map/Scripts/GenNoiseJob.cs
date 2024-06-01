using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using static Unity.Mathematics.math;
using static Unity.Mathematics.noise;

[BurstCompile]
public struct GenNoiseJob : IJobParallelFor
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
