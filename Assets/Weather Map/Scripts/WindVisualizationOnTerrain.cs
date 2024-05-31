using System;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

[RequireComponent (typeof(Terrain))]
public class WindVisualizationOnTerrain : MonoBehaviour
{
    [SerializeField]
    private WindSettings windSettings;

    private Terrain _terrain;
    public Terrain Terrain
    {
        get
        {
            if (_terrain == null)
                _terrain = GetComponent<Terrain>();
            return _terrain;
        }
    }

    private void Start()
    {
        RefreshColors();
    }

    [ContextMenu("Refresh Colors")]
    public void RefreshColors()
    {
        var terrainData = Terrain.terrainData;
        int alphamapResolution = terrainData.alphamapResolution;
        float oneOverResolution = 1f / alphamapResolution;

        float[,,] alphamaps = new float[alphamapResolution, alphamapResolution, 3];

        var terrainSize = terrainData.size;
        float oneOverWidth = 1f / terrainSize.x;
        float oneOverLength = 1f / terrainSize.z;

        for (int j = 0; j < alphamapResolution; j++)
        {
            for (int i = 0; i < alphamapResolution; i++)
            {
                var worldPosition = GetWorldPosition(i, j);
                var windVelocity = windSettings.GetVelocity(worldPosition.x, worldPosition.y);
                float atan2 = Mathf.Atan2(-windVelocity.y, -windVelocity.x);
                float angle = Mathf.InverseLerp(-Mathf.PI, Mathf.PI, atan2);
                Color color = Color.HSVToRGB(angle, 1, 1);

                for (int c = 0; c <= 2; c++)
                {
                    alphamaps[j, i, c] = color[c];
                    //alphamaps[i, j, c] = color[c];
                    //float speedAlongAxis = windVelocity[c];
                    //if (speedAlongAxis < 0)
                    //{
                    //    alphamaps[j, i, (c + 1) % 3] -= speedAlongAxis;
                    //    alphamaps[j, i, (c + 2) % 3] -= speedAlongAxis;
                    //}
                    //else
                    //{
                    //    alphamaps[j, i, c] += speedAlongAxis;
                    //}
                }
            }
        }
        terrainData.SetAlphamaps(0, 0, alphamaps);

        Vector2 GetWorldPosition(int xAlphamap, int yAlphamap)
        {
            float x01 = xAlphamap * oneOverResolution; 
            float y01 = yAlphamap * oneOverResolution;
            float xWorld = x01 * terrainSize.x + transform.position.x;
            float yWorld = y01 * terrainSize.z + transform.position.z;
            return new Vector2(xWorld, yWorld);
        }
    }
}
