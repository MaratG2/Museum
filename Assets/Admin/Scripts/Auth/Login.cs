using System;
using System.Collections;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using Admin.PHP;
using Admin.Utility;
using UnityEngine;

namespace Admin.Auth
{
    /// <summary>
    /// Занимается логикой авторизации пользователя.
    /// </summary>
    /// <remarks>
    /// Также получает пользователя из базы данных и запоминает его в публичное авто-свойство CurrentUser, которое используется множеством других классов.
    /// </remarks>
    public class Login : MonoBehaviour
    {
        private QueriesToPHP _queriesToPhp = new(isDebugOn: true);
        private Action<string> _responseCallback;
        private string _responseText = "";
        private LoginFieldsProvider _loginFields;
        private bool _canLogin = true;
        private PasswordSaver _passwordSaver;
        private PanelChanger _panelChanger;
        private Registration _registration;
        private ILoggable _loggerUI;
        public User CurrentUser { get; set; }

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

                StartCoroutine(LoginCR());
            }

            _canLogin = false;
        }

        private IEnumerator LoginCR()
        {
            yield return CheckIfUserNotRegistered();

            string securedPassword = GetUserPassword();
            yield return LoginUser(securedPassword);
            CurrentUser = ParseUser();
            
            _canLogin = true;
            if (string.IsNullOrEmpty(CurrentUser.email))
                yield break;
            if (CurrentUser.access_level == AccessLevel.Registered)
            {
                _loggerUI.LogBad(_loginFields.ErrorText,
                    "Пользователь не активирован администратором музея. Вход запрещен");
                yield break;
            }
            
            _passwordSaver.SaveOrDeletePassword(CurrentUser);
            _loginFields.Empty();
            _panelChanger.MoveToCanvasPanel(Panel.View);
        }

        private IEnumerator CheckIfUserNotRegistered()
        {
            yield return _registration.GetEmailMatchedQuantity(_loginFields.EmailField.text);
            if (Int32.TryParse(_responseText, out int emailMatchedQuantity))
                if (emailMatchedQuantity == 0)
                {
                    _loggerUI.LogBad(_loginFields.ErrorText,
                        "Пользователя с таким адресом электронной почты не существует");
                    _canLogin = true;
                    StopCoroutine(LoginCR());
                }
        }

        private string GetUserPassword()
        {
            string securedPassword = 
            Convert.ToBase64String(
                new SHA256CryptoServiceProvider().ComputeHash(Encoding.UTF8.GetBytes(_loginFields.PasswordField.text)));
            if (PlayerPrefs.HasKey("SavedPassword") && _loginFields.PasswordField.text == "")
                securedPassword = PlayerPrefs.GetString("SavedPassword");
            return securedPassword;
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

            var rawUserData = _responseText.Split('|');
            tempUser.uid = Int32.Parse(rawUserData[0]);
            tempUser.name = rawUserData[1];
            tempUser.email = rawUserData[2];
            tempUser.password = rawUserData[3];
            tempUser.access_level = (AccessLevel)Int32.Parse(rawUserData[4]);

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