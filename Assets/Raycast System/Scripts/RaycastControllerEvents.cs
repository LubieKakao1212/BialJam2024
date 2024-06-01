using UnityEngine;
using UnityEngine.Events;

namespace Bipolar.RaycastSystem
{
    [RequireComponent(typeof(RaycastController))]
    public class RaycastControllerEvents : MonoBehaviour
    {
        private RaycastController raycastController;

        [SerializeField]
        private UnityEvent<RaycastTarget> onRayEnter;
        [SerializeField]
        private UnityEvent<RaycastTarget> onRayExit;

        private void Awake()
        {
            raycastController = GetComponent<RaycastController>();
        }

        private void OnEnable()
        { 
            raycastController.OnRayEntered += CallEnterEvent;
            raycastController.OnRayExited += CallExitEvent;
        }

        private void CallEnterEvent(RaycastTarget target)
        {
            onRayEnter.Invoke(target);
        }

        private void CallExitEvent(RaycastTarget target)
        {
            onRayExit.Invoke(target);
        }

        private void OnDisable()
        {
            raycastController.OnRayEntered -= CallEnterEvent;
            raycastController.OnRayExited -= CallExitEvent;
        }

    }
}
