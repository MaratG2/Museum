using System;
using System.Collections;
using System.Security.Cryptography;
using System.Text;
using Admin.Auth;
using Admin.PHP;
using Admin.Utility;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Admin.UsersManagement
{
    /// <summary>
    /// Отвечает за представление пользователя в интерфейсе – отображение его полей, сохранение изменений у пользователя в базе данных и удаление пользователя.
    /// </summary>
    public class UserContent : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _nameText;
        [SerializeField] private TextMeshProUGUI _emailText;
        [SerializeField] private TMP_InputField _passwordInput;
        [SerializeField] private TMP_Dropdown _roleDropdown;
        [SerializeField] private Button _deleteUserButton;
        [SerializeField] private Button _saveUserButton;
        private User _user;
        private QueriesToPHP _queriesToPhp = new(isDebugOn: true);
        private Action<string> OnResponseCallback;
        private string _response;

        public void Initialize(User user)
        {
            this._user = user;
            if (user.access_level == AccessLevel.Administrator)
                _deleteUserButton.interactable = false;
            if(user.email == FindObjectOfType<Login>().CurrentUser.email)
                _roleDropdown.interactable = false;
            
            _deleteUserButton.onClick.AddListener(DeleteUserFromButton);
            _saveUserButton.onClick.AddListener(SaveUserFromButton);
            _nameText.text = _user.name;
            _emailText.text = _user.email;
            _roleDropdown.value = (int)_user.access_level;
        }

        private void OnEnable()
        {
            OnResponseCallback += response => _response = response;
        }

        private void OnDisable()
        {
            OnResponseCallback -= response => _response = response;
        }

        public void DeleteUserFromButton()
        {
            StartCoroutine(DeleteUserQuery());
        }

        private IEnumerator DeleteUserQuery()
        {
            string phpFileName = "delete_user.php";
            WWWForm data = new WWWForm();
            data.AddField("email", _user.email);
            yield return _queriesToPhp.PostRequest(phpFileName, data, OnResponseCallback);
            if (_response == "Query completed")
                FindObjectOfType<UsersParser>().ParseUsers();
            else
                Debug.LogError("Delete user query: " + _response);
        }

        public void SaveUserFromButton()
        {
            SaveDataToUser();
            StartCoroutine(UpdateUserQuery());
        }

        private void SaveDataToUser()
        {
            if(!string.IsNullOrWhiteSpace(_passwordInput.text))
            {
                string securedPassword = Convert.ToBase64String(
                    new SHA256CryptoServiceProvider().ComputeHash(Encoding.UTF8.GetBytes(_passwordInput.text)));
                _user.password = securedPassword;
            }
            _user.access_level = (AccessLevel)_roleDropdown.value;
        }

        private IEnumerator UpdateUserQuery()
        {
            string phpFileName = "update_user.php";
            WWWForm data = new WWWForm();
            data.AddField("name", _user.name);
            data.AddField("email", _user.email);
            data.AddField("password", _user.password);
            data.AddField("access_level", (int)_user.access_level);
            yield return _queriesToPhp.PostRequest(phpFileName, data, OnResponseCallback);
            if (_response != "Query completed")
                Debug.LogError("Update user query: " + _response);
        }
    }
}