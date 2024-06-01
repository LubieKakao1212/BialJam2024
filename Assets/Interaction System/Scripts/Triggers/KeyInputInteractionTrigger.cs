using UnityEngine;

namespace Bipolar.InteractionSystem.Triggers
{ 
    [CreateAssetMenu(menuName = "Bipolar/Interaction System/Triggers/Key Input Interaction Trigger")]
    public class KeyInputInteractionTrigger : InteractionTrigger
    {
        [SerializeField]
        private KeyCode key;

        public override bool Check()
        {
            return UnityEngine.Input.GetKeyUp(key);
        }
    }
}
