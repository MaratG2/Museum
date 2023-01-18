using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Admin.Utility
{
    /// <summary>
    /// Отвечает за получение элемента интерфейса, на который указывает курсор.
    /// </summary>
    public static class RaycastUtilities
    {
        public static bool PointerIsOverUI(Vector2 screenPos)
        {
            var hitObject = UIRaycast(ScreenPosToPointerData(screenPos));
            return hitObject != null && hitObject.layer == LayerMask.NameToLayer("UI");
        }

        public static GameObject UIRaycast(PointerEventData pointerData)
        {
            var results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(pointerData, results);

            return results.Count < 1 ? null : results[0].gameObject;
        }

        public static GameObject[] UIRaycasts(PointerEventData pointerData)
        {
            var results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(pointerData, results);
            List<GameObject> gos = new List<GameObject>();
            foreach (var r in results)
                gos.Add(r.gameObject);
            return gos.ToArray();
        }

        public static PointerEventData ScreenPosToPointerData(Vector2 screenPos)
            => new(EventSystem.current) { position = screenPos };
    }
}