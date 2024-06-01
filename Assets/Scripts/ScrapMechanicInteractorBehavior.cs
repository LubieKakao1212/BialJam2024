using Bipolar.InteractionSystem;
using NaughtyAttributes;
using UnityEngine;

public class ScrapMechanicInteractorBehavior : InteractorBehavior
{
    [SerializeField, ReadOnly]
    private int scrapCollected;

    public bool HasScrap => scrapCollected > 0;

    public void AddScrap()
    {
        scrapCollected++;
    }

    public void RemoveScrap()
    {
        scrapCollected--;
    }

}
