using TMPro;
using UnityEngine;

namespace Admin.Auth
{
    /// <summary>
    /// Контракт с интерфейсом IFieldsProvider, дает в использование публичные методы для манипуляции с полями ввода текста, используемые при регистрации пользователя.
    /// </summary>
    public class RegistrationFieldsProvider : MonoBehaviour, IFieldsProvider
    {
        [SerializeField] private TMP_InputField _nameField;
        [SerializeField] private TMP_InputField _emailField;
        [SerializeField] private TMP_InputField _passwordField;
        [SerializeField] private TextMeshProUGUI _errorText;
        public TMP_InputField NameField => _nameField;
        public TMP_InputField EmailField => _emailField;
        public TMP_InputField PasswordField => _passwordField;
        public TextMeshProUGUI ErrorText => _errorText;
        
        public void Empty()
        {
            NameField.text = "";
            EmailField.text = "";
            PasswordField.text = "";
            ErrorText.text = "";
        }

        public void Trim()
        {
            NameField.text.Trim();
            EmailField.text.Trim();
            PasswordField.text.Trim();
            ErrorText.text.Trim();
        }
    }
}