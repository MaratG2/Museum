using System;
using System.Collections;
using Admin.Auth;
using Admin.PHP;
using Admin.Utility;
using MaratG2.Extensions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AdminNewMode : MonoBehaviour
{
    [SerializeField] private TMP_InputField _inputName;
    [SerializeField] private TMP_InputField _inputSizeX;
    [SerializeField] private TMP_InputField _inputSizeZ;
    [SerializeField] private Toggle _dateBegin;
    [SerializeField] private Toggle _dateEnd;
    [SerializeField] private TMP_InputField _inputDateBegin;
    [SerializeField] private TMP_InputField _inputDateEnd;
    [SerializeField] private TMP_Dropdown _wallDropdown;
    [SerializeField] private TMP_Dropdown _floorDropdown;
    [SerializeField] private TMP_Dropdown _roofDropdown;
    [SerializeField] private Button _createHall;
    [SerializeField] private CanvasGroup _newCanvasGroup;
    [SerializeField] private CanvasGroup _viewCanvasGroup;
    [SerializeField] private Button _goToViewMode;
    [SerializeField] private Button _goToUsersMode;
    private AdminViewMode _adminViewMode;
    private QueriesToPHP _queriesToPhp = new (isDebugOn: true);
    private Action<string> _responseCallback;
    private string _responseText;
    private bool _isOnCooldown;

    private void Start()
    {
        _adminViewMode = FindObjectOfType<AdminViewMode>();
        _inputDateBegin.interactable = false;
        _inputDateEnd.interactable = false;
    }

    private void OnEnable()
    {
        _responseCallback += response => _responseText = response;
        var accessLevel = FindObjectOfType<Login>().CurrentUser.access_level;
        if(accessLevel == AccessLevel.Administrator)
        {
            _goToViewMode.gameObject.SetActive(false);
            _goToUsersMode.gameObject.SetActive(true);
        }
        else
        {
            _goToViewMode.gameObject.SetActive(true);
            _goToUsersMode.gameObject.SetActive(false);
        }
    }
    private void OnDisable()
    {
        _responseCallback -= response => _responseText = response;
    }

    void Update()
    {
        int sizeX, sizeZ;
        bool isX = Int32.TryParse(_inputSizeX.text, out sizeX);
        bool isZ = Int32.TryParse(_inputSizeZ.text, out sizeZ);

        if (string.IsNullOrWhiteSpace(_inputName.text) || !isX || !isZ || sizeX <= 0 || sizeZ <= 0)
        {
            _createHall.interactable = false;
            return;
        }
        if(_dateBegin.isOn)
            if (!ParseDate(_inputDateBegin.text))
                return;
        if(_dateEnd.isOn)
            if (!ParseDate(_inputDateEnd.text))
                return;
        _createHall.interactable = true;
    }

    public void CreateHall()
    {
        if (_isOnCooldown)
            return;
        _isOnCooldown = true;
        StartCoroutine(QueryInsertHall(ParseHall()));
    }

    public IEnumerator QueryInsertHall(Hall hall)
    {
        string phpFileName = "insert_hall.php";
        WWWForm data = new WWWForm();
        data.AddField(nameof(hall.name), hall.name);
        data.AddField(nameof(hall.sizex), hall.sizex);
        data.AddField(nameof(hall.sizez), hall.sizez);
        data.AddField(nameof(hall.is_date_b), hall.is_date_b ? 1 : 0);
        data.AddField(nameof(hall.is_date_e), hall.is_date_e ? 1 : 0);
        data.AddField(nameof(hall.date_begin), hall.date_begin);
        data.AddField(nameof(hall.date_end), hall.date_end);
        data.AddField(nameof(hall.is_maintained), hall.is_maintained ? 1 : 0);
        data.AddField(nameof(hall.is_hidden), hall.is_hidden ? 1 : 0);
        data.AddField(nameof(hall.author), string.IsNullOrWhiteSpace(hall.author) ? "maratg2develop@gmail.com" : hall.author);
        data.AddField(nameof(hall.wall), hall.wall);
        data.AddField(nameof(hall.floor), hall.floor);
        data.AddField(nameof(hall.roof), hall.roof);
        yield return _queriesToPhp.PostRequest(phpFileName, data, _responseCallback);
        FlushInputFields();
        CooldoownOff();
        MoveToViewWindow();
    }

    private void FlushInputFields()
    {
        _inputName.text = "";
        _inputSizeX.text = "";
        _inputSizeZ.text = "";
        _dateBegin.isOn = false;
        _dateEnd.isOn = false;
        _inputDateBegin.text = "";
        _inputDateEnd.text = "";
    }

    private void MoveToViewWindow()
    {
        _newCanvasGroup.SetActive(false);
        _viewCanvasGroup.SetActive(true);
        _adminViewMode.enabled = true;
        _adminViewMode.Refresh();
        this.enabled = false;
    }

    private Hall ParseHall()
    {
        Hall newHall = new Hall();
        newHall.name = _inputName.text;
        newHall.sizex = Int32.Parse(_inputSizeX.text);
        newHall.sizez = Int32.Parse(_inputSizeZ.text);
        newHall.is_date_b = _dateBegin.isOn;
        newHall.is_date_e = _dateEnd.isOn;
        newHall.date_begin = _dateBegin.isOn ? "'" + _inputDateBegin.text + "'" : "'0000-00-00 00:00:00'";
        newHall.date_end = _dateEnd.isOn ? "'" + _inputDateEnd.text + "'" : "'0000-00-00 00:00:00'";
        newHall.is_maintained = true;
        newHall.is_hidden = true;
        newHall.author = FindObjectOfType<Login>().CurrentUser.email;
        newHall.wall = _wallDropdown.value;
        newHall.floor = _floorDropdown.value;
        newHall.roof = _roofDropdown.value;
        return newHall;
    }

    private void CooldoownOff()
    {
        _isOnCooldown = false;
    }
    
    private bool ParseDate(string input)
    {
        if (input.Length != 19)
            return false;
        int day, month, year, hour, minute, second;
        bool isDay = Int32.TryParse(input.Substring(0, 4), out year);
        bool isMonth = Int32.TryParse(input.Substring(5, 2), out month);
        bool isYear = Int32.TryParse(input.Substring(8, 2), out day);
        bool isHour = Int32.TryParse(input.Substring(11, 2), out hour);
        bool isMinute = Int32.TryParse(input.Substring(14, 2), out minute);
        bool isSecond = Int32.TryParse(input.Substring(17, 2), out second);
        return isDay && isMonth && isYear && isHour && isMinute && isSecond;
    }
}
