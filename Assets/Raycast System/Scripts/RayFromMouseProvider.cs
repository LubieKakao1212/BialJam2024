using UnityEngine;

namespace Bipolar.RaycastSystem
{
    public abstract class RayFromMouseProvider : RayProvider
    {
        [SerializeField]
        private new Camera camera;
        private Camera Camera 
        {
            get 
            {
                if (camera == null)
                    FindCamera();
                return camera; 
            }
        }

        private void Reset()
        {
            FindCamera();
        }

        private void FindCamera()
        {
            if (TryGetComponent(out camera) == false)
                camera = Camera.main;
        }

        public override Ray GetRay()
        {
            Vector2 screenMousePosition = GetScreenPosition();
            return Camera.ScreenPointToRay(screenMousePosition);
        }

        protected abstract Vector2 GetScreenPosition();
    }
}