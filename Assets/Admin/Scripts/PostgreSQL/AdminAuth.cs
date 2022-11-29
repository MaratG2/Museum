using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using Npgsql;
using TMPro;
using UnityEngine;
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

    [System.Serializable]
    public struct User
    {
        public int uid;
        public string name;
        public string email;
        public string password;
        public bool is_activated;
    }

    private User _currentUser;
    public User CurrentUser
    {
        get { return _currentUser; }
        set { _currentUser = value; }
    }

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
        _buttonReg.interactable = true;
        _buttonAuth.interactable = true;
    }

    public void TryRegistration()
    {
        _nameReg.text.Trim();
        _emailReg.text.Trim();
        _passwordReg.text.Trim();
        
        if (_nameReg.text == "")
        {
            _errorReg.text = "ФИО не может быть пустым";
            _errorReg.color = Color.red; 
            return;
        }
        if (_emailReg.text == "")
        {
            _errorReg.text = "Адрес почты не может быть пустым";
            _errorReg.color = Color.red; 
            return;
        }
        if (_passwordReg.text.Length is < 8 or > 24)
        {
            _errorReg.text = "Пароль не может быть меньше 8 или больше 24 символов";
            _errorReg.color = Color.red; 
            return;
        }
        Regex regex = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");
        Match match = regex.Match(_emailReg.text);
        if (match.Success)
        {
            NpgsqlCommand dbcmd = AdminViewMode.dbcon.CreateCommand();
            string checkSql = "SELECT count(*) from public.users " +
                             "WHERE email = '" + _emailReg.text + "'";
            dbcmd.Prepare();
            dbcmd.CommandText = checkSql;
            NpgsqlDataReader reader = dbcmd.ExecuteReader();
            while (reader.Read())
            {
                int quantity = (reader.IsDBNull(0)) ? 0 : reader.GetInt32(0);
                if (quantity > 0)
                {
                    _errorReg.text = "Пользователь с заданным электронным адресом уже существует";
                    _errorReg.color = Color.red; 
                    reader.Close();
                    dbcmd.Dispose();
                    reader = null;
                    return;
                }
            }
            reader.Close();
            dbcmd.Dispose();
            reader = null;
            
            string securedPassword = Convert.ToBase64String(
                new SHA256CryptoServiceProvider().ComputeHash(Encoding.UTF8.GetBytes(_passwordReg.text)));
            
            dbcmd = AdminViewMode.dbcon.CreateCommand();
            string regSql = "INSERT INTO public.users(name, email, password) " +
                            "VALUES ('" + _nameReg.text + "', '" + _emailReg.text + "', '" + securedPassword + "')";
            dbcmd.Prepare();
            dbcmd.CommandText = regSql;
            dbcmd.ExecuteNonQuery();
            
            _errorReg.text = "Пользователь успешно зарегистрирован";
            _errorReg.color = Color.green;

            _nameReg.text = "";
            _emailReg.text = "";
            _passwordReg.text = "";
        }
        else
        {
            _errorReg.text = "Адрес электронной почты не соответствует правилам ввода";
            _errorReg.color = Color.red; 
            return;
        }
    }
    
    public void TryLogin()
    {
        _emailAuth.text.Trim();
        _passwordAuth.text.Trim();

        if (_emailAuth.text == "")
        {
            _errorAuth.text = "Адрес почты не может быть пустым";
            _errorAuth.color = Color.red; 
            return;
        }
        if (_passwordAuth.text.Length is < 8 or > 24)
        {
            _errorAuth.text = "Пароль не может быть меньше 8 или больше 24 символов";
            _errorAuth.color = Color.red; 
            return;
        }
        Regex regex = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");
        Match match = regex.Match(_emailAuth.text);
        if (match.Success)
        {
            NpgsqlCommand dbcmd = AdminViewMode.dbcon.CreateCommand();
            string checkSql = "SELECT count(*) from public.users " +
                              "WHERE email = '" + _emailAuth.text + "'";
            dbcmd.Prepare();
            dbcmd.CommandText = checkSql;
            NpgsqlDataReader reader = dbcmd.ExecuteReader();
            while (reader.Read())
            {
                int quantity = (reader.IsDBNull(0)) ? 0 : reader.GetInt32(0);
                if (quantity <= 0)
                {
                    _errorAuth.text = "Пользователя с таким адресом электронной почты не существует";
                    _errorAuth.color = Color.red;
                    reader.Close();
                    dbcmd.Dispose();
                    reader = null;
                }
            }
            reader.Close();
            dbcmd.Dispose();
            reader = null;

            string securedPassword = Convert.ToBase64String(
                new SHA256CryptoServiceProvider().ComputeHash(Encoding.UTF8.GetBytes(_passwordAuth.text)));

            dbcmd = AdminViewMode.dbcon.CreateCommand();
            string loginSql = "SELECT * from public.users " +
                              "WHERE email = '" + _emailAuth.text + "' AND password = '" + securedPassword + "'";
            dbcmd.Prepare();
            dbcmd.CommandText = loginSql;
            reader = dbcmd.ExecuteReader();
            bool isIn = false;
            while (reader.Read())
            {
                if (!reader.IsDBNull(0))
                    isIn = true;

                User tempUser = new User();
                int uid = (reader.IsDBNull(0)) ? 0 : reader.GetInt32(0);
                string name = (reader.IsDBNull(1)) ? "NULL" : reader.GetString(1);
                string email = (reader.IsDBNull(2)) ? "NULL" : reader.GetString(2);
                string password = (reader.IsDBNull(3)) ? "NULL" : reader.GetString(3);
                bool is_activated = (reader.IsDBNull(4)) ? false : reader.GetBoolean(4);
                tempUser.uid = uid;
                tempUser.name = name;
                tempUser.email = email;
                tempUser.password = password;
                tempUser.is_activated = is_activated;

                if (!tempUser.is_activated)
                {
                    _errorAuth.text = "Пользователь не активирован администратором музея. Вход запрещен";
                    _errorAuth.color = Color.red;
                    reader.Close();
                    dbcmd.Dispose();
                    reader = null;
                    return;
                }
                else
                {
                    CurrentUser = tempUser;
                    _emailAuth.text = "";
                    _passwordAuth.text = "";
                    _errorAuth.text = "Авторизация прошла успешна";
                    _errorAuth.color = Color.green;
                    var videoPlayer = FindObjectOfType<VideoPlayer>();
                    if(videoPlayer)
                        videoPlayer.Stop();
                    reader.Close();
                    dbcmd.Dispose();
                    reader = null;
                    Invoke(nameof(LateLoginned), 0.5f);
                    return;
                }
            }

            if (!isIn)
            {
                _errorAuth.text = "Адрес электронной почты и пароль не совпадают";
                _errorAuth.color = Color.red;
            }
            reader.Close();
            dbcmd.Dispose();
            reader = null;
            return;
        }
        else
        {
            _errorAuth.text = "Адрес электронной почты не соответствует правилам ввода";
            _errorAuth.color = Color.red; 
            return;
        }
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