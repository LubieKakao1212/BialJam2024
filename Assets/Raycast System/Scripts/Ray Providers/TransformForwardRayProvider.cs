using UnityEngine;

namespace Bipolar.RaycastSystem
{
    public class TransformForwardRayProvider : RayProvider
    {
        public override Ray GetRay()
        {
            return new Ray(transform.position, transform.forward);
        }
    }
}
