using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Admin.New
{
    /// <summary>
    /// Отвечает за ввод даты открытия и закрытия зала пользователем.
    /// </summary>
    public class HallCreationDate : MonoBehaviour
    {
        [SerializeField] private Toggle _dateBegin;
        [SerializeField] private Toggle _dateEnd;
        [SerializeField] private TMP_InputField _inputDateBegin;
        [SerializeField] private TMP_InputField _inputDateEnd;

        public bool IsDateB => _dateBegin.isOn;
        public bool IsDateE => _dateEnd.isOn;
        public string DateBegin => _dateBegin.isOn ? "'" + _inputDateBegin.text + "'" : "'0000-00-00 00:00:00'";
        public string DateEnd => _dateEnd.isOn ? "'" + _inputDateEnd.text + "'" : "'0000-00-00 00:00:00'";

        private void Start()
        {
            _inputDateBegin.interactable = false;
            _inputDateEnd.interactable = false;
        }

        public void ClearDateFields()
        {
            _dateBegin.isOn = false;
            _dateEnd.isOn = false;
            _inputDateBegin.text = "";
            _inputDateEnd.text = "";
        }

        public bool IsBeginDateReady()
        {
            if (_dateBegin.isOn)
                if (!ParseDate(_inputDateBegin.text))
                    return false;

            return true;
        }

        public bool IsEndDateReady()
        {
            if (_dateEnd.isOn)
                if (!ParseDate(_inputDateEnd.text))
                    return false;

            return true;
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
}