using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;

namespace PlayerBehaviour
{
    public class Player : MonoBehaviour
    {
        [SerializeField] private float movementSpeed;
        [SerializeField] private PlayerInput playerInput;
        [SerializeField] private RectTransform parentRectTransform;
        [SerializeField] private float hitDirectionChangeMax;

        private float _movement;

        private void Awake()
        {
            InputUser.PerformPairingWithDevice(Keyboard.current, playerInput.user);
        }

        private void FixedUpdate()
        {
            transform.position += new Vector3(0, _movement * movementSpeed * Time.deltaTime);
            RectTransform rectTransform = (RectTransform) transform;
            float height = rectTransform.rect.height / 2f;
            float parentHeight = parentRectTransform.rect.height / 2f;
            rectTransform.anchoredPosition = new Vector2(rectTransform.anchoredPosition.x,
                Mathf.Clamp(rectTransform.anchoredPosition.y, -parentHeight + height, parentHeight - height));
        }

        public void OnMove(InputAction.CallbackContext value) => _movement = value.ReadValue<float>();

        /// <summary>
        /// Returns a value between -1 and 1 depending on the given value. 
        /// </summary>
        /// <param name="yPosition">The vertical position of the hit.</param>
        /// <returns>Value between -1 and 1.</returns>
        public float GetHitDelta(float yPosition)
        {
            RectTransform rectTransform = (RectTransform) transform;
            float height = rectTransform.rect.height;

            return Mathf.Clamp((yPosition - transform.position.y) / height, -hitDirectionChangeMax,
                hitDirectionChangeMax);
        }
    }
}