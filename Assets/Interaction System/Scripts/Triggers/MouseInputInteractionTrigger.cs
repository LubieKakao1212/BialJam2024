using UnityEngine;
using UnityEngine.UIElements;

#if BIPOLAR_CORE
namespace Bipolar.InteractionSystem.Triggers
{
    [CreateAssetMenu(menuName = "Bipolar/Interaction System/Triggers/Mouse Input Interaction Trigger")]
    public class MouseInputInteractionTrigger : InteractionTrigger
    {
        [SerializeField]
        private MouseButton button;

        public override bool Check()
        {
            return UnityEngine.Input.GetMouseButtonUp((int)button);
        }
    }
}
#endif
