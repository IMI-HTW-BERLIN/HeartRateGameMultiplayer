using System;
using Managers;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;
using Util;

namespace PlayerBehaviour
{
    public class Player : MonoBehaviour
    {
        [SerializeField] private int playerIndex;
        [SerializeField] private float movementSpeed;
        [SerializeField] private PlayerInput playerInput;
        [SerializeField] private RectTransform parentRectTransform;
        [SerializeField] private float hitDirectionChangeMax;
        [SerializeField] private UIScalerBoxCollider2D uiScaler;

        private float _startHeight;

        /// <summary>
        /// The index of the player.
        /// </summary>
        public int PlayerIndex => playerIndex;

        /// <summary>
        /// The movement input.
        /// </summary>
        private float _movement;

        private void Awake() => InputUser.PerformPairingWithDevice(Keyboard.current, playerInput.user);

        private void Start()
        {
            _startHeight = ((RectTransform) transform).sizeDelta.y;
        }

        /// <summary>
        /// If <see cref="GameManager.SizeChange"/> is enabled, will update height depending on heart rate.
        /// </summary>
        private void Update()
        {
            if (!GameManager.Instance.SizeChange)
                return;

            RectTransform rectTransform = (RectTransform) transform;
            rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x,
                _startHeight * GameManager.Instance.GetSizeChange(playerIndex));
            
            uiScaler.ScaleBoxCollider();
        }

        /// <summary>
        /// Moves the player's pong-bat by changing the anchored position of the rect transform.
        /// </summary>
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
        /// Returns a value between -1 and 1 depending on the given vertical position.
        /// Highest yPosition on the pong-bat -> 1. 
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