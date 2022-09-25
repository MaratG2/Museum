using System;
using System.Collections;
using System.Collections.Generic;
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
    
    void Update()
    {
        int sizeX = 0, sizeZ;
        bool isX = Int32.TryParse(_inputSizeX.text, out sizeX);
        bool isZ = Int32.TryParse(_inputSizeZ.text, out sizeZ);

        if (_inputName.text.Equals("") || !isX || !isZ)
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

    private bool ParseDate(string input)
    {
        Debug.Log(input.Length);
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
