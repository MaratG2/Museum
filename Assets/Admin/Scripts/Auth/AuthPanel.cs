using System;
using System.Collections;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using MaratG2.Extensions;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace Admin.Utility
{
    public class AuthPanel : MonoBehaviour
    {
        [SerializeField] private bool _isAuthEnabled = true;
        private PanelChanger _panelChanger;
        private AuthFieldsManipulator _authFieldsManipulator;

        private void Awake()
        {
            _panelChanger = FindObjectOfType<PanelChanger>();
            _authFieldsManipulator = GetComponent<AuthFieldsManipulator>();
        }

        private void Start()
        {
            if (_isAuthEnabled)
            {
                _panelChanger.MoveToCanvasPanel(Panel.Auth);
                _authFieldsManipulator.GetSavedPassword();
            }
            else
                _panelChanger.MoveToCanvasPanel(Panel.View);
        }
    }
}