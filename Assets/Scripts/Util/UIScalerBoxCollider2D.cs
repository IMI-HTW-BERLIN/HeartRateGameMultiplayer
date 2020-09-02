using System;
using UnityEngine;

namespace Util
{
    [RequireComponent(typeof(BoxCollider2D))]
    public class UIScalerBoxCollider2D : MonoBehaviour
    {
        private void Awake() => ScaleBoxCollider();

        public void ScaleBoxCollider()
        {
            BoxCollider2D boxCollider2D = GetComponent<BoxCollider2D>();
            RectTransform rectTransform = (RectTransform) transform;
            boxCollider2D.size = rectTransform.rect.size;
            boxCollider2D.offset = (new Vector2(0.5f, 0.5f) - rectTransform.pivot) * rectTransform.rect.size;
        }
    }
}