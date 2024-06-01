using Bipolar.InteractionSystem;
using Bipolar.InteractionSystem.Hints;
using UnityEngine;
using UnityEngine.Events;

public class FixBillboardInteraction : Interaction
{
    [SerializeField]
    private UnityEvent onRepair;

    [SerializeField]
    private Hint hint;


    public override void Interact(Interactor interactor)
    {
        if (interactor.TryGetAdditionalBehavior<ScrapMechanicInteractorBehavior>(out var scrapMechanicInteractorBehavior))
        {
            if (scrapMechanicInteractorBehavior.HasScrap)
            {
                scrapMechanicInteractorBehavior.RemoveScrap();
                Debug.Log("Fixing the billboard");
                onRepair.Invoke();
                enabled = false;
                hint.Message = string.Empty;
            }
        }
    }

    public override bool CanInteract(in Interactor interactor)
    {
        bool canInteract = interactor.TryGetAdditionalBehavior<ScrapMechanicInteractorBehavior>(out var scrapMechanic) && scrapMechanic.HasScrap;
        hint.Message = canInteract ? "Press E to repair billboard" : "Cannot repair billboard without scrap";
        return canInteract;
    }
}

