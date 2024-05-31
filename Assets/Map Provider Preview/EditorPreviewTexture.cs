#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

public class EditorPreviewTexture
{
    public delegate Color GetColorCallback(int x, int y);

    private readonly int _xResolution = 100;
    private readonly int _yResolution = 100;
    private readonly GetColorCallback _colorFunc;
    private readonly Color32[] _colors;
    private readonly Texture2D _previewTexture;

    public EditorPreviewTexture(int xResolution, int yResolution, GetColorCallback colorFunc)
    {
        _xResolution = xResolution;
        _yResolution = yResolution;
        _colorFunc = colorFunc;
        _colors = new Color32[xResolution * yResolution];
        _previewTexture = CreateTexture(xResolution, yResolution);
    }

    public Texture2D PreviewTexture => _previewTexture;

    private static Texture2D CreateTexture(int xResolution, int yResolution)
    {
        return new Texture2D(xResolution, yResolution, TextureFormat.RGBA32, false, true)
        {
            wrapMode = TextureWrapMode.Clamp
        };
    }

    public void DrawPreview(Rect rect, bool repopulate = false)
    {
        if (repopulate)
            PopulatePreviewTexture();

        EditorGUI.DrawPreviewTexture(rect, PreviewTexture);
    }

    public void PopulatePreviewTexture()
    {
        int xHalfResolution = _xResolution / 2;
        int yHalfResolution = _yResolution / 2;
        for (int j = 0; j < _yResolution; j++)
        {
            for (int i = 0; i < _xResolution; i++)
            {
                // one pixel in preview = one unit in world
                var pixelColor = _colorFunc.Invoke(i - xHalfResolution, j - yHalfResolution);
                _colors[j * _xResolution + i] = pixelColor;
            }
        }

        PreviewTexture.SetPixels32(_colors);
        PreviewTexture.Apply();
    }
}
#endif
