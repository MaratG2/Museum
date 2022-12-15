using TMPro;
using UnityEngine;

namespace Admin.Auth
{
    public class LoginFieldsProvider : MonoBehaviour, IFieldsProvider
    {
        [SerializeField] private TMP_InputField _emailField;
        [SerializeField] private TMP_InputField _passwordField;
        [SerializeField] private TextMeshProUGUI _errorText;
        public TMP_InputField EmailField => _emailField;
        public TMP_InputField PasswordField => _passwordField;
        public TextMeshProUGUI ErrorText => _errorText;

        public void Empty()
        {
            EmailField.text = "";
            PasswordField.text = "";
            ErrorText.text = "";
        }

        public void Trim()
        {
            EmailField.text.Trim();
            PasswordField.text.Trim();
            ErrorText.text.Trim();
        }
    }
}