using UnityEngine;
using UnityEngine.EventSystems;

namespace Gino.PrototypeKit
{
    /// <summary>
    /// Attach to a UI Image/Button. Set direction and assign the character target.
    /// Holds the direction while the button is pressed, releases on pointer up.
    /// </summary>
    [RequireComponent(typeof(UnityEngine.UI.Image))]
    public class VirtualButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
    {
        [Tooltip("Direction this button represents (e.g. Vector2.up for Up button)")]
        public Vector2 direction;

        // Assign one of these in Inspector, or leave both empty to auto-find
        [Tooltip("Assign if using DirectionalCharacterController")]
        public DirectionalCharacterController directionalCharacter;

        [Tooltip("Assign if using TopDownController")]
        public TopDownController topDownCharacter;

        void Start()
        {
            if (directionalCharacter == null && topDownCharacter == null)
            {
                directionalCharacter = FindFirstObjectByType<DirectionalCharacterController>();
                if (directionalCharacter == null)
                    topDownCharacter = FindFirstObjectByType<TopDownController>();
            }
        }

        public void OnPointerDown(PointerEventData _)
        {
            directionalCharacter?.PressVirtualButton(direction);
            topDownCharacter?.PressVirtualButton(direction);
        }

        public void OnPointerUp(PointerEventData _)
        {
            directionalCharacter?.ReleaseVirtualButton(direction);
            topDownCharacter?.ReleaseVirtualButton(direction);
        }

        public void OnPointerExit(PointerEventData _)
        {
            directionalCharacter?.ReleaseVirtualButton(direction);
            topDownCharacter?.ReleaseVirtualButton(direction);
        }
    }
}
