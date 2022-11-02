using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tile : MonoBehaviour
{
    public AdminEditMode.HallContent hallContent;
    private Image _image;
    [SerializeField] private Color32 _doorColor, _frameColor, _infoColor, _rubberColor;
    
    public void Setup()
    {
        _image = GetComponent<Image>();
        SelectTool(Int32.Parse(hallContent.type));
    }
    
    private void SelectTool(int tool)
    {
        switch (tool)
        {
            case -1:
                _image.color = Color.clear;
                break;
            case 0:
                _image.color = _doorColor;
                break;
            case 1:
                _image.color = _frameColor;
                break;
            case 2:
                _image.color = _infoColor;
                break;
            case 7:
                _image.color = Color.clear;
                break;
            case 8:
                _image.color = _rubberColor;
                break;
        }
    }
}
