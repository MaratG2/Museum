using System;
using System.Collections;
using System.Collections.Generic;
using GenerationMap;
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
        SelectTool(hallContent.type);
    }
    
    private void SelectTool(int tool)
    {
        switch (tool)
        {
            case -2:
                _image.color = _rubberColor;
                break;
            case -1:
                _image.color = Color.clear;
                break;
        }
        if (tool == ExhibitsConstants.Picture.Id)
        {
            _image.color = _frameColor;
            return;
        }
        if (tool == ExhibitsConstants.SpawnPoint.Id)
        {
            _image.color = _doorColor;
            return;
        }
        if (tool == ExhibitsConstants.InfoBox.Id)
        {
            _image.color = _infoColor;
            return;
        }
    }
}
