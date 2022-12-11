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

namespace Admin.Auth
{
    public class AuthPanel : MonoBehaviour
    {
        [SerializeField] private bool _isAuthEnabled = true;
        private AuthPanelMover _authPanelMover;
        private AuthFieldsManipulator _authFieldsManipulator;

        private void Awake()
        {
            _authPanelMover = GetComponent<AuthPanelMover>();
            _authFieldsManipulator = GetComponent<AuthFieldsManipulator>();
        }

        private void Start()
        {
            if (_isAuthEnabled)
            {
                _authPanelMover.MoveToAuthScreen();
                _authFieldsManipulator.GetSavedPassword();
            }
            else
                _authPanelMover.MoveToViewScreen();
        }
    }
}