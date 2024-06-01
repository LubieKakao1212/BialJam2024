using UnityEngine;
using UnityEngine.Events;

namespace Bipolar.InteractionSystem
{
    public class GenericInteraction : Interaction
    {
        [SerializeField]
        private UnityEvent onInteract;

        public override void Interact(Interactor interactor)
        {
            onInteract.Invoke();
        }
    }
}
