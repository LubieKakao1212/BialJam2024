using UnityEngine;

[ExecuteInEditMode]
public class RotationEnviroArtHelper : MonoBehaviour
{
    [SerializeField]
    private Vector3 minAngles = new Vector3(-20, -180, -20);
    [SerializeField]
    private Vector3 maxAngles = new Vector3(20, 180, 20);

    private void Start()
    {
        if (gameObject.scene.buildIndex < 0)
            return;

        transform.localEulerAngles = new Vector3(
            Random.Range(minAngles.x, maxAngles.x),
            Random.Range(minAngles.y, maxAngles.y),
            Random.Range(minAngles.z, maxAngles.z));

        enabled = false;
    }


}