using System.Collections.Generic;
using UnityEngine;

namespace Bipolar.InteractionSystem.Hints.UI
{
    public class HintsUIController : MonoBehaviour
    {
        [SerializeField]
        private Interactor interactor;
        public Interactor Interactor
        {
            get => interactor;
            set
            {
                if (interactor != value)
                    SetInteractor(value);
            }
        }

        [SerializeField]
        private HintDisplay objectNameDisplay;
        public HintDisplay ObjectNameDisplay
        {
            get => objectNameDisplay;
            set => objectNameDisplay = value;
        }

        [SerializeField]
        private List<HintDisplay> hintDisplays;
        public List<HintDisplay> HintDisplays => hintDisplays;

        private void OnEnable()
        {
            if (interactor)
                interactor.OnInteractiveObjectChanged += Interactor_OnInteractiveObjectChanged;
        }

        private void Interactor_OnInteractiveObjectChanged(InteractiveObject oldObject, InteractiveObject newObject)
        {
            RefreshHints(newObject);
        }

        private void RefreshHints(InteractiveObject interactiveObject)
        {
            SetHintInDisplay(objectNameDisplay, null);
            foreach (var hintDisplay in hintDisplays)
                SetHintInDisplay(hintDisplay, null);

            if (interactiveObject == null)
                return;

            if (interactiveObject.TryGetComponent<Hint>(out var nameHint))
                SetHintInDisplay(objectNameDisplay, nameHint);

            var interactions = interactiveObject.Interactions;
            int displayIndex = 0;
            foreach (var interaction in interactions)
            {
                if (Interaction.CanInteract(interaction, interactor))
                {
                    if (displayIndex >= hintDisplays.Count)
                        break;

                    interaction.TryGetComponent<Hint>(out var hint);
                    SetHintInDisplay(hintDisplays[displayIndex], hint);
                    displayIndex++;
                }
            }
        }

        private void SetHintInDisplay(HintDisplay display, Hint hint)
        {
            if (display)
                display.CurrentHint = hint;
        }

        private void SetInteractor(Interactor value)
        {
            if (interactor)
                interactor.OnInteractiveObjectChanged -= Interactor_OnInteractiveObjectChanged;
            interactor = value;
            if (interactor)
                interactor.OnInteractiveObjectChanged += Interactor_OnInteractiveObjectChanged;
        }

        private void OnDisable()
        {
            if (interactor)
                interactor.OnInteractiveObjectChanged -= Interactor_OnInteractiveObjectChanged;
        }

        private void OnValidate()
        {
            if (Application.isPlaying)
                SetInteractor(interactor);
        }
    }
}
