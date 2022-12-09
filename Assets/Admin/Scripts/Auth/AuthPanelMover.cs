using System.Collections;
using System.Collections.Generic;
using MaratG2.Extensions;
using UnityEngine;

public class AuthPanelMover : MonoBehaviour
{
    [SerializeField] private CanvasGroup _authCGroup;
    [SerializeField] private CanvasGroup _viewCGroup;
    [SerializeField] private CanvasGroup _editCGroup;
    [SerializeField] private CanvasGroup _newCGroup;
    
    public void MoveToAuthScreen()
    {
        _authCGroup.SetActive(true);
        _viewCGroup.SetActive(false);
        _editCGroup.SetActive(false);
        _newCGroup.SetActive(false);
    }
    public void MoveToViewScreen()
    {
        _authCGroup.SetActive(false);
        _viewCGroup.SetActive(true);
        _editCGroup.SetActive(false);
        _newCGroup.SetActive(false);
    }
}
