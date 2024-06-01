using System.Collections;
using System.Collections.Generic;
using static Unity.Mathematics.math;
using UnityEngine;
using JetBrains.Annotations;
using Unity.VisualScripting;

public class Isobars : WeatherMeasurePoint<Isobars>
{
    [SerializeField]
    private SpriteRenderer sprite;

    private Texture2D texture;

    [SerializeField]
    private float interval;
    [SerializeField]
    private float offset;

    [SerializeField]
    private Gradient colorGradient;

    public override Isobars Init(WeatherMapSettings settings)
    {
        texture = new Texture2D(settings.pressureMapResolution.x, settings.pressureMapResolution.y);
        texture.filterMode = FilterMode.Point;
        sprite.sprite = Sprite.Create(texture, new Rect(0f, 0f, texture.width, texture.height), new Vector2(0.5f, 0.5f), texture.height);
        return base.Init(settings);
    }

    public void FindContours(float[,] sdf)
    {
        var pixels = new Color[texture.width * texture.height];
        
        for (int y = 0; y < texture.height; y++)
            for (int x = 0; x < texture.width; x++)
            { 
                var v = sdf[x, y];
                var tH = ceil((v + offset) / interval) * interval;
                //var tL = floor((v + offset) / interval) * interval;

                var vn = new float[]
                {
                    GetBound(sdf, x - 1, y - 1),
                    GetBound(sdf, x + 0, y - 1),
                    GetBound(sdf, x + 1, y - 1),

                    GetBound(sdf, x - 1, y + 0),
                    v,//GetBound(sdf, x + 0, y + 0), Self -> v
                    GetBound(sdf, x + 1, y + 0),

                    GetBound(sdf, x - 1, y + 1),
                    GetBound(sdf, x + 0, y + 1),
                    GetBound(sdf, x + 1, y + 1)
                };

                var nH = Mathf.Max(vn);
                //var nL = Mathf.Min(vn);

                var c = Color.clear;

                if (nH > tH)
                {
                    c = colorGradient.Evaluate((tH + 1f) / 2f);
                }
                /*else if(nL < tL)
                {
                    c = new Color(1f, 0.5f, 0.5f);
                }*/

                pixels[y * texture.width + x] = c;
            }

        texture.SetPixels(pixels);
        texture.Apply();
    }

    private float GetBound(float[,] sdf, int x, int y)
    {
        if (
            x < 0 || x >= texture.width ||
            y < 0 || y >= texture.height)
        {
            return float.NegativeInfinity;
        }
        return sdf[x, y];
    }
}
