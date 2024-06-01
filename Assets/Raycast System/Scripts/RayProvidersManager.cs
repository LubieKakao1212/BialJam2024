using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Bipolar.RaycastSystem
{
    public class RayProvidersManager : MonoBehaviour
    {
        [SerializeField]
        private RaycastController raycastController;
        public RaycastController RaycastController
        {
            get => raycastController;
            set
            {
                raycastController = value;
            }
        }

        public static void SetRayProvider(RaycastController controller, RayProvider rayProvider)
        {
            controller.RayProvider = rayProvider;
        }

        public void SetRayProvider(RayProvider rayProvider)
        {
            SetRayProvider(raycastController, rayProvider);
        }    

        public static void SetRayProvider<T>(RaycastController controller) where T : RayProvider
        {
            if (controller.TryGetComponent<T>(out var rayProvider) == false)
                rayProvider = controller.gameObject.AddComponent<T>();

            SetRayProvider(controller, rayProvider);
        }

        public void SetRayProvider<T>() where T : RayProvider
        {
            SetRayProvider<T>(raycastController);
        }
    }
}
