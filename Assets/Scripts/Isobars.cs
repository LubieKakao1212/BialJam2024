using static Unity.Mathematics.math;
using UnityEngine;
using UnityEngine.Profiling;
using Unity.Collections;
using Unity.Jobs;
using Unity.Burst;

public class Isobars : WeatherMeasurePoint<Isobars>
{
    [SerializeField]
    private SpriteRenderer sprite;

    private Texture2D texture;

    [SerializeField]
    private float interval;
    [SerializeField]
    private float offset;

    /*[SerializeField]
    private Gradient colorGradient;*/
    
    NativeArray<Color32> pixels;

    public override Isobars Init(WeatherMapSettings settings)
    {
        texture = new Texture2D(settings.pressureMapResolution.x, settings.pressureMapResolution.y, TextureFormat.RGBA32, false);
        texture.filterMode = FilterMode.Point;
        sprite.sprite = Sprite.Create(texture, new Rect(0f, 0f, texture.width, texture.height), new Vector2(0.5f, 0.5f), texture.height);
        pixels = new NativeArray<Color32>(texture.width * texture.height, Allocator.Persistent);
        return base.Init(settings);
    }
    
    public void FindContours(in NativeArray<float> pressureMap)
    {
        Profiler.BeginSample("Isobars");

        /*for (int y = 0; y < texture.height; y++)
            for (int x = 0; x < texture.width; x++)
            {
                
            }*/

        new ConturJob()
        {
            pressureMap = pressureMap,
            output = pixels,
            interval = interval,
            offset = offset,
            w = texture.width,
            h = texture.height
        }.Schedule(pressureMap.Length, 512).Complete();

        texture.SetPixelData(pixels, 0);
        texture.Apply();

        Profiler.EndSample();
    }

    private void OnDestroy()
    {
        pixels.Dispose();
    }

    private float GetBound(float[] sdf, int x, int y)
    {
        if (
            x < 0 || x >= texture.width ||
            y < 0 || y >= texture.height)
        {
            return float.NegativeInfinity;
        }
        var i = y * texture.width + x;
        return sdf[i];
    }

    [BurstCompile]
    private struct ConturJob : IJobParallelFor
    {
        [WriteOnly]
        public NativeArray<Color32> output;
        
        [ReadOnly]
        public NativeArray<float> pressureMap;

        public int w, h;

        public float offset, interval;

        public void Execute(int index)
        {
            var x = index % w;
            var y = index / w;
            
            var pressureInPoint = pressureMap[index];
            var tH = ceil(((pressureInPoint + offset) - offset) / interval) * interval;

            float v0 = GetBound(x - 1, y - 1);
            float v1 = GetBound(x + 0, y - 1);
            float v2 = GetBound(x + 1, y - 1);

            float v3 = GetBound(x - 1, y + 0);
            float v4 = pressureInPoint;
            float v5 = GetBound(x + 1, y + 0);

            float v6 = GetBound(x - 1, y + 1);
            float v7 = GetBound(x + 0, y + 1);
            float v8 = GetBound(x + 1, y + 1);

            var nH = max(v0, max(v1, max(v2, max(v3, max(v4, max(v5, max(v6, max(v7, v8))))))));

            
            Color32 c = new Color32(0,0,0,0);
            if(nH > tH)
            {
                tH *= -1;
                if (tH < -0.66f)
                {
                    c = new Color32(0, 0, 255, 255);
                }
                else if (tH > 0.66f)
                {
                    c = new Color32(255, 0, 0, 255);
                }
                else if (tH < 0f)
                {
                    c = Color32.Lerp(new Color32(255, 255, 255, 255), new Color32(0, 0, 255, 255), -tH / 0.66f);
                }
                else
                {
                    c = Color32.Lerp(new Color32(255, 255, 255, 255), new Color32(255, 0, 0, 255), tH / 0.66f);
                }
            }

            output[index] = c;

            //Color c =  ? colorGradient.Evaluate((tH + 1f) / 2f) : Color.clear;
        }

        private float GetBound(int x, int y)
        {
            if (
            x < 0 || x >= w ||
            y < 0 || y >= h)
            {
                return float.NegativeInfinity;
            }
            var i = y * w + x;
            return pressureMap[i];
        }
    }

}
