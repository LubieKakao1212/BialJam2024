using UnityEngine;

namespace Bipolar.RaycastSystem
{
#if !ENABLE_INPUT_SYSTEM
    public class InputManagerRayFromMouseProvider : RayFromMouseProvider
    {
        protected override Vector2 GetScreenPosition()
        {
            return UnityEngine.Input.mousePosition;
        }
    }
#endif
}