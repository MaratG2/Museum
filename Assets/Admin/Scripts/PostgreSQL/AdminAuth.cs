using System;
using System.Collections;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using MaratG2.Extensions;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class AdminAuth : MonoBehaviour
{
    [SerializeField] private bool _isAuth = true;
    [SerializeField] private TMP_InputField _nameReg;
    [SerializeField] private TMP_InputField _emailReg;
    [SerializeField] private TMP_InputField _passwordReg;
    [SerializeField] private TextMeshProUGUI _errorReg;
    [SerializeField] private Button _buttonReg;
    [SerializeField] private TMP_InputField _emailAuth;
    [SerializeField] private TMP_InputField _passwordAuth;
    [SerializeField] private TextMeshProUGUI _errorAuth;
    [SerializeField] private Button _buttonAuth;
    [SerializeField] private CanvasGroup _authCGroup;
    [SerializeField] private CanvasGroup _viewCGroup;
    [SerializeField] private CanvasGroup _editCGroup;
    [SerializeField] private CanvasGroup _newCGroup;
    [SerializeField] private Toggle _toggleSavePassword;
    private Action<string> _responseCallback;
    
    [System.Serializable]
    public struct User
    {
        public int uid;
        public string name;
        public string email;
        public string password;
        public int access_level;
    }

    private User _currentUser;
    public User CurrentUser
    {
        get { return _currentUser; }
        set { _currentUser = value; }
    }

    private QueriesToPHP _queriesToPhp = new QueriesToPHP();
    private bool _canLogin = true;
    private bool _canRegister = true;
    private string _responseText = "";
    
    private void OnEnable()
    {
        _responseCallback += response => _responseText = response;
    }
    private void OnDisable()
    {
        _responseCallback -= response => _responseText = response;
    }
    
    private void Start()
    {
        if(_isAuth)
        {
            MoveToAuthScreen();
            GetSavedPassword();
        }
        else
            MoveToViewScreen();
    }
    
    public void TryRegistration()
    {
        if (_canRegister)
        {
            TrimAuthFields();
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
                MessageThrowUI(_errorReg, "Пользователя с таким адресом электронной почты уже существует", false);
                _canRegister = true;
                yield break;
            }

        string securedPassword = Convert.ToBase64String(new SHA256CryptoServiceProvider()
            .ComputeHash(Encoding.UTF8.GetBytes(_passwordReg.text)));

        yield return RegisterUser(securedPassword);
        if (_responseText.Split(' ')[0] == "Registered")
            MessageThrowUI(_errorReg, "Пользователь успешно зарегистрирован", true);
        else
            MessageThrowUI(_errorReg, "При регистрации произошла непредвиденная ошибка", false);

        EmptyAuthFields();
        _canRegister = true;
    }
    private IEnumerator GetEmailMatchedQuantity(string email)
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
    
    public void TryLogin()
    {
        if(_canLogin)
        {
            TrimAuthFields();
            if (IsLoginInfoWrong())
                return;
            
            StartCoroutine(TryLoginCoroutine());
        }
        _canLogin = false;
    }
    private IEnumerator TryLoginCoroutine()
    {
        yield return GetEmailMatchedQuantity(_emailAuth.text);
        if (Int32.TryParse(_responseText, out int emailMatchedQuantity))
            if (emailMatchedQuantity == 0)
            {
                MessageThrowUI(_errorAuth, "Пользоввателя с таким адресом электронной почты не существует", false);
                _canLogin = true;
                yield break;
            }

        string securedPassword = Convert.ToBase64String(
            new SHA256CryptoServiceProvider().ComputeHash(Encoding.UTF8.GetBytes(_passwordAuth.text)));
        if (PlayerPrefs.HasKey("SavedPassword"))
            securedPassword = PlayerPrefs.GetString("SavedPassword");

        yield return LoginUser(securedPassword);
        CurrentUser = ParseUser();
        if (CurrentUser.email.Length > 0)
        {
            if (CurrentUser.access_level == 0)
                MessageThrowUI(_errorAuth, "Пользователь не активирован администратором музея. Вход запрещен", false);
            else
            {
                SavePassword(CurrentUser);
                EmptyAuthFields();
                MoveToViewScreen();
            }
        }
        _canLogin = true;
    }
    private IEnumerator LoginUser(string securedPassword)
    {
        _responseText = "";
        string phpFileName = "login_full.php";
        WWWForm data = new WWWForm();
        data.AddField("email", _emailAuth.text);;
        data.AddField("pass", securedPassword);;
        yield return _queriesToPhp.PostRequest(phpFileName, data, _responseCallback);
    }
    private User ParseUser()
    {
        User tempUser = new User();
        if (_responseText.Length > 0 && _responseText.Split(' ')[0] != "Query")
        {
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
        }
        return tempUser;
    }
    
    private bool IsRegistrationInfoWrong()
    {
        if (_nameReg.text == "")
        {
            MessageThrowUI(_errorReg, "ФИО не может быть пустым", false);
            return true;
        }
        if (_emailReg.text == "")
        {
            MessageThrowUI(_errorReg, "Адрес почты не может быть пустым", false);
            return true;
        }
        if (_passwordReg.text.Length is < 8 or > 24)
        {
            MessageThrowUI(_errorReg, "Пароль не может быть меньше 8 или больше 24 символов", false);
            return true;
        }
        
        Regex regex = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");
        Match match = regex.Match(_emailReg.text);
        if (!match.Success)
        {
            MessageThrowUI(_errorReg, "Адрес электронной почты не соответствует правилам ввода", false);
            return true;
        }
        
        return false;
    }
    private bool IsLoginInfoWrong()
    {
        if (_emailAuth.text == "")
        {
            MessageThrowUI(_errorAuth, "Адрес почты не может быть пустым", false);
            return true;
        }
        if (_passwordAuth.text.Length is < 8 or > 24 && !PlayerPrefs.HasKey("SavedPassword"))
        {
            MessageThrowUI(_errorAuth, "Пароль не может быть меньше 8 или больше 24 символов", false);
            return true;
        }
        
        Regex regex = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");
        Match match = regex.Match(_emailAuth.text);
        if (!match.Success)
        {
            MessageThrowUI(_errorAuth, "Адрес электронной почты не соответствует правилам ввода", false);
            return true;
        }
        
        return false;
    }
    
    private void GetSavedPassword()
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
    private void EmptyAuthFields()
    {
        _emailAuth.text = "";
        _passwordAuth.text = "";
        _nameReg.text = "";
        _emailReg.text = "";
        _passwordReg.text = "";
    }
    private void TrimAuthFields()
    {
        _emailAuth.text.Trim();
        _passwordAuth.text.Trim();
        _nameReg.text.Trim();
        _emailReg.text.Trim();
        _passwordReg.text.Trim();
    }
    private void SavePassword(User user)
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
    
    private void MessageThrowUI(TextMeshProUGUI _textUI, string message, bool isGood)
    {
        _textUI.text = message;
        _textUI.color = isGood ? Color.green : Color.red;
    }
    private void MoveToAuthScreen()
    {
        _authCGroup.SetActive(true);
        _viewCGroup.SetActive(false);
        _editCGroup.SetActive(false);
        _newCGroup.SetActive(false);
        _errorReg.text = "";
        _errorAuth.text = "";
    }
    private void MoveToViewScreen()
    {
        _authCGroup.SetActive(false);
        _viewCGroup.SetActive(true);
        _editCGroup.SetActive(false);
        _newCGroup.SetActive(false);
    }
}