using static Unity.Mathematics.math;
using UnityEngine;

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

    private float[] tempVN = new float[9];

    public void FindContours(float[,] pressureMap)
    {
        var pixels = new Color32[texture.width * texture.height];

        for (int y = 0; y < texture.height; y++)
            for (int x = 0; x < texture.width; x++)
            {
                var pressureInPoint = pressureMap[x, y];
                var tH = ceil((pressureInPoint + offset) / interval) * interval;

                tempVN[0] = GetBound(pressureMap, x - 1, y - 1);
                tempVN[1] = GetBound(pressureMap, x + 0, y - 1);
                tempVN[2] = GetBound(pressureMap, x + 1, y - 1);

                tempVN[3] = GetBound(pressureMap, x - 1, y + 0);
                tempVN[4] = pressureInPoint;
                tempVN[5] = GetBound(pressureMap, x + 1, y + 0);

                tempVN[6] = GetBound(pressureMap, x - 1, y + 1);
                tempVN[7] = GetBound(pressureMap, x + 0, y + 1);
                tempVN[8] = GetBound(pressureMap, x + 1, y + 1);

                var nH = Mathf.Max(tempVN);

                Color c = nH > tH ? colorGradient.Evaluate((tH + 1f) / 2f) : Color.clear;
                pixels[y * texture.width + x] = c;
            }

        texture.SetPixels32(pixels);
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
