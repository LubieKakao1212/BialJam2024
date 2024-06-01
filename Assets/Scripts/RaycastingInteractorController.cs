using Bipolar.RaycastSystem;
using UnityEngine;

namespace Bipolar.InteractionSystem
{
    public class RaycastingInteractorController : InteractorController 
    {
        [SerializeField]
        private RaycastController raycastController;

        private void OnEnable()
        {
            raycastController.OnRayEntered += TryGetInteractiveObjectFromRaycastTarget;
            raycastController.OnRayExited += RaycastController_OnRayExited;
        }

        private void TryGetInteractiveObjectFromRaycastTarget(RaycastTarget target)
        {
            target.TryGetComponent<InteractiveObject>(out var interactiveObject);
            Interactor.CurrentInteractiveObject = interactiveObject;
        }

        private void RaycastController_OnRayExited(RaycastTarget target)
        {
            Interactor.CurrentInteractiveObject = null;
        }

        private void OnDisable()
        {
            raycastController.OnRayEntered -= TryGetInteractiveObjectFromRaycastTarget;
            raycastController.OnRayExited -= RaycastController_OnRayExited;
        }
    }
}
