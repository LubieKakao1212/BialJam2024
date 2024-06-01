using UnityEngine;

namespace Bipolar.RaycastSystem
{
    [RequireComponent(typeof(Collider)), DisallowMultipleComponent]
    public class RaycastCollider : MonoBehaviour
    {
        public const int ignoreRaycastLayer = 2;

        [SerializeField]
        private RaycastTarget[] raycastTargets;
        public RaycastTarget RaycastTarget
        {
            get
            {
                foreach (var target in raycastTargets)
                    if (target.isActiveAndEnabled)
                        return target;

                return default;
            }
        }

        private int initialLayer;

        private void Reset()
        {
            raycastTargets = GetComponentsInParent<RaycastTarget>();
        }

        private void Awake()
        {
            initialLayer = gameObject.layer;
        }
    }
}
