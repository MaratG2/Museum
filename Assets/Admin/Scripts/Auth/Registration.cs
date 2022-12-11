using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;

namespace Admin.Auth
{
    public class Registration : MonoBehaviour
    {
        [SerializeField] private TMP_InputField _nameReg;
        [SerializeField] private TMP_InputField _emailReg;
        [SerializeField] private TMP_InputField _passwordReg;
        [SerializeField] private TextMeshProUGUI _errorReg;
        private Action<string> _responseCallback;
        private string _responseText = "";
        private bool _canRegister = true;
        private QueriesToPHP _queriesToPhp = new(isDebugOn: true);
        private AuthFieldsManipulator _authFieldsManipulator;

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
        }

        public void TryRegistration()
        {
            if (_canRegister)
            {
                _authFieldsManipulator.TrimAuthFields();
                if (IsRegistrationInfoWrong())
                    return;

                StartCoroutine(RegistrationCoroutine());
            }

            _canRegister = false;
        }

        private IEnumerator RegistrationCoroutine()
        {
            yield return GetEmailMatchedQuantity(_emailReg.text);
            if (Int32.TryParse(_responseText, out int emailMatchedQuantity))
                if (emailMatchedQuantity > 0)
                {
                    _authFieldsManipulator.MessageThrowUI(_errorReg,
                        "Пользователя с таким адресом электронной почты уже существует", false);
                    _canRegister = true;
                    yield break;
                }

            string securedPassword = Convert.ToBase64String(new SHA256CryptoServiceProvider()
                .ComputeHash(Encoding.UTF8.GetBytes(_passwordReg.text)));

            yield return RegisterUser(securedPassword);
            if (_responseText.Split(' ')[0] == "Registered")
                _authFieldsManipulator.MessageThrowUI(_errorReg, "Пользователь успешно зарегистрирован", true);
            else
                _authFieldsManipulator.MessageThrowUI(_errorReg, "При регистрации произошла непредвиденная ошибка",
                    false);

            _authFieldsManipulator.EmptyAuthFields();
            _canRegister = true;
        }

        public IEnumerator GetEmailMatchedQuantity(string email)
        {
            string phpFileName = "login_email_count.php";
            WWWForm data = new WWWForm();
            data.AddField("email", email);
            yield return _queriesToPhp.PostRequest(phpFileName, data, _responseCallback);
        }

        private IEnumerator RegisterUser(string securedPassword)
        {
            string phpFileName = "registration.php";
            WWWForm data = new WWWForm();
            data.AddField("name", _nameReg.text);
            data.AddField("email", _emailReg.text);
            data.AddField("pass", securedPassword);
            yield return _queriesToPhp.PostRequest(phpFileName, data, _responseCallback);
        }

        private bool IsRegistrationInfoWrong()
        {
            if (_nameReg.text == "")
            {
                _authFieldsManipulator.MessageThrowUI(_errorReg, "ФИО не может быть пустым", false);
                return true;
            }

            if (_emailReg.text == "")
            {
                _authFieldsManipulator.MessageThrowUI(_errorReg, "Адрес почты не может быть пустым", false);
                return true;
            }

            if (_passwordReg.text.Length is < 8 or > 24)
            {
                _authFieldsManipulator.MessageThrowUI(_errorReg, "Пароль не может быть меньше 8 или больше 24 символов",
                    false);
                return true;
            }

            Regex regex = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");
            Match match = regex.Match(_emailReg.text);
            if (!match.Success)
            {
                _authFieldsManipulator.MessageThrowUI(_errorReg,
                    "Адрес электронной почты не соответствует правилам ввода", false);
                return true;
            }

            return false;
        }
    }
}