using UnityEngine;

public class CloneTerrainsForMinimap : MonoBehaviour
{
    private void Awake()
    {
        var allTerrains = GetComponentsInChildren<Terrain>();
        foreach (var terrain in allTerrains)
        {
            var minimapTerrain = Instantiate(terrain, terrain.transform);
            minimapTerrain.drawTreesAndFoliage = false;
            minimapTerrain.drawInstanced = false;
        }
    }
}
