using UnityEngine;

namespace Util
{
    [ExecuteInEditMode]
    public class ScreenTransform : MonoBehaviour
    {
        [SerializeField] private Vector2 screenScale = new Vector2(1, 1);
        [SerializeField] private Vector2 screenAnchor = new Vector2(0.5f, 0.5f);
        [SerializeField] private Vector2 pivot = new Vector2(0.5f, 0.5f);
        [SerializeField] private Camera mainCamera;

        private Transform _transform;

        public void ManualUpdate()
        {
            if(mainCamera == null)
                return;

            float cameraSizeFactor = mainCamera.orthographicSize * 2f;
            
            float width = (float) Screen.width / Screen.height * cameraSizeFactor;
            float height = cameraSizeFactor;

            _transform = transform;
            _transform.localScale = new Vector3(screenScale.x * width, screenScale.y * height);
            _transform.position = new Vector3(
                -width / 2f + width * screenAnchor.x + (0.5f - pivot.x) * _transform.localScale.x,
                -height / 2f + height * screenAnchor.y + (0.5f - pivot.y) * _transform.localScale.y);
        }

        public void ForceUpdate()
        {
            foreach (Behaviour behaviour in GetComponents<Behaviour>())
            {
                behaviour.enabled = false;
                behaviour.enabled = true;
            }
        }
    }
}