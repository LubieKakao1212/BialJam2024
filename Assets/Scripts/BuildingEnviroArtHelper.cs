using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class BuildingEnviroArtHelper : MonoBehaviour
{
    private void Start()
    {
        if (gameObject.scene.buildIndex < 0)
            return;

        var meshRenderers = new List<MeshRenderer>();
        GetComponentsInChildren(meshRenderers);
        int removedMeshesCount = Mathf.RoundToInt(Random.Range(0.1f * meshRenderers.Count, 0.9f * meshRenderers.Count));
        for (int i = 0; i < removedMeshesCount; i++)
        {
            int index = Random.Range(0, meshRenderers.Count);
            var meshToRemove = meshRenderers[index];
            meshRenderers.RemoveAt(index);
            meshToRemove.gameObject.SetActive(false);
        }

        enabled = false;
    }
}
