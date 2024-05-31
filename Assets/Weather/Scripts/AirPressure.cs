using NaughtyAttributes;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class AirPressure : MonoBehaviour
{
    [Header("Noise Settings")]
    [SerializeField]
    private int seed;
    [SerializeField]
    private FastNoiseLite.NoiseType noiseType;
    [SerializeField]
    private float noiseScale;

    [Header("Properties")]
    [SerializeField]
    private float changeSpeed;
    [SerializeField, ReadOnly]
    private float timer;

    private FastNoiseLite _noise;
    public FastNoiseLite Noise
    {
        get
        {
            ValidateNoise();
            return _noise;
        }
    }

    private void ValidateNoise()
    {
        _noise ??= new FastNoiseLite(seed);
        _noise.SetSeed(seed);
        _noise.SetNoiseType(noiseType);
        _noise.SetFrequency(1f / noiseScale);
    }

    public float GetPressure(float x, float z)
    {
        return Mathf.InverseLerp(-1, 1, Noise.GetNoise(x, z, timer));
    }

    private void Update()
    {
        timer += Time.deltaTime * changeSpeed;
    }

    private void OnValidate()
    {
        ValidateNoise();
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(AirPressure))]
public class AirPressureSettingsEditor : Editor
{
    private EditorPreviewTexture previewTexture;

    private void OnEnable()
    {
        previewTexture ??= new EditorPreviewTexture(100, 100, GetColorOnTexture);
        previewTexture.PopulatePreviewTexture();
    }
    public override void OnInspectorGUI()
    {
        EditorGUI.BeginChangeCheck();
        base.OnInspectorGUI();

        if (EditorGUI.EndChangeCheck() || EditorApplication.isPlaying)
        {
            previewTexture.PopulatePreviewTexture();
        }
    }

    public override bool HasPreviewGUI() => true;
    public override void OnPreviewGUI(Rect r, GUIStyle background)
    {
        base.OnPreviewGUI(r, background);
        previewTexture.DrawPreview(r);
    }

    private Color GetColorOnTexture(int x, int y)
    {
        var airPressure = target as AirPressure;
        float pressure = airPressure.GetPressure(x, y);
        return Color.Lerp(Color.blue, Color.white, pressure);
    }
}
#endif