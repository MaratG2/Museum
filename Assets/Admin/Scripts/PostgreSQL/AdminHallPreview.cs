using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class AdminHallPreview : MonoBehaviour
{
    [SerializeField] private AdminViewMode _adminView;
    private RectTransform _rt;
    private BoxCollider2D _collider2D;
    private Image _image;

    private void Awake()
    {
        _rt = GetComponent<RectTransform>();
        _image = GetComponent<Image>();
        _collider2D = GetComponent<BoxCollider2D>();
        _rt.sizeDelta = Vector2.zero;
    }

    void Update()
    {
        if (!_adminView || _adminView.HallSelected.sizex == 0 || _adminView.HallSelected.sizez == 0)
        {
            _rt.sizeDelta = Vector2.zero;
            if(_collider2D)
                _collider2D.size = _rt.sizeDelta;
            return;
        }
        
        Vector2 windowSize = new Vector2(Screen.currentResolution.width, Screen.currentResolution.height);
        int sizeX = _adminView.HallSelected.sizex, sizeZ = _adminView.HallSelected.sizez;
        float heightScale = windowSize.y * 0.85f / sizeZ;
        float widthScale = windowSize.x * 0.25f / sizeX;
        
        float tileSize = _image.rectTransform.sizeDelta.x / _adminView.HallSelected.sizex;
        
        float addPosX = 0, addPosY = tileSize / 4;
        if(_adminView.HallSelected.sizez % 2 == 0)
            addPosY = -tileSize / 4;
        if (_adminView.HallSelected.sizex % 2 != 0)
            addPosX = tileSize / 2;

        _image.rectTransform.anchoredPosition = new Vector2
        (
            Mathf.FloorToInt((0.35f) * (windowSize.x / tileSize)) * tileSize + addPosX,
            Mathf.FloorToInt((0.55f) * (windowSize.y / tileSize)) * tileSize + addPosY
        );
        
        if (heightScale < widthScale)
            _rt.sizeDelta = new Vector2(sizeX * heightScale, sizeZ * heightScale);
        else
            _rt.sizeDelta = new Vector2(sizeX * widthScale, sizeZ * widthScale);

        _image.material.SetTextureScale("_MainTex", new Vector2(sizeX, sizeZ));
        if(_collider2D)
            _collider2D.size = _rt.sizeDelta;
    }
    
    public static class RaycastUtilities
    {
        public static bool PointerIsOverUI(Vector2 screenPos)
        {
            var hitObject = UIRaycast(ScreenPosToPointerData(screenPos));
            return hitObject != null && hitObject.layer == LayerMask.NameToLayer("UI");
        }
 
        public static GameObject UIRaycast (PointerEventData pointerData)
        {
            var results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(pointerData, results);
 
            return results.Count < 1 ? null : results[0].gameObject;
        }
        public static GameObject[] UIRaycasts (PointerEventData pointerData)
        {
            var results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(pointerData, results);
            List<GameObject> gos = new List<GameObject>();
            foreach (var r in results)
                gos.Add(r.gameObject);
            return gos.ToArray();
        }
 
        public static PointerEventData ScreenPosToPointerData (Vector2 screenPos)
            => new(EventSystem.current){position = screenPos};
    }
}


