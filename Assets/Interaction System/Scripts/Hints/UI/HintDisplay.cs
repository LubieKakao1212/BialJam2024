#if NAUGHTY_ATTRIBUTES
using NaughtyAttributes;
#endif
using UnityEngine;

namespace Bipolar.InteractionSystem.Hints.UI
{
    public abstract class HintDisplay : MonoBehaviour
    {
        [SerializeField]
#if NAUGHTY_ATTRIBUTES
        [ReadOnly]
#endif
        private Hint currentHint;
        public Hint CurrentHint
        {
            get => currentHint;
            set
            {
                if (currentHint)
                    currentHint.OnHintChanged -= Refresh;
                currentHint = value;
                if (currentHint)
                {
                    Refresh(currentHint);
                    currentHint.OnHintChanged += Refresh;
                }
                else
                {
                    Refresh(null);
                }
            }
        }

        private void OnEnable()
        {
            Refresh(currentHint);
            if (currentHint != null)
                currentHint.OnHintChanged += Refresh;
        }

        protected abstract void Refresh(Hint hint);

        private void OnDisable()
        {
            if (currentHint != null)
                currentHint.OnHintChanged -= Refresh;
        }
    }
}
