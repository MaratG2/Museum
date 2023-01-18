using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Отвечает за изменение размера предварительного просмотра плана 2D музея.
/// </summary>
public class MuseumPreviewSize : MonoBehaviour
{
    [SerializeField] private TMP_InputField _inputSizeX;
    [SerializeField] private TMP_InputField _inputSizeZ;

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
        Vector2 windowSize = new Vector2(Screen.currentResolution.width, Screen.currentResolution.height);
        int sizeX = 0, sizeZ;
        bool isX = Int32.TryParse(_inputSizeX.text, out sizeX);
        bool isZ = Int32.TryParse(_inputSizeZ.text, out sizeZ);
        
        if (!isX || !isZ)
            return;
        
        float heightScale = windowSize.y * 0.85f / sizeZ;
        float widthScale = windowSize.x * 0.25f / sizeX;
        if(heightScale < widthScale)
            _rt.sizeDelta = new Vector2(sizeX * heightScale, sizeZ * heightScale);
        else
            _rt.sizeDelta = new Vector2(sizeX * widthScale, sizeZ * widthScale);

        _image.material.SetTextureScale("_MainTex", new Vector2(sizeX, sizeZ));
    }
}
