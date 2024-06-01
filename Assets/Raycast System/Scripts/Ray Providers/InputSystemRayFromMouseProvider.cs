#if ENABLE_INPUT_SYSTEM
using UnityEngine;
using UnityEngine.InputSystem;

namespace Bipolar.RaycastSystem
{
    public class InputSystemRayFromMouseProvider : RayFromMouseProvider
    {
        protected override Vector2 GetScreenPosition()
        {
            return Mouse.current.position.ReadValue();
        }
    }
}
#endif
