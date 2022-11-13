using System;
using System.Collections;
using System.Collections.Generic;
using GoogleSheetsForUnity;
using Npgsql;
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
        HallOptions newOption = new HallOptions();
        newOption.name = _inputName.text;
        newOption.sizex = Int32.Parse(_inputSizeX.text);
        newOption.sizez = Int32.Parse(_inputSizeZ.text);
        newOption.is_date_b = _dateBegin.isOn;
        newOption.is_date_e = _dateEnd.isOn;
        newOption.date_begin = _dateBegin.isOn ? "'" + _inputDateBegin.text + "'" : "CURRENT_TIMESTAMP";
        newOption.date_end = _dateEnd.isOn ? "'" + _inputDateEnd.text + "'" : "CURRENT_TIMESTAMP";
        newOption.is_maintained = true;
        newOption.is_hidden = true;
        SQLInsertOption(newOption);
    }

    private void SQLInsertOption(HallOptions option)
    {
        NpgsqlCommand dbcmd = AdminViewMode.dbcon.CreateCommand();
        string dateSql = "SET datestyle to DMY";
        dbcmd.Prepare();
        dbcmd.CommandText = dateSql;
        dbcmd.ExecuteNonQuery();
        string sql =
            "INSERT INTO public.options (name, sizex, sizez, is_date_b, is_date_e, date_begin, date_end, is_maintained, is_hidden, operation) " +
            "VALUES('" + option.name + "'," + option.sizex + ',' + option.sizez + ',' + option.is_date_b + ',' + option.is_date_e
            + ',' + option.date_begin + ',' + option.date_end + ',' + option.is_maintained + ',' + option.is_hidden + ", 'INSERT')";
        dbcmd.Prepare();
        dbcmd.CommandText = sql;
        dbcmd.ExecuteNonQuery();
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
