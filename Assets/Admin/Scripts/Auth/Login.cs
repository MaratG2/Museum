using System;
using System.Collections;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using Admin.PHP;
using Admin.UsersManagement;
using Admin.Utility;
using TMPro;
using UnityEngine;

namespace Admin.Auth
{
    public class Login : MonoBehaviour
    {
        private LoginFieldsProvider _loginFields;
        private Action<string> _responseCallback;
        private string _responseText = "";
        private bool _canLogin = true;
        private QueriesToPHP _queriesToPhp = new(isDebugOn: true);
        private ILoggable _loggerUI;
        private PasswordSaver _passwordSaver;
        private PanelChanger _panelChanger;
        private Registration _registration;

        private User _currentUser;

        public User CurrentUser
        {
            get { return _currentUser; }
            set { _currentUser = value; }
        }

        private void OnEnable()
        {
            _responseCallback += response => _responseText = response;
        }

        private void OnDisable()
        {
            _responseCallback -= response => _responseText = response;
        }

        private void Awake()
        {
            _loggerUI = GetComponent<ILoggable>();
            _passwordSaver = GetComponent<PasswordSaver>();
            _registration = GetComponent<Registration>();
            _loginFields = GetComponent<LoginFieldsProvider>();
            _panelChanger = FindObjectOfType<PanelChanger>();
            _loginFields.EmailField.text = PlayerPrefs.HasKey("SavedEmail") ? PlayerPrefs.GetString("SavedEmail") : "";
        }

        public void TryLogin()
        {
            if (_canLogin)
            {
                _loginFields.Trim();
                if (IsLoginInfoWrong())
                    return;

                StartCoroutine(TryLoginCoroutine());
            }

            _canLogin = false;
        }

        private IEnumerator TryLoginCoroutine()
        {
            yield return _registration.GetEmailMatchedQuantity(_loginFields.EmailField.text);
            if (Int32.TryParse(_responseText, out int emailMatchedQuantity))
                if (emailMatchedQuantity == 0)
                {
                    _loggerUI.LogBad(_loginFields.ErrorText,
                        "Пользователя с таким адресом электронной почты не существует");
                    _canLogin = true;
                    yield break;
                }

            string securedPassword = Convert.ToBase64String(
                new SHA256CryptoServiceProvider().ComputeHash(Encoding.UTF8.GetBytes(_loginFields.PasswordField.text)));
            if (PlayerPrefs.HasKey("SavedPassword") && _loginFields.PasswordField.text == "")
                securedPassword = PlayerPrefs.GetString("SavedPassword");

            yield return LoginUser(securedPassword);
            CurrentUser = ParseUser();
            if (!string.IsNullOrEmpty(CurrentUser.email))
            {
                if (CurrentUser.access_level == AccessLevel.Registered)
                    _loggerUI.LogBad(_loginFields.ErrorText,
                        "Пользователь не активирован администратором музея. Вход запрещен");
                else
                {
                    _passwordSaver.SavePassword(CurrentUser);
                    _loginFields.Empty();
                    _panelChanger.MoveToCanvasPanel(Panel.View);
                }
            }

            _canLogin = true;
        }

        private IEnumerator LoginUser(string securedPassword)
        {
            _responseText = "";
            string phpFileName = "login_full.php";
            WWWForm data = new WWWForm();
            data.AddField("email", _loginFields.EmailField.text);
            data.AddField("pass", securedPassword);
            yield return _queriesToPhp.PostRequest(phpFileName, data, _responseCallback);
        }

        private User ParseUser()
        {
            User tempUser = new User();
            if (_responseText.Length == 0)
                return tempUser;

            string firstWord = _responseText.Split(' ')[0];
            if (firstWord == "<br")
            {
                _loggerUI.LogBad(_loginFields.ErrorText, "Неправильный пароль");
                return tempUser;
            }

            if (firstWord == "Query")
            {
                _loggerUI.LogBad(_loginFields.ErrorText, "Непредвиденная ошибка");
                return tempUser;
            }

            var datas = _responseText.Split('|');
            int uid = Int32.Parse(datas[0]);
            string name = datas[1];
            string email = datas[2];
            string password = datas[3];
            int access_level = Int32.Parse(datas[4]);
            tempUser.uid = uid;
            tempUser.name = name;
            tempUser.email = email;
            tempUser.password = password;
            tempUser.access_level = (AccessLevel)access_level;

            return tempUser;
        }

        private bool IsLoginInfoWrong()
        {
            if (_loginFields.EmailField.text == "")
            {
                _loggerUI.LogBad(_loginFields.ErrorText, "Адрес почты не может быть пустым");
                return true;
            }

            if (_loginFields.PasswordField.text.Length is < 8 or > 24 && !PlayerPrefs.HasKey("SavedPassword"))
            {
                _loggerUI.LogBad(_loginFields.ErrorText,
                    "Пароль не может быть меньше 8 или больше 24 символов");
                return true;
            }

            Regex regex = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");
            Match match = regex.Match(_loginFields.EmailField.text);
            if (!match.Success)
            {
                _loggerUI.LogBad(_loginFields.ErrorText,
                    "Адрес электронной почты не соответствует правилам ввода");
                return true;
            }

            return false;
        }
    }
}