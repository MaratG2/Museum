using System;
using System.Collections;
using System.Collections.Generic;
using GoogleSheetsForUnity;
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
    [SerializeField] private Button _createHall;

    private bool _isOnCooldown;
    private string _tableOptionsName = "Options";
    
    [System.Serializable]
    public struct HallOptions
    {
        public string name;
        public int sizex;
        public int sizez;
        public string is_date_b;
        public string is_date_e;
        public string date_begin;
        public string date_end;
        public string is_maintained;
        public string is_hidden;
    }
    
    void Update()
    {
        int sizeX = 0, sizeZ;
        bool isX = Int32.TryParse(_inputSizeX.text, out sizeX);
        bool isZ = Int32.TryParse(_inputSizeZ.text, out sizeZ);

        if (_inputName.text.Equals("") || !isX || !isZ || sizeX <= 0 || sizeZ <= 0)
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
        Invoke(nameof(CooldoownOff), 1f);
        HallOptions newOptions = new HallOptions();
        newOptions.name = _inputName.text;
        newOptions.sizex = Int32.Parse(_inputSizeX.text);
        newOptions.sizez = Int32.Parse(_inputSizeZ.text);
        newOptions.is_date_b = _dateBegin.isOn.ToString();
        newOptions.is_date_e = _dateEnd.isOn.ToString();
        newOptions.date_begin = _dateBegin.isOn ? _inputDateBegin.text : "";
        newOptions.date_end = _dateEnd.isOn ? _inputDateEnd.text : "";
        newOptions.is_maintained = false.ToString();
        newOptions.is_hidden = true.ToString();
        CreateHallTable(newOptions.name);
        SaveHallOptions(newOptions);
    }

    private void CreateHallTable(string tableName)
    {
        Debug.Log("<color=yellow>Creating a hall table in the cloud for hall data.</color>");

        // Creating a string array for field names (table headers) .
        string[] fieldNames = new string[6];
        fieldNames[0] = "uid";
        fieldNames[1] = "title";
        fieldNames[2] = "image_url";
        fieldNames[3] = "image_desc";
        fieldNames[4] = "pos_x";
        fieldNames[5] = "pos_z";

        // Request for the table to be created on the cloud.
        Drive.CreateTable(fieldNames, tableName, true);
    }
    private void SaveHallOptions(HallOptions options)
    {
        // Get the json string of the object.
        string jsonOptions = JsonUtility.ToJson(options);

        Debug.Log("<color=yellow>Sending following hall options to the cloud: \n</color>" + options);

        // Save the object on the cloud, in a table called like the object type.
        Drive.CreateObject(jsonOptions, _tableOptionsName, true);
    }

    private void CooldoownOff()
    {
        _isOnCooldown = false;
    }
    
    private bool ParseDate(string input)
    {
        if (input.Length != 16)
            return false;
        int day, month, year, hour, minute;
        bool isDay = Int32.TryParse(input.Substring(0, 2), out day);
        bool isMonth = Int32.TryParse(input.Substring(3, 2), out month);
        bool isYear = Int32.TryParse(input.Substring(6, 4), out year);
        bool isHour = Int32.TryParse(input.Substring(11, 2), out hour);
        bool isMinute = Int32.TryParse(input.Substring(14, 2), out minute);

        return isDay && isMonth && isYear && isHour && isMinute;
    }
}
