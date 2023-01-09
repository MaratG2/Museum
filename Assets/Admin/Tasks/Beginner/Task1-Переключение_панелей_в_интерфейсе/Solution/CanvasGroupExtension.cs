using UnityEngine;

namespace Task1
{
    public static class CanvasGroupExtension
    {
        public static void SetActive(this CanvasGroup canvasGroup, bool setTo)
        {
            canvasGroup.alpha = setTo ? 1f : 0f;
            canvasGroup.interactable = setTo;
            canvasGroup.blocksRaycasts = setTo;
        }
    }
}