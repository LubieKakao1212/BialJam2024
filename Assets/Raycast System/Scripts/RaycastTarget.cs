using System;
using UnityEngine;
using UnityEngine.Events;

namespace Bipolar.RaycastSystem
{
    public class RaycastTarget : MonoBehaviour
    {
        public event Action OnRayEnter;
        public event Action OnRayStay;
        public event Action OnRayExit;

        [SerializeField]
        private UnityEvent onRayEnter;
        [SerializeField]
        private UnityEvent onRayExit;

        private void Start() { }

        public void RayEnter()
        { 
            onRayEnter.Invoke();
            OnRayEnter?.Invoke();
        }

        public void RayStay()
        {
            OnRayStay?.Invoke();
        }

        public void RayExit()
        {
            onRayExit.Invoke();
            OnRayExit?.Invoke();
        }
    }
}
