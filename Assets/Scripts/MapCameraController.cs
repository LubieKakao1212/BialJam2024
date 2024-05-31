using UnityEngine;
using UnityEngine.Rendering;

public class MapCameraController : MonoBehaviour
{
    [SerializeField]
    private Transform followedPlayer;
    [SerializeField]
    private Bounds movementBounds;

    [SerializeField]
    private Camera cameraData;

    [SerializeField]
    private bool renderFog;

    private bool fog;

    private void OnEnable()
    {
        RenderPipelineManager.beginCameraRendering += RenderPipelineManager_beginCameraRendering;
        RenderPipelineManager.endCameraRendering += RenderPipelineManager_endCameraRendering;
    }

    private void RenderPipelineManager_beginCameraRendering(ScriptableRenderContext context, Camera camera)
    {
        if (camera == cameraData)
        {
            fog = RenderSettings.fog;
            if (renderFog == false)
                RenderSettings.fog = false;
        }
    }

    private void RenderPipelineManager_endCameraRendering(ScriptableRenderContext context, Camera camera)
    {
        if (camera == cameraData)
            RenderSettings.fog = fog;
    }

    private void LateUpdate()
    {
        float zExtent = cameraData.orthographicSize;
        float xExtent = cameraData.aspect * zExtent;

        Vector3 position = followedPlayer.position;
        position.x = Mathf.Clamp(position.x, movementBounds.min.x + xExtent, movementBounds.max.x - xExtent);
        position.z = Mathf.Clamp(position.z, movementBounds.min.z + zExtent, movementBounds.max.z - zExtent);

        transform.position = new Vector3(position.x, transform.position.y, position.z);
    }

    private void OnDisable()
    {
        RenderPipelineManager.beginCameraRendering -= RenderPipelineManager_beginCameraRendering;
        RenderPipelineManager.endCameraRendering -= RenderPipelineManager_endCameraRendering;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(movementBounds.center, movementBounds.size);
    } 
}
