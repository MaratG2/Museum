using System;
using System.Collections;
using System.Collections.Generic;
using Admin.PHP;
using Admin.Utility;
using Admin.View;
using GenerationMap;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Admin.Edit
{
    public class AdminEditMode : MonoBehaviour
    {
        [SerializeField] private Sprite _doorSprite,
            _frameSprite,
            _infoSprite,
            _cupSprite,
            _medalSprite,
            _rubberSprite,
            _videoSprite,
            _decorSprite,
            _selectSprite;

        [SerializeField] private HallViewer _hallViewer;
        [SerializeField] private RectTransform _paintsParent;
        [SerializeField] private RectTransform _cursorTile;
        [SerializeField] private RectTransform _imagePreview;
        [SerializeField] private Toggle _toggleMaintained, _toggleHidden;
        [SerializeField] private TMP_InputField _nameText;
        [SerializeField] private CanvasGroup _changePropertiesGroup;
        [SerializeField] private CanvasGroup _photoVideoGroup;
        [SerializeField] private CanvasGroup _decorGroup;
        [SerializeField] private CanvasGroup _infoGroup;
        [SerializeField] private CanvasGroup _confirmGroup;
        [SerializeField] private TextMeshProUGUI _propertiesHeader;
        [SerializeField] private TMP_InputField _propertiesName;
        [SerializeField] private TMP_InputField _propertiesUrl;
        [SerializeField] private TMP_InputField _propertiesDesc;
        [SerializeField] private TMP_InputField _infoBoxName;
        [SerializeField] private TMP_Dropdown _decorationsDropdown;
        [SerializeField] private TMP_Dropdown _wallDropdown;
        [SerializeField] private TMP_Dropdown _floorDropdown;
        [SerializeField] private TMP_Dropdown _roofDropdown;
        [SerializeField] private TMP_InputField _dateBegin;
        [SerializeField] private TMP_InputField _dateEnd;
        [SerializeField] private Button _doorTool;

        private TilesDrawer _tilesDrawer;
        private InfoController _infoController;
        private int _currentTool = -999;
        private int[][] _hallPlan;
        private Vector2 _startTilePos = Vector2.zero;
        private List<Vector2> posToDelete = new List<Vector2>();
        private Tile _tileSelected;
        private bool _isCursorLock;
        private bool _isDoorBlock;
        private QueriesToPHP _queriesToPhp = new(isDebugOn: true);
        private HallQueries _hallQueries = new HallQueries();
        private List<HallContent> _cachedHallContents = new();
        private Action<string> OnResponseCallback;
        private string _response;
        private Vector2 _tiledMousePos;
        private float _tileSize;

        private void Awake()
        {
            _tilesDrawer = GetComponent<TilesDrawer>();
        }
        
        private void Start()
        {
            _infoController = FindObjectOfType<InfoController>();
            SelectTool(-1);
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
            SelectTool(-1);
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
                _dateBegin.placeholder.GetComponent<TextMeshProUGUI>().text = _hallViewer.HallSelected.date_begin;
            if (!_hallViewer.HallSelected.is_date_e)
                _dateEnd.placeholder.GetComponent<TextMeshProUGUI>().text = "0000-12-31 23:59:59";
            else
                _dateEnd.placeholder.GetComponent<TextMeshProUGUI>().text = _hallViewer.HallSelected.date_end;

            _startTilePos = Vector2.zero;
        }

        public void DeleteHall()
        {
            SelectTool(-1);
            TurnCanvasGroupTo(ref _confirmGroup, true);
        }

        public void DeleteHallConfirm()
        {
            StartCoroutine(DeleteHallQuery(_hallViewer.HallSelected.hnum));
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
            TurnCanvasGroupTo(ref _confirmGroup, false);
        }

        public void SaveHall()
        {
            StartCoroutine(SaveHallCR());
        }

        private IEnumerator SaveHallCR()
        {
            yield return UpdateHallQuery(_hallViewer.HallSelected.hnum);

            for (int i = 0; i < _paintsParent.childCount; i++)
            {
                var c = _paintsParent.GetChild(i).GetComponent<Tile>().hallContent;
                c.hnum = _hallViewer.HallSelected.hnum;
                yield return InsertOrUpdateContentQuery(c);
            }

            foreach (var posDel in posToDelete)
                yield return DeleteContentQuery(posDel.x + "_" + posDel.y);
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

        private IEnumerator DeleteContentQuery(string combined_pos)
        {
            string phpFileName = "delete_content.php";
            WWWForm data = new WWWForm();
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
            data.AddField("time_end", GetDate(false));
            yield return _queriesToPhp.PostRequest(phpFileName, data, OnResponseCallback);
            if (_response == "Query completed")
            {
            }
            else
                Debug.LogError("Update hall query: " + _response);
        }

        private IEnumerator GetAndDrawHallContents(int hnum)
        {
            yield return _hallQueries.GetAllContentsByHnum(hnum);
            float tileSize = _imagePreview.sizeDelta.x / _hallViewer.HallSelected.sizex;
            foreach (var newContent in _cachedHallContents)
            {
                SelectTool(newContent.type);
                Vector2 tilePos = _startTilePos + new Vector2(newContent.pos_x, newContent.pos_z);
                Vector2 drawPos = new Vector2
                (
                    tilePos.x * tileSize,
                    tilePos.y * tileSize
                );
                Paint(tilePos, drawPos, true, newContent);
            }
        }

        private void UpdateCursorPosition()
        {
            Vector2 windowSize = new Vector2(Screen.currentResolution.width, Screen.currentResolution.height);
            Vector2 absoluteMousePos = Input.mousePosition;
            _tileSize = _imagePreview.sizeDelta.x / _hallViewer.HallSelected.sizex;
            _cursorTile.sizeDelta = new Vector2(_tileSize, _tileSize);

            float addPosX = 0, addPosY = _tileSize / 4;
            if (_hallViewer.HallSelected.sizez % 2 == 0)
                addPosY = -_tileSize / 4;
            if (_hallViewer.HallSelected.sizex % 2 != 0)
                addPosX = _tileSize / 2;

            _imagePreview.anchoredPosition = new Vector2
            (
                Mathf.FloorToInt((0.35f) * (windowSize.x / _tileSize)) * _tileSize + addPosX,
                Mathf.FloorToInt((0.55f) * (windowSize.y / _tileSize)) * _tileSize + addPosY
            );
            _tiledMousePos = new Vector2
            (
                Mathf.FloorToInt((absoluteMousePos.x / windowSize.x) * (windowSize.x / _tileSize)) * _tileSize +
                _tileSize / 2,
                Mathf.FloorToInt(((absoluteMousePos.y + _tileSize / 4) / windowSize.y) * (windowSize.y / _tileSize)) *
                _tileSize + _tileSize / 4
            );
            
            bool isOverPreview = false;
            GameObject[] casted =
                RaycastUtilities.UIRaycasts(RaycastUtilities.ScreenPosToPointerData(absoluteMousePos));
            foreach (var c in casted)
            {
                if (c.GetComponent<HallPreviewResizer>())
                    isOverPreview = true;
            }

            if (!_isCursorLock)
            {
                if (absoluteMousePos.x < 0.75f * windowSize.x && isOverPreview)
                    _cursorTile.anchoredPosition = _tiledMousePos;
                else
                    _cursorTile.anchoredPosition = -windowSize;
            }
            //Debug.Log(_tiledMousePos/_tileSize);
        }

        private void BlockDoorToolIfNeeded()
        {
            bool turnToTrue = true;
            for (int i = 0; i < _paintsParent.childCount; i++)
            {
                Tile tileChange = _paintsParent.GetChild(i).GetComponent<Tile>();
                if (tileChange.hallContent.type == ExhibitsConstants.SpawnPoint.Id)
                    turnToTrue = false;
            }

            _doorTool.interactable = turnToTrue;
            _isDoorBlock = !turnToTrue;
        }

        private bool CanDraw()
        {
            return (_currentTool == ExhibitsConstants.SpawnPoint.Id
                   || _currentTool == ExhibitsConstants.Picture.Id
                   || _currentTool == ExhibitsConstants.InfoBox.Id
                   || _currentTool == ExhibitsConstants.Cup.Id
                   || _currentTool == ExhibitsConstants.Medal.Id
                   || _currentTool == ExhibitsConstants.Video.Id
                   || _currentTool == ExhibitsConstants.Decoration.Id)
                   && IsCursorReady();
        }

        private bool IsCursorReady()
        {
            return _cursorTile.anchoredPosition.x > 1 && _changePropertiesGroup.alpha == 0;
        }

        private bool IsHallSelected()
        {
            return _hallViewer.HallSelected.sizex != 0 && _hallViewer.HallSelected.sizez != 0;
        }

        void Update()
        {
            if (!IsHallSelected())
                return;

            UpdateCursorPosition();
            BlockDoorToolIfNeeded();

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
                StartCoroutine(_tilesDrawer.DrawTilesForHall(_hallViewer.HallSelected));
            }

            if (CanDraw() && Input.GetMouseButtonDown(0))
                Paint(_tiledMousePos / _tileSize, _cursorTile.anchoredPosition);
       

            if (_currentTool is -3 && IsCursorReady())
            {
                if (Input.GetMouseButtonDown(0))
                {
                    Vector2 tiledPos = _tiledMousePos / _tileSize;
                    Vector2 tileRealPos = new Vector2(Mathf.FloorToInt(tiledPos.x - _startTilePos.x),
                        Mathf.FloorToInt(tiledPos.y - _startTilePos.y));
                    for (int i = 0; i < _paintsParent.childCount; i++)
                    {
                        Tile tileChange = _paintsParent.GetChild(i).GetComponent<Tile>();
                        if (tileChange && tileChange.hallContent.pos_x == tileRealPos.x &&
                            tileChange.hallContent.pos_z == tileRealPos.y)
                        {
                            if (tileChange.hallContent.type == ExhibitsConstants.SpawnPoint.Id)
                            {
                                Debug.Log("Tile Change Door: " + i);
                            }

                            if (tileChange.hallContent.type == ExhibitsConstants.Picture.Id)
                            {
                                _propertiesHeader.text = "Редактирование фото";
                                _isCursorLock = true;
                                TurnCanvasGroupTo(ref _changePropertiesGroup, true);
                                TurnCanvasGroupTo(ref _photoVideoGroup, true);
                                _propertiesName.text = tileChange.hallContent.title;
                                _propertiesUrl.text = tileChange.hallContent.image_url;
                                _propertiesDesc.text = tileChange.hallContent.image_desc;
                                _tileSelected = tileChange;
                            }

                            if (tileChange.hallContent.type == ExhibitsConstants.InfoBox.Id)
                            {
                                _isCursorLock = true;
                                TurnCanvasGroupTo(ref _changePropertiesGroup, true);
                                TurnCanvasGroupTo(ref _infoGroup, true);
                                _infoBoxName.text = tileChange.hallContent.title;
                                _infoController.Setup(tileChange.hallContent.image_desc);
                                _tileSelected = tileChange;
                            }

                            if (tileChange.hallContent.type == ExhibitsConstants.Cup.Id)
                            {
                                Debug.Log("Tile Change Cup: " + i);
                            }

                            if (tileChange.hallContent.type == ExhibitsConstants.Medal.Id)
                            {
                                Debug.Log("Tile Change Medal: " + i);
                            }

                            if (tileChange.hallContent.type == ExhibitsConstants.Video.Id)
                            {
                                _propertiesHeader.text = "Редактирование видео";
                                _isCursorLock = true;
                                TurnCanvasGroupTo(ref _changePropertiesGroup, true);
                                TurnCanvasGroupTo(ref _photoVideoGroup, true);
                                _propertiesName.text = tileChange.hallContent.title;
                                _propertiesUrl.text = tileChange.hallContent.image_url;
                                _propertiesDesc.text = tileChange.hallContent.image_desc;
                                _tileSelected = tileChange;
                            }

                            if (tileChange.hallContent.type == ExhibitsConstants.Decoration.Id)
                            {
                                _decorationsDropdown.value = Int32.Parse(tileChange.hallContent.title);
                                _isCursorLock = true;
                                TurnCanvasGroupTo(ref _changePropertiesGroup, true);
                                TurnCanvasGroupTo(ref _decorGroup, true);
                                _tileSelected = tileChange;
                            }
                        }
                    }
                }
            }

            if (_currentTool is -2 && _cursorTile.anchoredPosition.x > 1 && _changePropertiesGroup.alpha == 0)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    Vector2 tiledPos = _tiledMousePos / _tileSize;
                    _hallPlan[Mathf.FloorToInt(tiledPos.x - _startTilePos.x)][
                        Mathf.FloorToInt(tiledPos.y - _startTilePos.y)] = -1;
                    for (int i = 0; i < _paintsParent.childCount; i++)
                    {
                        Tile tileDelete = _paintsParent.GetChild(i).GetComponent<Tile>();
                        if (tileDelete && tileDelete.hallContent.pos_x == Mathf.FloorToInt(tiledPos.x - _startTilePos.x)
                                       && tileDelete.hallContent.pos_z ==
                                       Mathf.FloorToInt(tiledPos.y - _startTilePos.y))
                        {
                            Debug.Log("Delete: " + i);
                            posToDelete.Add(new Vector2(tileDelete.hallContent.pos_x, tileDelete.hallContent.pos_z));
                            Destroy(tileDelete.gameObject);
                        }
                    }
                }
            }
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
            _isCursorLock = false;
            TurnCanvasGroupTo(ref _changePropertiesGroup, false);
            TurnCanvasGroupTo(ref _photoVideoGroup, false);
            TurnCanvasGroupTo(ref _decorGroup, false);
            TurnCanvasGroupTo(ref _infoGroup, false);
        }

        private void TurnCanvasGroupTo(ref CanvasGroup canvasGroup, bool turnTo)
        {
            canvasGroup.alpha = turnTo ? 1f : 0f;
            canvasGroup.interactable = turnTo;
            canvasGroup.blocksRaycasts = turnTo;
        }

        public void SaveProperties()
        {
            if (!_tileSelected)
                return;
            if (_tileSelected.hallContent.type == ExhibitsConstants.Picture.Id
                || _tileSelected.hallContent.type == ExhibitsConstants.Video.Id)
            {
                _tileSelected.hallContent.title = _propertiesName.text;
                _tileSelected.hallContent.image_url = _propertiesUrl.text;
                _tileSelected.hallContent.image_desc = _propertiesDesc.text;
            }

            if (_tileSelected.hallContent.type == ExhibitsConstants.InfoBox.Id)
            {
                _infoController.InfoPartsChanged();
                _tileSelected.hallContent.title = _infoBoxName.text;
                _tileSelected.hallContent.image_url = "InfoBox";
                _tileSelected.hallContent.image_desc = _infoController.AllJsonData;
            }

            if (_tileSelected.hallContent.type == ExhibitsConstants.Decoration.Id)
            {
                _tileSelected.hallContent.title = _decorationsDropdown.value.ToString();
                _tileSelected.hallContent.image_url = "Decoration";
                _tileSelected.hallContent.image_desc = _decorationsDropdown.options[_decorationsDropdown.value].text;
            }

            HidePropertiesGroup();
        }

        public void ClearAll()
        {
            for (int i = 0; i < _paintsParent.childCount; i++)
                Destroy(_paintsParent.GetChild(i).gameObject);
            posToDelete = new List<Vector2>();
        }

        private void Paint(Vector2 tiledPos, Vector2 pos, bool hasStruct = false, HallContent content = new())
        {
            if (_hallPlan == null)
                return;
            if (Mathf.FloorToInt(tiledPos.x - _startTilePos.x) >= _hallPlan.Length ||
                (Mathf.FloorToInt(tiledPos.x - _startTilePos.x) < _hallPlan.Length
                 && Mathf.FloorToInt(tiledPos.y - _startTilePos.y) >=
                 _hallPlan[Mathf.FloorToInt(tiledPos.x - _startTilePos.x)].Length))
            {
                return;
            }

            var newTile = Instantiate(_cursorTile.gameObject, _cursorTile.anchoredPosition, Quaternion.identity,
                _paintsParent);
            newTile.GetComponent<RectTransform>().anchorMin = Vector2.zero;
            newTile.GetComponent<RectTransform>().anchorMax = Vector2.zero;
            newTile.GetComponent<RectTransform>().anchoredPosition = pos;
            newTile.GetComponent<Image>().color = _cursorTile.GetComponent<Image>().color;

            _hallPlan[Mathf.FloorToInt(tiledPos.x - _startTilePos.x)][Mathf.FloorToInt(tiledPos.y - _startTilePos.y)] =
                _currentTool;
            Tile tileInstance = newTile.GetComponent<Tile>();
            tileInstance.hallContent.type = _currentTool;
            tileInstance.hallContent.pos_x = Mathf.FloorToInt(tiledPos.x - _startTilePos.x);
            tileInstance.hallContent.pos_z = Mathf.FloorToInt(tiledPos.y - _startTilePos.y);
            tileInstance.hallContent.combined_pos =
                tileInstance.hallContent.pos_x + "_" + tileInstance.hallContent.pos_z;

            if (tileInstance.hallContent.type == ExhibitsConstants.Picture.Id
                || tileInstance.hallContent.type == ExhibitsConstants.Video.Id)
            {
                tileInstance.hallContent.image_desc = "desc";
                tileInstance.hallContent.image_url = "url";
                tileInstance.hallContent.title = "title";
            }

            if (tileInstance.hallContent.type == ExhibitsConstants.Decoration.Id)
            {
                tileInstance.hallContent.title = _decorationsDropdown.value.ToString();
                tileInstance.hallContent.image_url = "Decoration";
                tileInstance.hallContent.image_desc = _decorationsDropdown.options[_decorationsDropdown.value].text;
            }

            for (int i = 0; i < posToDelete.Count; i++)
            {
                if (tileInstance.hallContent.combined_pos == posToDelete[i].x + "_" + posToDelete[i].y)
                    posToDelete.RemoveAt(i);
            }

            //tileInstance.hallContent.uid = uid;
            if (hasStruct)
            {
                tileInstance.hallContent = content;
            }

            tileInstance.Setup();
            if (tileInstance.hallContent.type == ExhibitsConstants.SpawnPoint.Id)
            {
                SelectTool(-3);
                _isDoorBlock = true;
                _tileSelected = null;
            }
        }

        public void SelectTool(int tool)
        {
            _currentTool = tool;
            _cursorTile.GetComponent<Image>().color = Color.white;
            switch (_currentTool)
            {
                case -3:
                    _cursorTile.GetComponent<Image>().sprite = _selectSprite;
                    _cursorTile.GetComponent<Image>().raycastTarget = false;
                    break;
                case -2:
                    _cursorTile.GetComponent<Image>().sprite = _rubberSprite;
                    break;
                case -1:
                    _cursorTile.GetComponent<Image>().sprite = null;
                    _cursorTile.GetComponent<Image>().color = Color.clear;
                    break;
            }

            if (_currentTool == ExhibitsConstants.Picture.Id)
                _cursorTile.GetComponent<Image>().sprite = _frameSprite;
            else if (!_isDoorBlock && _currentTool == ExhibitsConstants.SpawnPoint.Id)
                _cursorTile.GetComponent<Image>().sprite = _doorSprite;
            else if (_currentTool == ExhibitsConstants.InfoBox.Id)
                _cursorTile.GetComponent<Image>().sprite = _infoSprite;
            else if (_currentTool == ExhibitsConstants.Cup.Id)
                _cursorTile.GetComponent<Image>().sprite = _cupSprite;
            else if (_currentTool == ExhibitsConstants.Medal.Id)
                _cursorTile.GetComponent<Image>().sprite = _medalSprite;
            else if (_currentTool == ExhibitsConstants.Video.Id)
                _cursorTile.GetComponent<Image>().sprite = _videoSprite;
            else if (_currentTool == ExhibitsConstants.Decoration.Id)
                _cursorTile.GetComponent<Image>().sprite = _decorSprite;
        }
    }
}