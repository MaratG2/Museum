using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AdminHallPreview : MonoBehaviour
{
    [SerializeField] private AdminViewMode _adminView;
    private RectTransform _rt;
    private Image _image;

    private void Awake()
    {
        _rt = GetComponent<RectTransform>();
        _image = GetComponent<Image>();
        _rt.sizeDelta = Vector2.zero;
    }

    void Update()
    {
        if (!_adminView || _adminView.HallSelected.sizex == 0 || _adminView.HallSelected.sizez == 0)
            return;
        
        Vector2 windowSize = new Vector2(Screen.currentResolution.width, Screen.currentResolution.height);
        int sizeX = _adminView.HallSelected.sizex, sizeZ = _adminView.HallSelected.sizez;
        float heightScale = (windowSize.y - 250f) / sizeZ;
        float widthScale = (windowSize.x - 1400f) / sizeX;
        if (heightScale < widthScale)
            _rt.sizeDelta = new Vector2(sizeX * heightScale, sizeZ * heightScale);
        else
            _rt.sizeDelta = new Vector2(sizeX * widthScale, sizeZ * widthScale);

        _image.material.SetTextureScale("_MainTex", new Vector2(sizeX, sizeZ));
    }
}
