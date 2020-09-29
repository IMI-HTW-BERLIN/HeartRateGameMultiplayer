using System;
using UnityEngine;

namespace Util
{
    [RequireComponent(typeof(BoxCollider2D))]
    public class UIScalerBoxCollider2D : MonoBehaviour
    {
        private BoxCollider2D _boxCollider2D;
        private void Awake()
        {
            _boxCollider2D = GetComponent<BoxCollider2D>();
            ScaleBoxCollider();
        }

        public void ScaleBoxCollider()
        {
            if (_boxCollider2D == null)
                _boxCollider2D = GetComponent<BoxCollider2D>();
            RectTransform rectTransform = (RectTransform) transform;
            _boxCollider2D.size = rectTransform.rect.size;
            _boxCollider2D.offset = (new Vector2(0.5f, 0.5f) - rectTransform.pivot) * rectTransform.rect.size;
        }
    }
}