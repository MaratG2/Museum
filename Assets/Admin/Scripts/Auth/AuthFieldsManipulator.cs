using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AuthFieldsManipulator : MonoBehaviour
{
    [SerializeField] private TMP_InputField _nameReg;
    [SerializeField] private TMP_InputField _emailReg;
    [SerializeField] private TMP_InputField _passwordReg;
    [SerializeField] private TMP_InputField _emailAuth;
    [SerializeField] private TMP_InputField _passwordAuth;
    [SerializeField] private Toggle _toggleSavePassword;
    
    public void GetSavedPassword()
    {
        if (PlayerPrefs.HasKey("SavedPassword"))
        {
            _emailAuth.text = PlayerPrefs.GetString("SavedEmail");
            _toggleSavePassword.isOn = true;
        }
        else
        {
            _toggleSavePassword.isOn = false;
        }
    }
    public void EmptyAuthFields()
    {
        _emailAuth.text = "";
        _passwordAuth.text = "";
        _nameReg.text = "";
        _emailReg.text = "";
        _passwordReg.text = "";
    }
    public void TrimAuthFields()
    {
        _emailAuth.text.Trim();
        _passwordAuth.text.Trim();
        _nameReg.text.Trim();
        _emailReg.text.Trim();
        _passwordReg.text.Trim();
    }
    public void SavePassword(User user)
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

    public void MessageThrowUI(TextMeshProUGUI textUI, string message, bool isGood)
    {
        textUI.text = message;
        textUI.color = isGood ? Color.green : Color.red;
    }
}
