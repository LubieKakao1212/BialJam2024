using Unity.VisualScripting;
using UnityEngine;

public class MapCameraController : MonoBehaviour
{
    [SerializeField]
    private Transform followedPlayer;
    [SerializeField]
    private Bounds movementBounds;

    [SerializeField]
    private Camera cameraData;

    private void LateUpdate()
    {
        float zExtent = cameraData.orthographicSize;
        float xExtent = cameraData.aspect * zExtent;

        Vector3 position = followedPlayer.position;
        position.x = Mathf.Clamp(position.x, movementBounds.min.x + xExtent, movementBounds.max.x - xExtent);
        position.z = Mathf.Clamp(position.z, movementBounds.min.z + zExtent, movementBounds.max.z - zExtent);

        transform.position = new Vector3(position.x, transform.position.y, position.z);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(movementBounds.center, movementBounds.size);
    } 
}
