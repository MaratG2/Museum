using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdminEditMode : MonoBehaviour
{
    [SerializeField] private RectTransform _cursorTile;

    void Update()
    {
        Vector2 windowSize = new Vector2(Screen.currentResolution.width, Screen.currentResolution.height);
        float mouseX = Input.mousePosition.x;
        if (mouseX < 0.75f * windowSize.x)
            _cursorTile.anchoredPosition = Input.mousePosition;
        else
            _cursorTile.anchoredPosition = -windowSize;
    }
}
