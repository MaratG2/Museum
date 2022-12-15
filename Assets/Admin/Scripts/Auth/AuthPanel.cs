using System;
using System.Collections;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using Admin.Utility;
using MaratG2.Extensions;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace Admin.Auth
{
    public class AuthPanel : MonoBehaviour
    {
        [SerializeField] private bool _isAuthEnabled = true;
        private PanelChanger _panelChanger;

        private void Awake()
        {
            _panelChanger = FindObjectOfType<PanelChanger>();
            Screen.fullScreen = true;
        }

        private void Start()
        {
            if (_isAuthEnabled)
                _panelChanger.MoveToCanvasPanel(Panel.Auth);
            else
                _panelChanger.MoveToCanvasPanel(Panel.View);
        }
    }
}