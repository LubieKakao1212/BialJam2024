using NaughtyAttributes;
using UnityEngine;

public class CloneTerrainsForMinimap : MonoBehaviour
{
    [SerializeField, Layer]
    private int minimapLayer;

    private void Awake()
    {
        var allTerrains = GetComponentsInChildren<Terrain>();
        foreach (var terrain in allTerrains)
        {
            var minimapTerrain = Instantiate(terrain, terrain.transform, worldPositionStays: true);
            minimapTerrain.gameObject.name = $"{terrain.name} For Minimap";
            if (minimapTerrain.TryGetComponent<TerrainCollider>(out var collider))
                Destroy(collider);
            
            minimapTerrain.drawTreesAndFoliage = false;
            minimapTerrain.drawInstanced = false;
            minimapTerrain.gameObject.layer = minimapLayer; 
        }
    }
}
