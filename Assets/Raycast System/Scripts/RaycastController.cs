using UnityEngine;

namespace Bipolar.RaycastSystem
{
    public abstract class RayProvider : MonoBehaviour
    {
        public abstract Ray GetRay();
    }

    public class RaycastController : MonoBehaviour
    {
        public delegate void RaycastTargetChangeEventHandler(RaycastTarget target);

        public event RaycastTargetChangeEventHandler OnRayEntered;
        public event RaycastTargetChangeEventHandler OnRayExited;

        [Header("Settings")]
        [SerializeField]
        private RayProvider rayProvider;
        public RayProvider RayProvider
        {
            get
            {
                if (rayProvider == null)
                    rayProvider = gameObject.AddComponent<TransformForwardRayProvider>();
                return rayProvider;
            }

            set
            {
                rayProvider = value;
            }
        }

        [SerializeField]
        private LayerMask raycastedLayers;
        public LayerMask RaycastedLayers
        {
            get => raycastedLayers;
            set => raycastedLayers = value;
        }

        [SerializeField]
        private float raycastDistance;
        public float RaycastDistance
        {
            get => raycastDistance;
            set => raycastDistance = value;
        }

        [SerializeField, Min(0)]
        private float radius;
        public float Radius
        {
            get => radius;
            set => radius = value;
        }

        [Header("States")]
        [SerializeField]
        private RaycastTarget currentTarget;
        public RaycastTarget Target => enabled ? currentTarget : null;

        private Ray ray;

        private void Update()
        {
            DoRaycast();
        }

        private void DoRaycast()
        {
            if (TryGetRaycastTarget(out var target))
            {
                if (target != currentTarget)
                {
                    CallExitEvents(currentTarget);
                    currentTarget = target;
                    CallEnterEvents(currentTarget);
                }
                else
                {
                    currentTarget.RayStay();
                }
            }
            else
            {
                ExitCurrentTarget();
            }
        }

        private void ExitCurrentTarget()
        {
            if (currentTarget != null)
            {
                var exitedTarget = currentTarget;
                currentTarget = null;
                CallExitEvents(exitedTarget);
            }
        }

        public bool TryGetRaycastTarget(out RaycastTarget target)
        {
            RayProvider provider = RayProvider;
            ray = provider.GetRay();
            return TryGetRaycastTarget(ray, out target);
        }

        private bool TryGetRaycastTarget(Ray ray, out RaycastTarget target)
        {
            target = null;
            if (TryGetRaycastHit(ray, out var hit) == false)
                return false;

            if (hit.collider.TryGetComponent<RaycastCollider>(out var raycastCollider) == false)
                return false;

            return TryGetRaycastTarget(raycastCollider, out target);
        }

        private bool TryGetRaycastHit(Ray ray, out RaycastHit hit)
        {
            if (radius == 0)
                return Physics.Raycast(ray, out hit, raycastDistance, raycastedLayers);

            return Physics.SphereCast(ray, radius, out hit, raycastDistance, raycastedLayers);
        }

        private static bool TryGetRaycastTarget(RaycastCollider collider, out RaycastTarget target)
        {
            target = collider.RaycastTarget;
            return target != null;
        }

        private void CallEnterEvents(RaycastTarget target)
        {
            if (target != null)
            {
                OnRayEntered?.Invoke(target);
                target.RayEnter();
            }
        }

        private void CallExitEvents(RaycastTarget target)
        {
            if (target != null)
            {
                OnRayExited?.Invoke(target);
                target.RayExit();
            }
        }

        private void OnDisable()
        {
            ExitCurrentTarget();
        }

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.white;
            if (rayProvider == null)
                ray = new Ray(transform.position, transform.forward);
            else
                ray = RayProvider.GetRay();

            if (radius == 0)
                Gizmos.DrawRay(ray.origin, ray.direction * raycastDistance);
            else
                HandlesDrawCapsule(ray, raycastDistance, radius);
        }

        private static void HandlesDrawCapsule(Ray orientation, float height, float radius)
        {
            Vector3 start = orientation.origin;
            Vector3 end = orientation.direction * height + orientation.origin;
            Vector3 localRight = Vector3.Cross(Vector3.up, orientation.direction);
            Vector3 localUp = Vector3.Cross(localRight, orientation.direction);

            UnityEditor.Handles.DrawWireDisc(start, orientation.direction, radius);
            UnityEditor.Handles.DrawWireDisc(end, orientation.direction, radius);

            UnityEditor.Handles.DrawWireArc(start, localUp, -localRight, 180, radius);
            UnityEditor.Handles.DrawWireArc(start, localRight, localUp, 180, radius);

            UnityEditor.Handles.DrawWireArc(end, localUp, localRight, 180, radius);
            UnityEditor.Handles.DrawWireArc(end, localRight, -localUp, 180, radius);

            UnityEditor.Handles.DrawLine(start + localRight * radius, end + localRight * radius);
            UnityEditor.Handles.DrawLine(start - localRight * radius, end - localRight * radius);
            UnityEditor.Handles.DrawLine(start + localUp * radius, end + localUp * radius);
            UnityEditor.Handles.DrawLine(start - localUp * radius, end - localUp * radius);




        }


#endif
    }
}
