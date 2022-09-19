using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class LevelCell : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    private Image _cellImage;
    private int _data = 0;
    
    private void Awake()
    {
        _cellImage = GetComponent<Image>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if(_data == 0)
            _cellImage.color = Color.red;
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        if(_data == 0)
            _cellImage.color = Color.white;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (_data != 0)
            return;
        
        _data = 1; // = tile drawn
        _cellImage.color = Color.yellow;
    }
}
