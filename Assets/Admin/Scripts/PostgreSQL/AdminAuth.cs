using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using Npgsql;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.Video;

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

    private bool _isLoginCRAble = true;
    private bool _isRegistartionCRAble = true;

    void Start()
    {
        if(_isAuth)
        {
            _authCGroup.alpha = 1f;
            _authCGroup.interactable = true;
            _authCGroup.blocksRaycasts = true;
            _viewCGroup.alpha = 0f;
            _viewCGroup.interactable = false;
            _viewCGroup.blocksRaycasts = false;
            _editCGroup.alpha = 0f;
            _editCGroup.interactable = false;
            _editCGroup.blocksRaycasts = false;
            _newCGroup.alpha = 0f;
            _newCGroup.interactable = false;
            _newCGroup.blocksRaycasts = false;
            _errorReg.text = "";
            _errorReg.color = Color.red;
            _errorAuth.text = "";
            _buttonReg.interactable = false;
            _buttonAuth.interactable = false;
            Invoke(nameof(LateStart), 0.5f);
        }
        else
        {
            _authCGroup.alpha = 0f;
            _authCGroup.interactable = false;
            _authCGroup.blocksRaycasts = false;
            _viewCGroup.alpha = 1f;
            _viewCGroup.interactable = true;
            _viewCGroup.blocksRaycasts = true;
            var videoPlayer = FindObjectOfType<Video>();
            if (videoPlayer)
                Destroy(videoPlayer.gameObject);
        }
    }

    private void LateStart()
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
        _buttonReg.interactable = true;
        _buttonAuth.interactable = true;
    }

    public void TryRegistration()
    {
        if (_isRegistartionCRAble)
            StartCoroutine(TryRegistrationCoroutine());
        _isRegistartionCRAble = false;
    }

    private IEnumerator TryRegistrationCoroutine()
    {
        _nameReg.text.Trim();
        _emailReg.text.Trim();
        _passwordReg.text.Trim();
        
        if (_nameReg.text == "")
        {
            _errorReg.text = "ФИО не может быть пустым";
            _errorReg.color = Color.red;
            _isRegistartionCRAble = true;
            yield break;
        }
        if (_emailReg.text == "")
        {
            _errorReg.text = "Адрес почты не может быть пустым";
            _errorReg.color = Color.red; 
            _isRegistartionCRAble = true;
            yield break;
        }
        if (_passwordReg.text.Length is < 8 or > 24)
        {
            _errorReg.text = "Пароль не может быть меньше 8 или больше 24 символов";
            _errorReg.color = Color.red; 
            _isRegistartionCRAble = true;
            yield break;
        }
        Regex regex = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");
        Match match = regex.Match(_emailReg.text);
        if (match.Success)
        {
            //---------------PHP_LoginEmailCount
            WWWForm form = new WWWForm();
            form.AddField ("email", _emailReg.text);
            string url = PHPSettings.UrlRoot + "login_email_count.php";
            using (UnityWebRequest www = UnityWebRequest.Post(url, form))
            {
                yield return www.SendWebRequest();
                Debug.Log($"Login Email Count request ({url}): ");
            
                if (www.result != UnityWebRequest.Result.Success)
                    Debug.Log("Url: " + www.uri + " | Error: " + www.error + " | " + www.downloadHandler?.text);
                else
                {
                    string responseText = www.downloadHandler.text;
                    Debug.Log(responseText);
                    if (Int32.TryParse(responseText, out int quantity))
                    {
                        if (quantity > 0)
                        {
                            _errorReg.text = "Пользователь с заданным электронным адресом уже существует";
                            _errorReg.color = Color.red;
                            _isRegistartionCRAble = true;
                            yield break;
                        }
                    }
                }
            }
            //-----------------------------------
            
            string securedPassword = Convert.ToBase64String(
                new SHA256CryptoServiceProvider().ComputeHash(Encoding.UTF8.GetBytes(_passwordReg.text)));
            
            //---------------PHP_Registration
            WWWForm formReg = new WWWForm();
            formReg.AddField ("name", _nameReg.text);
            formReg.AddField ("email", _emailReg.text);
            formReg.AddField ("pass", securedPassword);
            string urlReg = PHPSettings.UrlRoot + "registration.php";
            using (UnityWebRequest www = UnityWebRequest.Post(urlReg, formReg))
            {
                yield return www.SendWebRequest();
                Debug.Log($"Registration request ({urlReg}): ");
            
                if (www.result != UnityWebRequest.Result.Success)
                    Debug.Log("Url: " + www.uri + " | Error: " + www.error + " | " + www.downloadHandler?.text);
                else
                {
                    string responseText = www.downloadHandler.text;
                    Debug.Log(responseText);
                    if (responseText.Split(' ')[0] == "Registered")
                    {
                        _errorReg.text = "Пользователь успешно зарегистрирован";
                        _errorReg.color = Color.green;
                    }
                    else
                    {
                        _errorReg.text = "При регистрации на стороне PHP произошла ошибка (прологгирована";
                        _errorReg.color = Color.red;
                    }
                }
            }
            //-----------------------------------
            
            _nameReg.text = "";
            _emailReg.text = "";
            _passwordReg.text = "";
        }
        else
        {
            _errorReg.text = "Адрес электронной почты не соответствует правилам ввода";
            _errorReg.color = Color.red;
        }
        _isRegistartionCRAble = true;
    }

    public void TryLogin()
    {
        if(_isLoginCRAble)
            StartCoroutine(TryLoginCoroutine());
        _isLoginCRAble = false;
    }
    
    private IEnumerator TryLoginCoroutine()
    {
        _emailAuth.text.Trim();
        _passwordAuth.text.Trim();

        if (_emailAuth.text == "")
        {
            _errorAuth.text = "Адрес почты не может быть пустым";
            _errorAuth.color = Color.red;
            _isLoginCRAble = true;
            yield break;
        }
        if (_passwordAuth.text.Length is < 8 or > 24 && !PlayerPrefs.HasKey("SavedPassword"))
        {
            _errorAuth.text = "Пароль не может быть меньше 8 или больше 24 символов";
            _errorAuth.color = Color.red;
            _isLoginCRAble = true;
            yield break;
        }
        Regex regex = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");
        Match match = regex.Match(_emailAuth.text);
        if (match.Success)
        {
            //---------------PHP_LoginEmailCount
            WWWForm form = new WWWForm();
            form.AddField ("email", _emailAuth.text);
            string url = PHPSettings.UrlRoot + "login_email_count.php";
            using (UnityWebRequest www = UnityWebRequest.Post(url, form))
            {
                yield return www.SendWebRequest();
                Debug.Log($"Login Email Count request ({url}): ");
            
                if (www.result != UnityWebRequest.Result.Success)
                    Debug.Log("Url: " + www.uri + " | Error: " + www.error + " | " + www.downloadHandler?.text);
                else
                {
                    string responseText = www.downloadHandler.text;
                    Debug.Log(responseText);
                    if (Int32.TryParse(responseText, out int quantity))
                    {
                        if (quantity <= 0)
                        {
                            _errorAuth.text = "Пользователя с таким адресом электронной почты не существует";
                            _errorAuth.color = Color.red;
                            _isLoginCRAble = true;
                            yield break;
                        }
                    }
                }
            }
            //-----------------------------------

            string securedPassword = Convert.ToBase64String(
                new SHA256CryptoServiceProvider().ComputeHash(Encoding.UTF8.GetBytes(_passwordAuth.text)));
            if(PlayerPrefs.HasKey("SavedPassword"))
                securedPassword = PlayerPrefs.GetString("SavedPassword");
            
            //---------------PHP_Login
            WWWForm formLogin = new WWWForm();
            formLogin.AddField ("email", _emailAuth.text);
            formLogin.AddField ("pass", securedPassword);
            string urlLogin = PHPSettings.UrlRoot + "login_full.php";
            bool isIn = false;
            User tempUser = new User();
            using (UnityWebRequest www = UnityWebRequest.Post(urlLogin, formLogin))
            {
                yield return www.SendWebRequest();
                Debug.Log($"Login request ({urlLogin}): ");
            
                if (www.result != UnityWebRequest.Result.Success)
                    Debug.Log("Url: " + www.uri + " | Error: " + www.error + " | " + www.downloadHandler?.text);
                else
                {
                    string responseText = www.downloadHandler.text;
                    if (responseText.Length > 0 && responseText.Split(' ')[0] != "Query")
                    {
                        isIn = true;
                        var datas = responseText.Split('|');
                        
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
                }
            }

            if (isIn)
            {
                if (tempUser.access_level == 0)
                {
                    _errorAuth.text = "Пользователь не активирован администратором музея. Вход запрещен";
                    _errorAuth.color = Color.red;
                    _isLoginCRAble = true;
                    yield break;
                }
                else
                {
                    if(_toggleSavePassword.isOn)
                    {
                        PlayerPrefs.SetString("SavedPassword", tempUser.password);
                        PlayerPrefs.SetString("SavedEmail", tempUser.email);
                    }
                    else
                    {
                        PlayerPrefs.DeleteKey("SavedPassword");
                        PlayerPrefs.DeleteKey("SavedEmail");
                    }
                    CurrentUser = tempUser;
                    _emailAuth.text = "";
                    _passwordAuth.text = "";
                    _errorAuth.text = "Авторизация прошла успешна";
                    _errorAuth.color = Color.green;
                    var videoPlayer = FindObjectOfType<VideoPlayer>();
                    if(videoPlayer)
                        videoPlayer.Stop();
                    Invoke(nameof(LateLoginned), 0.5f);
                    _isLoginCRAble = true;
                    yield break;
                }
            }
        }
        else
        {
            _errorAuth.text = "Адрес электронной почты и пароль не совпадают";
            _errorAuth.color = Color.red;
        }
        _isLoginCRAble = true;
    }

    private void LateLoginned()
    {
        _authCGroup.alpha = 0f;
        _authCGroup.interactable = false;
        _authCGroup.blocksRaycasts = false;
        _viewCGroup.alpha = 1f;
        _viewCGroup.interactable = true;
        _viewCGroup.blocksRaycasts = true;
    }
}