using System;
using System.Collections;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using Admin.PHP;
using Admin.Utility;
using TMPro;
using UnityEngine;

namespace Admin.Auth
{
    public class Login : MonoBehaviour
    {
        [SerializeField] private TMP_InputField _emailAuth;
        [SerializeField] private TMP_InputField _passwordAuth;
        [SerializeField] private TextMeshProUGUI _errorAuth;
        private Action<string> _responseCallback;
        private string _responseText = "";
        private bool _canLogin = true;
        private QueriesToPHP _queriesToPhp = new(isDebugOn: true);
        private AuthFieldsManipulator _authFieldsManipulator;
        private AuthPanelMover _authPanelMover;
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
            _authFieldsManipulator = GetComponent<AuthFieldsManipulator>();
            _registration = GetComponent<Registration>();
            _authPanelMover = GetComponent<AuthPanelMover>();
        }

        public void TryLogin()
        {
            if (_canLogin)
            {
                _authFieldsManipulator.TrimAuthFields();
                if (IsLoginInfoWrong())
                    return;

                StartCoroutine(TryLoginCoroutine());
            }

            _canLogin = false;
        }

        private IEnumerator TryLoginCoroutine()
        {
            yield return _registration.GetEmailMatchedQuantity(_emailAuth.text);
            if (Int32.TryParse(_responseText, out int emailMatchedQuantity))
                if (emailMatchedQuantity == 0)
                {
                    _authFieldsManipulator.MessageThrowUI(_errorAuth,
                        "Пользоввателя с таким адресом электронной почты не существует", false);
                    _canLogin = true;
                    yield break;
                }

            string securedPassword = Convert.ToBase64String(
                new SHA256CryptoServiceProvider().ComputeHash(Encoding.UTF8.GetBytes(_passwordAuth.text)));
            if (PlayerPrefs.HasKey("SavedPassword") && _passwordAuth.text == "")
                securedPassword = PlayerPrefs.GetString("SavedPassword");

            yield return LoginUser(securedPassword);
            CurrentUser = ParseUser();
            if (!string.IsNullOrEmpty(CurrentUser.email))
            {
                if (CurrentUser.access_level == 0)
                    _authFieldsManipulator.MessageThrowUI(_errorAuth,
                        "Пользователь не активирован администратором музея. Вход запрещен", false);
                else
                {
                    _authFieldsManipulator.SavePassword(CurrentUser);
                    _authFieldsManipulator.EmptyAuthFields();
                    _authPanelMover.MoveToViewScreen();
                }
            }

            _canLogin = true;
        }

        private IEnumerator LoginUser(string securedPassword)
        {
            _responseText = "";
            string phpFileName = "login_full.php";
            WWWForm data = new WWWForm();
            data.AddField("email", _emailAuth.text);
            ;
            data.AddField("pass", securedPassword);
            ;
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
                _authFieldsManipulator.MessageThrowUI(_errorAuth, "Неправильный пароль", false);
                return tempUser;
            }

            if (firstWord == "Query")
            {
                _authFieldsManipulator.MessageThrowUI(_errorAuth, "Непредвиденная ошибка", false);
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
            tempUser.access_level = access_level;

            return tempUser;
        }

        private bool IsLoginInfoWrong()
        {
            if (_emailAuth.text == "")
            {
                _authFieldsManipulator.MessageThrowUI(_errorAuth, "Адрес почты не может быть пустым", false);
                return true;
            }

            if (_passwordAuth.text.Length is < 8 or > 24 && !PlayerPrefs.HasKey("SavedPassword"))
            {
                _authFieldsManipulator.MessageThrowUI(_errorAuth,
                    "Пароль не может быть меньше 8 или больше 24 символов", false);
                return true;
            }

            Regex regex = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");
            Match match = regex.Match(_emailAuth.text);
            if (!match.Success)
            {
                _authFieldsManipulator.MessageThrowUI(_errorAuth,
                    "Адрес электронной почты не соответствует правилам ввода", false);
                return true;
            }

            return false;
        }
    }
}