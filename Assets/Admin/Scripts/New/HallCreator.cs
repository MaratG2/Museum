using System;
using System.Collections;
using Admin.Auth;
using Admin.PHP;
using Admin.Utility;
using Admin.View;
using MaratG2.Extensions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Admin.New
{
    /// <summary>
    /// Отвечает за парсинг и валидацию введенных данных нового зала, отправляет запрос к базе данных на его создание.
    /// </summary>
    public class HallCreator : MonoBehaviour
    {
        [SerializeField] private TMP_InputField _inputName;
        [SerializeField] private TMP_InputField _inputSizeX;
        [SerializeField] private TMP_InputField _inputSizeZ;
        [SerializeField] private TMP_Dropdown _wallDropdown;
        [SerializeField] private TMP_Dropdown _floorDropdown;
        [SerializeField] private TMP_Dropdown _roofDropdown;
        [SerializeField] private Button _createHall;
        [SerializeField] private CanvasGroup _newCanvasGroup;
        [SerializeField] private CanvasGroup _viewCanvasGroup;
        [SerializeField] private Button _goToViewMode;
        [SerializeField] private Button _goToUsersMode;
        private HallViewer _hallViewer;
        private QueriesToPHP _queriesToPhp = new(isDebugOn: true);
        private bool _isOnCooldown;
        private HallCreationDate _hallCreationDate;

        private void Awake()
        {
            _hallViewer = FindObjectOfType<HallViewer>();
            _hallCreationDate = GetComponent<HallCreationDate>();
        }

        private void OnEnable()
        {
            var accessLevel = FindObjectOfType<Login>().CurrentUser.access_level;
            if (accessLevel == AccessLevel.Administrator)
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

            if (!_hallCreationDate.IsBeginDateReady()
                || !_hallCreationDate.IsEndDateReady())
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
            data.AddField(nameof(hall.author),
                string.IsNullOrWhiteSpace(hall.author) ? "maratg2develop@gmail.com" : hall.author);
            data.AddField(nameof(hall.wall), hall.wall);
            data.AddField(nameof(hall.floor), hall.floor);
            data.AddField(nameof(hall.roof), hall.roof);
            yield return _queriesToPhp.PostRequest(phpFileName, data, response => response = response);
            FlushInputFields();
            CooldoownOff();
            MoveToViewWindow();
        }

        private void FlushInputFields()
        {
            _inputName.text = "";
            _inputSizeX.text = "";
            _inputSizeZ.text = "";
            _hallCreationDate.ClearDateFields();
        }

        private void MoveToViewWindow()
        {
            _newCanvasGroup.SetActive(false);
            _viewCanvasGroup.SetActive(true);
            _hallViewer.enabled = true;
            _hallViewer.Refresh();
            this.enabled = false;
        }

        private Hall ParseHall()
        {
            Hall newHall = new Hall();
            newHall.name = _inputName.text;
            newHall.sizex = Int32.Parse(_inputSizeX.text);
            newHall.sizez = Int32.Parse(_inputSizeZ.text);
            newHall.is_date_b = _hallCreationDate.IsDateB;
            newHall.is_date_e = _hallCreationDate.IsDateE;
            newHall.date_begin = _hallCreationDate.DateBegin;
            newHall.date_end = _hallCreationDate.DateEnd;
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
    }
}