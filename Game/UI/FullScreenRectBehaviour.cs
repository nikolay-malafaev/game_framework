using UnityEngine;

namespace GameFramework.UI
{
    [RequireComponent(typeof(RectTransform))]
    public class FullScreenRectBehaviour : MonoBehaviour
    {
        [SerializeField] private bool _useSafeArea;
        
        private void Start()
        {
            if (_useSafeArea)
            {
                var rectTransform = GetComponent<RectTransform>();
                SetSafeArea(rectTransform);
            }
        }

        private void SetSafeArea(RectTransform rectTransform)
        {
            if (!rectTransform)
            {
                return;
            }
            
            var safeArea = Screen.safeArea;

            var width = Screen.width;
            var height = Screen.height;
            var canvasSize = rectTransform.rect.size;

            safeArea.x = safeArea.x * canvasSize.x / width;
            safeArea.y = safeArea.y * canvasSize.y / height;

            safeArea.width = safeArea.width * canvasSize.x / width;
            safeArea.height = safeArea.height * canvasSize.y / height;
            
            rectTransform.anchorMin = new Vector2(safeArea.x / canvasSize.x, safeArea.y / canvasSize.y);
            rectTransform.anchorMax = new Vector2((safeArea.x + safeArea.width) / canvasSize.x, (safeArea.y + safeArea.height) / canvasSize.y);

            rectTransform.offsetMin = Vector2.zero;
            rectTransform.offsetMax = Vector2.zero;
        }
    }
}