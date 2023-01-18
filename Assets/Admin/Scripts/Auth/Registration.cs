using System;
using System.Collections;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using Admin.PHP;
using UnityEngine;

namespace Admin.Auth
{
    /// <summary>
    /// Занимается логикой регистрации пользователя.
    /// </summary>
    public class Registration : MonoBehaviour
    {
        private RegistrationFieldsProvider _registrationFields;
        private Action<string> _responseCallback;
        private string _responseText = "";
        private bool _canRegister = true;
        private QueriesToPHP _queriesToPhp = new(isDebugOn: true);
        private ILoggable _loggerUI;

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
            _registrationFields = GetComponent<RegistrationFieldsProvider>();
        }

        public void TryRegistration()
        {
            if (_canRegister)
            {
                _registrationFields.Trim();
                if (IsRegistrationInfoWrong())
                    return;

                StartCoroutine(RegistrationCR());
            }

            _canRegister = false;
        }

        private IEnumerator RegistrationCR()
        {
            yield return CheckIfUserAlreadyRegistered();

            string securedPassword = EncodePassword(_registrationFields.PasswordField.text);
            yield return RegisterUser(securedPassword);
            
            if (_responseText.Split(' ')[0] == "Registered")
                _loggerUI.LogGood(_registrationFields.ErrorText, "Пользователь успешно зарегистрирован");
            else
                _loggerUI.LogBad(_registrationFields.ErrorText, "При регистрации произошла непредвиденная ошибка");

            _registrationFields.Empty();
            _canRegister = true;
        }

        private string EncodePassword(string password)
        {
            return Convert.ToBase64String(new SHA256CryptoServiceProvider()
                .ComputeHash(Encoding.UTF8.GetBytes(password)));
        }

        private IEnumerator CheckIfUserAlreadyRegistered()
        {
            yield return GetEmailMatchedQuantity(_registrationFields.EmailField.text);
            if (Int32.TryParse(_responseText, out int emailMatchedQuantity))
                if (emailMatchedQuantity > 0)
                {
                    _loggerUI.LogBad(_registrationFields.ErrorText,
                        "Пользователя с таким адресом электронной почты уже существует");
                    _canRegister = true;
                    StopCoroutine(RegistrationCR());
                }
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
            data.AddField("name", _registrationFields.NameField.text);
            data.AddField("email", _registrationFields.EmailField.text);
            data.AddField("pass", securedPassword);
            yield return _queriesToPhp.PostRequest(phpFileName, data, _responseCallback);
        }

        private bool IsRegistrationInfoWrong()
        {
            if (_registrationFields.NameField.text == "")
            {
                _loggerUI.LogBad(_registrationFields.ErrorText, "ФИО не может быть пустым");
                return true;
            }

            if (_registrationFields.EmailField.text == "")
            {
                _loggerUI.LogBad(_registrationFields.ErrorText, "Адрес почты не может быть пустым");
                return true;
            }

            if (_registrationFields.PasswordField.text.Length is < 8 or > 24)
            {
                _loggerUI.LogBad(_registrationFields.ErrorText, "Пароль не может быть меньше 8 или больше 24 символов");
                return true;
            }

            Regex regex = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");
            Match match = regex.Match(_registrationFields.EmailField.text);
            if (!match.Success)
            {
                _loggerUI.LogBad(_registrationFields.ErrorText,
                    "Адрес электронной почты не соответствует правилам ввода");
                return true;
            }

            return false;
        }
    }
}