using System.Collections;
using System.Collections.Generic;
using Admin.Utility;
using UnityEngine;
using UnityEngine.UI;

namespace Admin.Auth
{
    public class PasswordSaver : MonoBehaviour
    {
        [SerializeField] private Toggle _toggleSavePassword;

        public void Awake()
        {
            _toggleSavePassword.isOn = PlayerPrefs.HasKey("SavedPassword");
        }

        public void SaveOrDeletePassword(User user)
        {
            if (_toggleSavePassword.isOn)
            {
                PlayerPrefs.SetString("SavedPassword", user.password);
                PlayerPrefs.SetString("SavedEmail", user.email);
            }
            else
            {
                PlayerPrefs.DeleteKey("SavedPassword");
                PlayerPrefs.DeleteKey("SavedEmail");
            }
        }

        public void SavePasswordToggleChanged(bool setTo)
        {
            if (!setTo)
            {
                PlayerPrefs.DeleteKey("SavedPassword");
                PlayerPrefs.DeleteKey("SavedEmail");
            }
        }
    }
}