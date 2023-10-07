using System;
using System.Globalization;
using Admin.Utility;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Museum.Scripts.Menu
{
    public class HallListing : MonoBehaviour
    {
        [SerializeField] private TMP_Text _nameText;
        [SerializeField] private TMP_Text _timeText;
        [SerializeField] private Button _selectButton;
        private RoomsMenu _roomsMenu;
        private Hall _hall;
        
        public void Setup(Hall hall, RoomsMenu roomsMenu)
        {
            this._hall = hall;
            this._roomsMenu = roomsMenu;
            _timeText.text = "";
            string prefix = hall.is_maintained ? "(в работе) " : "";
            _nameText.text = prefix + hall.name;
            if(hall.is_date_b)
            {
                _timeText.text = "С:   " + hall.date_begin;
                if (hall.is_date_e)
                    _timeText.text += "\n";
            }
            if (hall.is_date_e)
                _timeText.text += "До: " + hall.date_end;

            if (!CanEnter())
                _selectButton.interactable = false;

            _selectButton.onClick.AddListener(() => _roomsMenu.SelectHall(this));
        }

        public int GetHNum()
        {
            return _hall.hnum;
        }

        public bool CanEnter()
        {
            if (_hall.is_maintained)
                return false;
            if (_hall.is_hidden)
                return false;
            if (IsDateOutside())
                return false;
            
            return true;
        }

        public bool IsActive()
        {
            return _selectButton.interactable;
        }

        private bool IsDateOutside()
        {
            if (!_hall.is_date_b && !_hall.is_date_e)
                return false;

            DateTime date_begin = DateTime.Now;
            DateTime date_end = DateTime.Now;
            if (_hall.is_date_b)
            {
                date_begin = DateTime.ParseExact(_hall.date_begin, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture,
                    DateTimeStyles.AssumeUniversal);
            }
            if (_hall.is_date_e)
            {
                date_end = DateTime.ParseExact(_hall.date_end, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture,
                    DateTimeStyles.AssumeUniversal);
            }
            
            if (_hall.is_date_b && TimeManager.UtcNow.Ticks >= date_begin.Ticks)
            {
                if (_hall.is_date_e && TimeManager.UtcNow.Ticks <= date_end.Ticks)
                {
                    return false;
                }
                else if (_hall.is_date_e)
                    return true;
            }
            else if (_hall.is_date_b)
                return true;
            
            if (_hall.is_date_e && TimeManager.UtcNow.Ticks <= date_end.Ticks)
            {
                return false;
            }
            else if (_hall.is_date_e)
                return true;
            
            return false;
        }
    }
}