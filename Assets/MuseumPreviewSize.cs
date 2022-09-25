using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MuseumPreviewSize : MonoBehaviour
{
    [SerializeField] private TMP_InputField _inputSizeX;
    [SerializeField] private TMP_InputField _inputSizeZ;

    private RectTransform _rt;

    private void Awake()
    {
        _rt = GetComponent<RectTransform>();
    }

    void Update()
    {
        Vector2 windowSize = new Vector2(Screen.currentResolution.width, Screen.currentResolution.height);
        int sizeX = 0, sizeZ;
        bool isX = Int32.TryParse(_inputSizeX.text, out sizeX);
        bool isZ = Int32.TryParse(_inputSizeZ.text, out sizeZ);
        if (!isX)
        {
            //_inputSizeX.text = 8.ToString();
            return;
        }
        if (!isZ)
        {
            //_inputSizeZ.text = 40.ToString();
            return;
        }
        
        float heightScale = (windowSize.y - 250f) / sizeZ;
        float widthScale = (windowSize.x - 1400f) / sizeX;
        if(heightScale < widthScale)
            _rt.sizeDelta = new Vector2(sizeX * heightScale, sizeZ * heightScale);
        else
            _rt.sizeDelta = new Vector2(sizeX * widthScale, sizeZ * widthScale);
    }
}
