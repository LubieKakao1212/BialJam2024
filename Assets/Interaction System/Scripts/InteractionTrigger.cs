using UnityEngine;

namespace Bipolar.InteractionSystem
{
    public abstract class InteractionTrigger : ScriptableObject
    {
        public abstract bool Check();
    }
}
