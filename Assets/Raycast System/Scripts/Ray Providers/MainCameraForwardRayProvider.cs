using UnityEngine;

namespace Bipolar.RaycastSystem
{
    public class MainCameraForwardRayProvider : RayProvider
    {
        private Transform mainCameraTransform;
        private Transform MainCameraTransform
        {
            get 
            {
                if (mainCameraTransform == null)
                    mainCameraTransform = Camera.main.transform;
                return mainCameraTransform; 
            }
        }

        public override Ray GetRay()
        {
            return new Ray(MainCameraTransform.position, MainCameraTransform.forward);
        }
    }
}
