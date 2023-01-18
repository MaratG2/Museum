using System;
using System.Collections;
using System.Collections.Generic;
using Admin.GenerationMap;
using Admin.PHP;
using Admin.Utility;
using Admin.View;
using MaratG2.Extensions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Admin.Edit
{
    /// <summary>
    /// Отвечает за редактирование настроек зала – даты открытия/закрытие, название зала, его доступность, удаление. 
    /// </summary>
    /// <remarks>
    /// Также хранит в себе массив, являющийся 2D планом содержимого зала. В нем происходят все взаимодействия с базой данных – сохранение настроек зала, содержимого, их удаление.
    /// </remarks>
    public class HallEditor : MonoBehaviour
    {
        [SerializeField] private HallViewer _hallViewer;
        [SerializeField] private RectTransform _paintsParent;
        [SerializeField] private Toggle _toggleMaintained, _toggleHidden;
        [SerializeField] private TMP_InputField _nameText;
        [SerializeField] private CanvasGroup _changePropertiesGroup;
        [SerializeField] private CanvasGroup _photoVideoGroup;
        [SerializeField] private CanvasGroup _decorGroup;
        [SerializeField] private CanvasGroup _infoGroup;
        [SerializeField] private CanvasGroup _confirmGroup;
        [SerializeField] private TMP_Dropdown _wallDropdown;
        [SerializeField] private TMP_Dropdown _floorDropdown;
        [SerializeField] private TMP_Dropdown _roofDropdown;
        [SerializeField] private TMP_InputField _dateBegin;
        [SerializeField] private TMP_InputField _dateEnd;

        private TilesDrawer _tilesDrawer;
        private int[][] _hallPlan;
        private Vector2 _startTilePos = Vector2.zero;
        private List<Vector2> posToDelete = new List<Vector2>();
        private QueriesToPHP _queriesToPhp = new(isDebugOn: true);
        private HallQueries _hallQueries = new HallQueries();
        private List<HallContent> _cachedHallContents = new();
        private Action<string> OnResponseCallback;
        private string _response;

        private ToolSelector _toolSelector;
        private EditBrush _editBrush;
        private RubberBrush _rubberBrush;
        private EditCursor _editCursor;
        private EditInfoBox _editInfoBox;
        private EditMedia _editMedia;
        private EditDecoration _editDecoration;
        private ProgressSaver _progressSaver;

        private void Awake()
        {
            _tilesDrawer = GetComponent<TilesDrawer>();
            _toolSelector = GetComponent<ToolSelector>();
            _editBrush = GetComponent<EditBrush>();
            _rubberBrush = GetComponent<RubberBrush>();
            _editCursor = GetComponent<EditCursor>();
            _editInfoBox = GetComponent<EditInfoBox>();
            _editMedia = GetComponent<EditMedia>();
            _editDecoration = GetComponent<EditDecoration>();
            _progressSaver = GetComponent<ProgressSaver>();
        }

        private void OnEnable()
        {
            _tilesDrawer.OnStartTileFound += startTile => _startTilePos = startTile;
            OnResponseCallback += response => _response = response;
            _hallQueries.OnAllHallContentsGet += gotContent => _cachedHallContents = gotContent;
        }

        private void OnDisable()
        {
            _tilesDrawer.OnStartTileFound -= startTile => _startTilePos = startTile;
            OnResponseCallback -= response => _response = response;
            _hallQueries.OnAllHallContentsGet -= gotContent => _cachedHallContents = gotContent;
            _dateBegin.text = "";
            _dateEnd.text = "";
            _toolSelector.SelectTool(-1);
            StopAllCoroutines();
            _progressSaver.Clear();
        }

        public void Refresh()
        {
            if (_hallViewer.HallSelected.sizex == 0)
            {
                _nameText.text = "";
                _toggleMaintained.interactable = false;
                _toggleHidden.interactable = false;
                return;
            }

            _hallPlan = new int[_hallViewer.HallSelected.sizex][];
            for (int i = 0; i < _hallViewer.HallSelected.sizex; i++)
            {
                _hallPlan[i] = new int[_hallViewer.HallSelected.sizez];
                for (int j = 0; j < _hallViewer.HallSelected.sizez; j++)
                    _hallPlan[i][j] = -1;
            }

            _toggleMaintained.interactable = true;
            _toggleHidden.interactable = true;
            posToDelete = new List<Vector2>();
            _nameText.text = _hallViewer.HallSelected.name;
            _toggleMaintained.isOn = Convert.ToBoolean(_hallViewer.HallSelected.is_maintained);
            _toggleHidden.isOn = Convert.ToBoolean(_hallViewer.HallSelected.is_hidden);
            _wallDropdown.value = _hallViewer.HallSelected.wall;
            _floorDropdown.value = _hallViewer.HallSelected.floor;
            _roofDropdown.value = _hallViewer.HallSelected.roof;
            if (!_hallViewer.HallSelected.is_date_b)
                _dateBegin.placeholder.GetComponent<TextMeshProUGUI>().text = "0000-12-31 23:59:59";
            else
            {
                _dateBegin.placeholder.GetComponent<TextMeshProUGUI>().text = _hallViewer.HallSelected.date_begin;
                _dateBegin.text = _hallViewer.HallSelected.date_begin;
            }
            if (!_hallViewer.HallSelected.is_date_e)
                _dateEnd.placeholder.GetComponent<TextMeshProUGUI>().text = "0000-12-31 23:59:59";
            else
            {
                _dateEnd.placeholder.GetComponent<TextMeshProUGUI>().text = _hallViewer.HallSelected.date_end;
                _dateEnd.text = _hallViewer.HallSelected.date_end;
            }

            _startTilePos = Vector2.zero;
        }

        public void DeleteHall()
        {
            _toolSelector.SelectTool(-1);
            _confirmGroup.SetActive(true);
        }

        public void DeleteHallConfirm()
        {
            StartCoroutine(Deletion());
        }

        private IEnumerator Deletion()
        {
            _progressSaver.Clear();
            _progressSaver.UpdateCustomMessage("Удаление зала...");
            yield return DeleteAllContents();
            _progressSaver.UpdateCustomMessage("Удаление настроек зала...");
            yield return DeleteHallQuery(_hallViewer.HallSelected.hnum);
            _progressSaver.UpdateCustomMessage("Зал удален");
            yield return new WaitForSecondsRealtime(0.8f);
            _progressSaver.Clear();
        }
        private IEnumerator DeleteAllContents()
        {
            for (int i = 0; i < _paintsParent.childCount; i++)
            {
                var c = _paintsParent.GetChild(i).GetComponent<Tile>().hallContent;
                yield return DeleteContentQuery(_hallViewer.HallSelected.hnum, c.combined_pos);
                _progressSaver.UpdateProgress(i + 1, _paintsParent.childCount, false);
            }
        }

        private IEnumerator DeleteHallQuery(int hnum)
        {
            string phpFileName = "delete_hall.php";
            WWWForm data = new WWWForm();
            data.AddField("hnum", hnum);
            yield return _queriesToPhp.PostRequest(phpFileName, data, OnResponseCallback);
            if (_response == "Query completed")
            {
                ClearAll();
                _nameText.text = "";
                _hallViewer.HallSelected = new Hall();
                DeleteHallBack();
            }
            else
                Debug.LogError("Delete hall query: " + _response);
        }

        public void DeleteHallBack()
        {
            _confirmGroup.SetActive(false);
        }

        public void SaveHall()
        {
            StartCoroutine(SaveHallCR());
        }

        private IEnumerator SaveHallCR()
        {
            _progressSaver.Clear();
            _progressSaver.UpdateCustomMessage("Сохранение зала...");
            yield return UpdateHallQuery(_hallViewer.HallSelected.hnum);
            _progressSaver.UpdateCustomMessage("Настройки зала сохранены!");

            for (int i = 0; i < _paintsParent.childCount; i++)
            {
                var c = _paintsParent.GetChild(i).GetComponent<Tile>().hallContent;
                c.hnum = _hallViewer.HallSelected.hnum;
                yield return InsertOrUpdateContentQuery(c);
                _progressSaver.UpdateProgress(i+1, _paintsParent.childCount, true);
            }

            {
                int i = 0;
                foreach (var posDel in posToDelete)
                {
                    yield return DeleteContentQuery(_hallViewer.HallSelected.hnum, $"{posDel.x}_{posDel.y}");
                    _progressSaver.UpdateProgress(i+1, posToDelete.Count, false);
                    i++;
                }
            }
            _progressSaver.UpdateCustomMessage("Сохранение и удаление завершено");
            yield return new WaitForSecondsRealtime(0.7f);
            _progressSaver.Clear();
        }

        public void SetHallPlan(Vector2 pos, int value)
        {
            if (IsOutOfBoundsHallPlan(pos))
            {
                Debug.Log(pos);
                Debug.LogError("HallPlan out of bounds");
                return;
            }
            
            _hallPlan[(int)pos.x][(int)pos.y] = value;
        }

        public bool IsOutOfBoundsHallPlan(Vector2 pos)
        {
            return (int)pos.x >= _hallPlan.Length || (_hallPlan.Length > 0 && (int)pos.y >= _hallPlan[0].Length);
        }

        public bool IsPlanAtPosEmpty(Vector2 pos)
        {
            return _hallPlan[(int)pos.x][(int)pos.y] < 0;
        }

        public void AddToPosToDelete(Vector2 pos)
        {
            posToDelete.Add(pos);
        }

        public void RemoveFromPosToDelete(string combined_pos)
        {
            for (int i = 0; i < posToDelete.Count; i++)
            {
                if (combined_pos == $"{posToDelete[i].x}_{posToDelete[i].y}")
                    posToDelete.RemoveAt(i);
            }
        }
        
        private IEnumerator InsertOrUpdateContentQuery(HallContent c)
        {
            string phpFileName = "insert_or_update_content.php";
            WWWForm data = new WWWForm();
            data.AddField("hnum", c.hnum);
            data.AddField("title", string.IsNullOrWhiteSpace(c.title) ? "" : c.title);
            data.AddField("image_url", string.IsNullOrWhiteSpace(c.image_url) ? "" : c.image_url);
            data.AddField("image_desc", string.IsNullOrWhiteSpace(c.image_desc) ? "" : c.image_desc);
            data.AddField("combined_pos", string.IsNullOrWhiteSpace(c.combined_pos) ? "" : c.combined_pos);
            data.AddField("type", c.type);
            yield return _queriesToPhp.PostRequest(phpFileName, data, OnResponseCallback);
            if (_response == "Query completed")
            {
            }
            else
                Debug.LogError("Insert or update hall query: " + _response);
        }

        private IEnumerator DeleteContentQuery(int hnum, string combined_pos)
        {
            string phpFileName = "delete_content.php";
            WWWForm data = new WWWForm();
            Debug.Log("Delete for hnum: " + hnum);
            data.AddField("hnum", hnum);
            data.AddField("combined_pos", combined_pos);
            yield return _queriesToPhp.PostRequest(phpFileName, data, OnResponseCallback);
            if (_response == "Query completed")
            {
            }
            else
                Debug.LogError("Update hall query: " + _response);
        }

        private IEnumerator UpdateHallQuery(int hnum)
        {
            string phpFileName = "update_hall.php";
            WWWForm data = new WWWForm();
            data.AddField("name", _nameText.text);
            data.AddField("is_hidden", _toggleHidden.isOn ? "1" : "0");
            data.AddField("is_maintained", _toggleMaintained.isOn ? "1" : "0");
            data.AddField("hnum", hnum);
            data.AddField("wall", _wallDropdown.value);
            data.AddField("floor", _floorDropdown.value);
            data.AddField("roof", _roofDropdown.value);
            data.AddField("is_date_b", GetDate(true) == "" ? 0 : 1);
            data.AddField("is_date_e", GetDate(false) == "" ? 0 : 1);
            data.AddField("time_begin", GetDate(true));
            Debug.Log(GetDate(true));
            data.AddField("time_end", GetDate(false));
            yield return _queriesToPhp.PostRequest(phpFileName, data, OnResponseCallback);
            if (_response == "Query completed")
            {
            }
            else
                Debug.LogError("Update hall query: " + _response);
        }

        private bool IsHallSelected()
        {
            return _hallViewer.HallSelected.sizex != 0 && _hallViewer.HallSelected.sizez != 0;
        }

        void Update()
        {
            if (!IsHallSelected())
                return;

            if (_startTilePos == Vector2.zero)
            {
                _startTilePos = Vector2.one;
                _hallPlan = new int[_hallViewer.HallSelected.sizex][];
                for (int i = 0; i < _hallViewer.HallSelected.sizex; i++)
                {
                    _hallPlan[i] = new int[_hallViewer.HallSelected.sizez];
                    for (int j = 0; j < _hallViewer.HallSelected.sizez; j++)
                        _hallPlan[i][j] = -1;
                }

                posToDelete = new List<Vector2>();
                StartCoroutine(_tilesDrawer.DrawTilesForHall(_hallViewer.HallSelected, this));
            }

            if (!Input.GetMouseButtonDown(0) || !_editCursor.IsCursorReady())
                return;
            
            if (_toolSelector.CurrentTool is -3)
                _editBrush.Edit();
            if (_toolSelector.CurrentTool is -2)
                _rubberBrush.Delete();
        }
        
        private string GetDate(bool isBegin)
        {
            string date;
            if (isBegin)
                date = _dateBegin.text;
            else
                date = _dateEnd.text;
            if (string.IsNullOrWhiteSpace(date))
                return "";

            bool isCorrectDate = date.Length == 19;
            if (!isCorrectDate || date[0] == '0')
                return "";

            int day, month, year, hour, minute, second;
            bool isDay = Int32.TryParse(date.Substring(0, 4), out year);
            bool isMonth = Int32.TryParse(date.Substring(5, 2), out month);
            bool isYear = Int32.TryParse(date.Substring(8, 2), out day);
            bool isHour = Int32.TryParse(date.Substring(11, 2), out hour);
            bool isMinute = Int32.TryParse(date.Substring(14, 2), out minute);
            bool isSecond = Int32.TryParse(date.Substring(17, 2), out second);
            isCorrectDate = isDay && isMonth && isYear && isHour && isMinute && isSecond;
            if (!isCorrectDate)
                return "";
            return date;
        }

        public void HidePropertiesGroup()
        {
            _editCursor.ChangeCursorLock(false);
            _changePropertiesGroup.SetActive(false);
            _photoVideoGroup.SetActive(false);
            _decorGroup.SetActive(false);
            _infoGroup.SetActive(false);
        }


        public void SaveProperties()
        {
            if (!_editBrush.TileSelected)
                return;
            var hallContent = _editBrush.TileSelected.hallContent;
            if (hallContent.type == ExhibitsConstants.Picture.Id
                || hallContent.type == ExhibitsConstants.Video.Id)
            {
                hallContent.title = _editMedia.Title;
                hallContent.image_url = _editMedia.Url;
                hallContent.image_desc = _editMedia.Desc;
            }

            if (hallContent.type == ExhibitsConstants.InfoBox.Id)
            {
                _editInfoBox.InfoController.InfoPartsChanged();
                hallContent.title = _editInfoBox.Title;
                hallContent.image_url = "InfoBox";
                hallContent.image_desc = _editInfoBox.InfoController.AllJsonData;
            }

            if (hallContent.type == ExhibitsConstants.Decoration.Id)
            {
                var decorValue = _editDecoration.DecorationsDropdown.value;
                hallContent.title = decorValue.ToString();
                hallContent.image_url = "Decoration";
                hallContent.image_desc = _editDecoration.DecorationsDropdown.options[decorValue].text;
            }
            _editBrush.TileSelected.hallContent = hallContent;
            HidePropertiesGroup();
        }

        public void ClearAll()
        {
            for (int i = 0; i < _paintsParent.childCount; i++)
                Destroy(_paintsParent.GetChild(i).gameObject);
            posToDelete = new List<Vector2>();
        }
    }
}