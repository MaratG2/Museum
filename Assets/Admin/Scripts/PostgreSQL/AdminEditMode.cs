using System;
using System.Collections;
using System.Collections.Generic;
using Admin.PHP;
using Admin.Utility;
using GenerationMap;
using Npgsql;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AdminEditMode : MonoBehaviour
{
    [SerializeField] private Sprite _doorSprite, _frameSprite, _infoSprite, _cupSprite, _medalSprite, _rubberSprite, _videoSprite, _decorSprite, _selectSprite;
    [SerializeField] private AdminViewMode _adminView;
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
    [SerializeField] private Button _doorTool;

    private InfoController _infoController;
    private int _currentTool = -999;
    private int[][] _hallPlan;
    private Vector2 _startTilePos = Vector2.zero;
    private List<Vector2> posToDelete = new List<Vector2>();
    private Tile _tileSelected;
    private bool _isCursorLock;
    private bool _isDoorBlock;
    private QueriesToPHP _queriesToPhp = new (isDebugOn: true);
    private Action<string> OnResponseCallback;
    private string _response;

    private void Start()
    {
        _infoController = FindObjectOfType<InfoController>();
        SelectTool(-1);
    }

    private void OnEnable()
    {
        OnResponseCallback += response => _response = response;
    }
    private void OnDisable()
    {
        OnResponseCallback -= response => _response = response;
    }

    public void Refresh()
    {
        if (_adminView.HallSelected.sizex == 0)
        {
            _nameText.text = "";
            _toggleMaintained.interactable = false;
            _toggleHidden.interactable = false;
            return;
        }
        _hallPlan = new int[_adminView.HallSelected.sizex][];
        for (int i = 0; i < _adminView.HallSelected.sizex; i++)
        {
            _hallPlan[i] = new int[_adminView.HallSelected.sizez];
            for (int j = 0; j < _adminView.HallSelected.sizez; j++)
                _hallPlan[i][j] = -1;
        }

        _toggleMaintained.interactable = true;
        _toggleHidden.interactable = true;
        posToDelete = new List<Vector2>();
        _nameText.text = _adminView.HallSelected.name;
        _toggleMaintained.isOn = Convert.ToBoolean(_adminView.HallSelected.is_maintained);
        _toggleHidden.isOn = Convert.ToBoolean(_adminView.HallSelected.is_hidden);
        _startTilePos = Vector2.zero;
    }

    private void FindLeftBottomTile()
    {
        float tileSize = _imagePreview.sizeDelta.x / _adminView.HallSelected.sizex;
     
        float addPosX = 0, addPosY = tileSize / 4;
        if(_adminView.HallSelected.sizez % 2 == 0)
            addPosY += -tileSize / 4;
        if (_adminView.HallSelected.sizex % 2 != 0)
            addPosX += tileSize / 2;

        for(int i = 0; i < 1920 / tileSize; i++)
        {
            for (int j = 0; j < 1080 / tileSize; j++)
            {
                bool isOverPreview = false;
                GameObject[] casted = AdminHallPreview.RaycastUtilities.UIRaycasts(
                    AdminHallPreview.RaycastUtilities.ScreenPosToPointerData(
                        new Vector2(i * tileSize + tileSize/2, j * tileSize + tileSize/4)));
                foreach (var c in casted)
                {
                    if (c.GetComponent<AdminHallPreview>())
                        isOverPreview = true;
                }

                if (isOverPreview)
                {
                    _startTilePos = new Vector2
                    (
                        i + 0.5f,
                        j + 0.25f
                    );
                    return;
                }
            }
        }
    }

    public void DeleteHall()
    {
        SelectTool(-1);
        TurnCanvasGroupTo(ref _confirmGroup, true);
    }

    public void DeleteHallConfirm()
    {
        StartCoroutine(DeleteHallQuery(_adminView.HallSelected.hnum));
    }

    private IEnumerator DeleteHallQuery(int hnum)
    {
        string phpFileName = "delete_hall.php";
        WWWForm data = new WWWForm();
        data.AddField("hnum", hnum);
        yield return _queriesToPhp.PostRequest(phpFileName, data, OnResponseCallback);
        if(_response == "Query completed")
        {
            ClearAll();
            _nameText.text = "";
            _adminView.HallSelected = new Hall();
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
        NpgsqlCommand dbcmd = AdminViewMode.dbcon.CreateCommand();
        string sql = "UPDATE options"
                     + " SET name = '" + _nameText.text + "'"
                     + ", is_hidden = " + _toggleHidden.isOn
                     + ", is_maintained = " + _toggleMaintained.isOn
                     + ", operation = 'UPDATE'"
                     + " WHERE onum = " + _adminView.HallSelected.hnum;
        dbcmd.Prepare();
        dbcmd.CommandText = sql;
        dbcmd.ExecuteNonQuery();

        for (int i = 0; i < _paintsParent.childCount; i++)
        {
            var c = _paintsParent.GetChild(i).GetComponent<Tile>().hallContent;
            c.hnum = _adminView.HallSelected.hnum;
            string sqlInsert = "INSERT INTO contents (onum, title, image_url, pos_x, pos_z, combined_pos, image_desc, type, operation)" +
                               " VALUES(" + c.hnum + ",'" + c.title + "','" + c.image_url + "'," + c.pos_x + ',' + c.pos_z + ",'" +
                               c.combined_pos + "','" + c.image_desc + "'," + c.type + ", 'INSERT')" +
                               " ON CONFLICT ON CONSTRAINT combined_pos_onum_unique DO UPDATE" +
                               " SET title = EXCLUDED.title, image_url = EXCLUDED.image_url, pos_x = EXCLUDED.pos_x, pos_z = EXCLUDED.pos_z, " +
                               "combined_pos = EXCLUDED.combined_pos, image_desc = EXCLUDED.image_desc, type = EXCLUDED.type, operation = 'UPDATE'";
            dbcmd.Prepare();
            dbcmd.CommandText = sqlInsert;
            dbcmd.ExecuteNonQuery();
        }

        foreach (var posDel in posToDelete)
        {
            string sqlDelete = "DELETE FROM contents"
                               + " WHERE combined_pos = '" + posDel.x + "_" + posDel.y + "'";
            dbcmd.Prepare();
            dbcmd.CommandText = sqlDelete;
            dbcmd.ExecuteNonQuery();
        }
    }

    private void SQLGetOptionsContents(int on)
    {
        NpgsqlCommand dbcmd = AdminViewMode.dbcon.CreateCommand();
        string sql =
            "SELECT * FROM public.contents c" +
            " WHERE c.onum = " + on;
        dbcmd.CommandText = sql;
        NpgsqlDataReader reader = dbcmd.ExecuteReader();
        float tileSize = _imagePreview.sizeDelta.x / _adminView.HallSelected.sizex;
        while (reader.Read())
        {
            int onum = (reader.IsDBNull(0)) ? 0 : reader.GetInt32(0);
            int cnum = (reader.IsDBNull(1)) ? 0 : reader.GetInt32(1);
            string title = (reader.IsDBNull(2)) ? "NULL" : reader.GetString(2);
            string image_url = (reader.IsDBNull(3)) ? "NULL" : reader.GetString(3);
            int pos_x = (reader.IsDBNull(4)) ? 0 : reader.GetInt32(4);
            int pos_z = (reader.IsDBNull(5)) ? 0 : reader.GetInt32(5);
            string combined_pos = (reader.IsDBNull(6)) ? "NULL" : reader.GetString(6);
            int type = (reader.IsDBNull(7)) ? 0 : reader.GetInt32(7);
            string image_desc = (reader.IsDBNull(8)) ? "NULL" : reader.GetString(8);
          
            HallContent newContent = new HallContent();
            newContent.hnum = onum;
            newContent.cnum = cnum;
            newContent.title = title;
            newContent.image_url = image_url;
            newContent.pos_x = pos_x;
            newContent.pos_z = pos_z;
            newContent.combined_pos = combined_pos;
            newContent.type = type;
            newContent.image_desc = image_desc;

            SelectTool(newContent.type);
            Vector2 tilePos = _startTilePos + new Vector2(newContent.pos_x, newContent.pos_z);
            Vector2 drawPos = new Vector2
            (
                tilePos.x * tileSize,
                tilePos.y * tileSize
            );
            Paint(tilePos, drawPos, true, newContent);
        }
        
        reader.Close();
        reader = null;
    }

    void Update()
    {
        if (_adminView.HallSelected.sizex == 0 || _adminView.HallSelected.sizez == 0)
            return;
        
        Vector2 windowSize = new Vector2(Screen.currentResolution.width, Screen.currentResolution.height);
        Vector2 absoluteMousePos = Input.mousePosition;
        float tileSize = _imagePreview.sizeDelta.x / _adminView.HallSelected.sizex;
        _cursorTile.sizeDelta = new Vector2(tileSize, tileSize);

        float addPosX = 0, addPosY = tileSize / 4;
        if(_adminView.HallSelected.sizez % 2 == 0)
            addPosY = -tileSize / 4;
        if (_adminView.HallSelected.sizex % 2 != 0)
            addPosX = tileSize / 2;

        _imagePreview.anchoredPosition = new Vector2
        (
            Mathf.FloorToInt((0.35f) * (windowSize.x / tileSize)) * tileSize + addPosX,
            Mathf.FloorToInt((0.55f) * (windowSize.y / tileSize)) * tileSize + addPosY
        );
        Vector2 tiledMousePos = new Vector2
        (
            Mathf.FloorToInt((absoluteMousePos.x / windowSize.x) * (windowSize.x / tileSize)) * tileSize + tileSize/2,
            Mathf.FloorToInt(((absoluteMousePos.y + tileSize / 4) / windowSize.y) * (windowSize.y / tileSize)) * tileSize + tileSize/4
        );

        bool turnToTrue = true;
        for (int i = 0; i < _paintsParent.childCount; i++)
        {
            Tile tileChange = _paintsParent.GetChild(i).GetComponent<Tile>();
            if (tileChange.hallContent.type == ExhibitsConstants.SpawnPoint.Id)
                turnToTrue = false;
        }
        _doorTool.interactable = turnToTrue;

        bool isOverPreview = false;
        GameObject[] casted = AdminHallPreview.RaycastUtilities.UIRaycasts(
            AdminHallPreview.RaycastUtilities.ScreenPosToPointerData(absoluteMousePos));
        foreach (var c in casted)
        {
            if (c.GetComponent<AdminHallPreview>())
                isOverPreview = true;
        }
        
        if(!_isCursorLock)
        {
            if (absoluteMousePos.x < 0.75f * windowSize.x && isOverPreview)
                _cursorTile.anchoredPosition = tiledMousePos;
            else
                _cursorTile.anchoredPosition = -windowSize;
        }
        
        if(_startTilePos == Vector2.zero)
        {
            _startTilePos = Vector2.one;
            _hallPlan = new int[_adminView.HallSelected.sizex][];
            for (int i = 0; i < _adminView.HallSelected.sizex; i++)
            {
                _hallPlan[i] = new int[_adminView.HallSelected.sizez];
                for (int j = 0; j < _adminView.HallSelected.sizez; j++)
                    _hallPlan[i][j] = -1;
            }
            posToDelete = new List<Vector2>();
            FindLeftBottomTile();
            SQLGetOptionsContents(_adminView.HallSelected.hnum);
        }
        
        if(_currentTool == ExhibitsConstants.SpawnPoint.Id 
           || _currentTool == ExhibitsConstants.Picture.Id
           || _currentTool == ExhibitsConstants.InfoBox.Id
           || _currentTool == ExhibitsConstants.Cup.Id
           || _currentTool == ExhibitsConstants.Medal.Id
           || _currentTool == ExhibitsConstants.Video.Id
           || _currentTool == ExhibitsConstants.Decoration.Id
           && _cursorTile.anchoredPosition.x > 1 && _changePropertiesGroup.alpha == 0)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Paint(tiledMousePos/tileSize, _cursorTile.anchoredPosition);
            }
        }
        if(_currentTool is -3 && _cursorTile.anchoredPosition.x > 1 && _changePropertiesGroup.alpha == 0)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Vector2 tiledPos = tiledMousePos / tileSize;
                Vector2 tileRealPos = new Vector2(Mathf.FloorToInt(tiledPos.x - _startTilePos.x),
                    Mathf.FloorToInt(tiledPos.y - _startTilePos.y));
           
                for (int i = 0; i < _paintsParent.childCount; i++)
                {
                    Tile tileChange = _paintsParent.GetChild(i).GetComponent<Tile>();
                    if(tileChange && tileChange.hallContent.pos_x == tileRealPos.x && tileChange.hallContent.pos_z == tileRealPos.y)
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
        if(_currentTool is -2 && _cursorTile.anchoredPosition.x > 1 && _changePropertiesGroup.alpha == 0)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Vector2 tiledPos = tiledMousePos / tileSize;
                _hallPlan[Mathf.FloorToInt(tiledPos.x - _startTilePos.x)][
                    Mathf.FloorToInt(tiledPos.y - _startTilePos.y)] = -1;
                for (int i = 0; i < _paintsParent.childCount; i++)
                {
                    Tile tileDelete = _paintsParent.GetChild(i).GetComponent<Tile>();
                    if(tileDelete && tileDelete.hallContent.pos_x == Mathf.FloorToInt(tiledPos.x - _startTilePos.x) 
                       && tileDelete.hallContent.pos_z == Mathf.FloorToInt(tiledPos.y - _startTilePos.y))
                    {
                        Debug.Log("Delete: " + i);
                        posToDelete.Add(new Vector2(tileDelete.hallContent.pos_x, tileDelete.hallContent.pos_z));
                        Destroy(tileDelete.gameObject);
                    }
                }
            }
        }
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
        if(_tileSelected.hallContent.type == ExhibitsConstants.Picture.Id
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
        if(Mathf.FloorToInt(tiledPos.x - _startTilePos.x) >= _hallPlan.Length || 
           (Mathf.FloorToInt(tiledPos.x - _startTilePos.x) < _hallPlan.Length 
            && Mathf.FloorToInt(tiledPos.y - _startTilePos.y) >= _hallPlan[Mathf.FloorToInt(tiledPos.x - _startTilePos.x)].Length))
        {
            return;
        }
        
        var newTile = Instantiate(_cursorTile.gameObject, _cursorTile.anchoredPosition, Quaternion.identity, _paintsParent);
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
        tileInstance.hallContent.combined_pos = tileInstance.hallContent.pos_x + "_" + tileInstance.hallContent.pos_z;
        
        if(tileInstance.hallContent.type == ExhibitsConstants.Picture.Id
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
        else if (_isDoorBlock && _currentTool == ExhibitsConstants.SpawnPoint.Id)
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
