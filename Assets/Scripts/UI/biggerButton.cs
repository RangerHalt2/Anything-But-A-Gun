using UnityEngine;
using UnityEngine.EventSystems;

public class biggerButton : MonoBehaviour
{

    
    
        private Vector3 originalScale;
        public float scaleFactor = 2f;
        void Start()
        {
            originalScale = transform.localScale;
        }

        public void OnPointerEnter()
        {
            transform.localScale = originalScale * scaleFactor;
        }

        public void OnPointerExit()
        {
            transform.localScale = originalScale;
        }
    
}

