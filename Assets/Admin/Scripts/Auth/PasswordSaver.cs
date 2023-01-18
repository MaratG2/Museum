using Admin.Utility;
using UnityEngine;
using UnityEngine.UI;

namespace Admin.Auth
{
    /// <summary>
    /// Занимается логикой сохранения или удаления пароля, если отмечена галочка «Запомнить пароль».
    /// </summary>
    /// <remarks>
    /// Использует PlayerPrefs для сохранения (максимальный вес сохраняемых данных – 1МБ, сохраняются они в IndexedDB браузера).
    /// </remarks>
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