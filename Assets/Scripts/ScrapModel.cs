using UnityEngine;

public class ScrapModel : MonoBehaviour
{
    private MeshRenderer[] renderers;
    private Material[] materials;

    [SerializeField]
    private Material highlightedMaterial;

    private void Awake()
    {
        int modelIndex = Random.Range(0, transform.childCount);
        for (int i = 0; i < transform.childCount; i++)
        {
            bool isActiveModel = i == modelIndex;
            transform.GetChild(i).gameObject.SetActive(isActiveModel);
        }
            
        renderers = transform.GetChild(modelIndex).GetComponentsInChildren<MeshRenderer>();
        materials = new Material[renderers.Length];
        for (int i = 0; i < renderers.Length; i++)
            materials[i] = renderers[i].sharedMaterial;
        
    }

    public void Highlight(bool highlight) // used in UnityEvent
    {
        for (int i = 0; i < renderers.Length; i++)
            renderers[i].material = highlight ? highlightedMaterial : materials[i];
    }
}
