using System.Collections.Generic;
using UnityEngine;

namespace Bipolar.InteractionSystem
{
    [DisallowMultipleComponent]
    public class InteractiveObject : MonoBehaviour
    {
        [SerializeField]
        private Interaction[] interactions;
        public IReadOnlyList<Interaction> Interactions => interactions;

        public bool TryInteract(Interactor interactor, out Interaction interactedInteraction)
        {
            interactedInteraction = null;
            foreach (var interaction in Interactions)
            {
                if (CheckInteraction(interaction, interactor))
                {
                    interaction.Interact(interactor);
                    interactedInteraction = interaction;
                    return true;
                }
            }
            return false;
        }

        private bool CheckInteraction(Interaction interaction, Interactor interactor)
        {
            if (interaction == null)
                return false;

            return CheckTrigger(interaction) && Interaction.CanInteract(interaction, interactor);
        }

        private static bool CheckTrigger(Interaction interaction)
        {
            return interaction.CheckTrigger();
        }
    }
}
