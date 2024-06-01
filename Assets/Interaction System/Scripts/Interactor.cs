#if NAUGHTY_ATTRIBUTES
using NaughtyAttributes;
#endif
using System.Collections.Generic;
using UnityEngine;

namespace Bipolar.InteractionSystem
{
    public class InteractorBehavior : MonoBehaviour
    {

    }

    public class Interactor : MonoBehaviour
    {
        public delegate void InteractiveObjectChangeEventHandler(InteractiveObject oldObject, InteractiveObject newObject);
        public delegate void InteractionEventHandler(InteractiveObject interactiveObject, Interaction interaction);

        public event InteractiveObjectChangeEventHandler OnInteractiveObjectChanged;
        public event InteractionEventHandler OnInteracted;

        [SerializeField]
        private List<InteractorBehavior> additionalBehaviors;
        public List<InteractorBehavior> AdditionalBehaviors => additionalBehaviors;

        [SerializeField]
#if NAUGHTY_ATTRIBUTES
        [ReadOnly]
#endif
        private InteractiveObject currentInteractiveObject;
        public InteractiveObject CurrentInteractiveObject
        {
            get => currentInteractiveObject;
            set
            {
                if (currentInteractiveObject == value)
                    return;

                var oldInteractiveObject = currentInteractiveObject;
                currentInteractiveObject = value;
                OnInteractiveObjectChanged?.Invoke(oldInteractiveObject, value);
            }
        }

        public void CheckInteractions()
        {
            if (CurrentInteractiveObject && CurrentInteractiveObject.isActiveAndEnabled)
                if (CurrentInteractiveObject.TryInteract(this, out var interaction))
                    OnInteracted?.Invoke(CurrentInteractiveObject, interaction);
        }

        public bool TryGetAdditionalBehavior<T>(out T additionalBehavior) where T : InteractorBehavior
        {
            foreach (var behavior in additionalBehaviors)
            {
                if (behavior.isActiveAndEnabled && behavior is T correct)
                {
                    additionalBehavior = correct;
                    return true;
                }
            }
            additionalBehavior = null;
            return false;
        }
    }
}
