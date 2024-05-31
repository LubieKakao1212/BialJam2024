using System.Collections;
using System.Collections.Generic;
using static Unity.Mathematics.math;
using UnityEngine;
using JetBrains.Annotations;

public class Isobars : WeatherMeasurePoint<Isobars>
{
    [SerializeField]
    private SpriteRenderer sprite;

    private Texture2D texture;

    [SerializeField]
    private float interval;
    [SerializeField]
    private float offset;

    public override Isobars Init(WeatherMapSettings settings)
    {
        texture = new Texture2D(settings.pressureMapResolution.x, settings.pressureMapResolution.y);
        sprite.sprite = Sprite.Create(texture, new Rect(0f, 0f, texture.width, texture.height), new Vector2(0.5f, 0.5f), texture.width);
        return base.Init(settings);
    }

    public void FindContours(float[,] sdf)
    {
        var pixels = new Color[texture.width * texture.height];
        
        for (int y = 0; y < texture.width; y++)
            for (int x = 0; x < texture.width; x++)
            { 
                var v = sdf[x, y];
                var t = ceil((v + offset) / interval) * interval;

                var n = Mathf.Max(v,
                    GetBound(sdf, x - 1, y - 1),
                    GetBound(sdf, x + 0, y - 1),
                    GetBound(sdf, x + 1, y - 1),

                    GetBound(sdf, x - 1, y + 0),
                    //GetBound(sdf, x + 0, y + 0), Self -> v
                    GetBound(sdf, x + 1, y + 0),

                    GetBound(sdf, x - 1, y + 1),
                    GetBound(sdf, x + 0, y + 1),
                    GetBound(sdf, x + 1, y + 1));

                var c = 0f;

                if (n > t)
                {
                    c = 1;
                }

                pixels[y * texture.width + x] = new Color(c, c, c, c);
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
