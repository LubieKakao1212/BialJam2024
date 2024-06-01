using Bipolar.InteractionSystem;
using UnityEngine;

public class CollectScrapInteraction : Interaction
{
    [SerializeField]
    private GameObject visual;

    private void Reset()
    {
        visual = GetComponentInChildren<Collider>()?.gameObject;
    }

    public override bool CanInteract(in Interactor interactor) => interactor.TryGetAdditionalBehavior<ScrapMechanicInteractorBehavior>(out _);

    public override void Interact(Interactor interactor)
    {
        if (interactor.TryGetAdditionalBehavior<ScrapMechanicInteractorBehavior>(out var scrapMechanicInteractorBehavior))
        {
            scrapMechanicInteractorBehavior.AddScrap();
            visual.SetActive(false);
        }
    }
}
