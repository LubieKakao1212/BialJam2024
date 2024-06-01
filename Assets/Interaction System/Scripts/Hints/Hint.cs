using UnityEngine;

namespace Bipolar.InteractionSystem.Hints
{
    public interface IHint
    {
        event System.Action<Hint> OnHintChanged;
        string Message { get; set; }
    }

    public class Hint : MonoBehaviour, IHint
    {
        public virtual event System.Action<Hint> OnHintChanged;

        [SerializeField]
        private string message;
        public string Message
        {
            get => message;
            set
            {
                message = value;
                OnHintChanged?.Invoke(this);
            }
        }
    }
}
