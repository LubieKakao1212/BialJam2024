using UnityEngine;

namespace Bipolar.InteractionSystem
{
    [RequireComponent(typeof(Interactor))]
    public class InteractorController : MonoBehaviour
    {
        private Interactor _interactor;
        public Interactor Interactor
        {
            get
            {
                if (_interactor == null)
                    TryGetComponent(out _interactor);
                return _interactor;
            }
        }

        protected virtual void Update()
        {
            Interactor.CheckInteractions();
        }
    }
}
