using System;
using System.Collections;
using System.Collections.Generic;
using Admin.Auth;
using Admin.PHP;
using Admin.Utility;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AdminViewMode : MonoBehaviour
{
    [SerializeField] private GameObject _hallPreview;
    [SerializeField] private GameObject _textGORefreshing;
    [SerializeField] private Button _hallListingPrefab;
    [SerializeField] private RectTransform _hallListingsParent;
    [SerializeField] private GameObject _tilePrefab;
    [SerializeField] private GameObject _tilesParent;
    [SerializeField] private Button _modeSwitchEdit;
    [SerializeField] private Button _modeSwitchNew;
    private HallQueries _hallQueries = new ();
    private Hall _hallSelected;
    private List<Hall> _cachedHalls;
    private Vector2 _startTilePos;
    private List<HallContent> _currentHallContents;
    
    public Hall HallSelected
    {
        get => _hallSelected;
        set => _hallSelected = value;
    }

    private void OnEnable()
    {
        _hallQueries.OnAllHallContentsGet += hallContents => _currentHallContents = hallContents;
        _hallQueries.OnAllHallsGet += halls => _cachedHalls = halls;
    }

    private void OnDisable()
    {
        _hallQueries.OnAllHallContentsGet -= hallContents => _currentHallContents = hallContents;
        _hallQueries.OnAllHallsGet -= halls => _cachedHalls = halls;
    }

    private void Start()
    {
        _modeSwitchEdit.gameObject.SetActive(false);
        _modeSwitchNew.gameObject.SetActive(true);
        Refresh();
    }

    private void Update()
    {
        if (FindObjectOfType<Login>().CurrentUser.access_level == AccessLevel.Guest)
        {
            _modeSwitchEdit.gameObject.SetActive(false);
            _modeSwitchNew.gameObject.SetActive(false);
        }
    }

    public void SelectHall(int num)
    {
        Hall current = new Hall();
        bool hasFound = false;
        foreach (var cho in _cachedHalls)
        {
            if (cho.hnum == num)
            {
                current = cho;
                hasFound = true;
            }
        }

        if (!hasFound)
        {
            Debug.LogError("NOT FOUND OPTION BY THAT ONUM");
            return;
        }
        
        if (_hallSelected.name != current.name)
        {
            FindObjectOfType<AdminEditMode>().ClearAll();
            for (int i = 0; i < _tilesParent.transform.childCount; i++)
                Destroy(_tilesParent.transform.GetChild(i).gameObject);
        }

        _hallSelected = current;
        var user = FindObjectOfType<Login>().CurrentUser;
        if(user.access_level == AccessLevel.Editor)
        {
            if(_hallSelected.author == user.email)
            {
                _modeSwitchEdit.gameObject.SetActive(true);
                _modeSwitchNew.gameObject.SetActive(false);
            }
            else
            {
                _modeSwitchEdit.gameObject.SetActive(false);
                _modeSwitchNew.gameObject.SetActive(true);
            }
        }
        else
        {
            _modeSwitchEdit.gameObject.SetActive(true);
            _modeSwitchNew.gameObject.SetActive(false);
        }
        StartCoroutine(FindLeftBottomTile(num));
    }

    private void Paint(Vector2 pos, HallContent content)
    {
        var newTile = Instantiate(_tilePrefab.gameObject, Vector2.zero, Quaternion.identity,
            _tilesParent.GetComponent<RectTransform>());
        newTile.GetComponent<RectTransform>().anchorMin = Vector2.zero;
        newTile.GetComponent<RectTransform>().anchorMax = Vector2.zero;
        newTile.GetComponent<RectTransform>().anchoredPosition = pos;
        newTile.GetComponent<Tile>().hallContent = content;
        newTile.GetComponent<Tile>().Setup();
        float tileSize = _hallPreview.GetComponent<RectTransform>().sizeDelta.x / HallSelected.sizex;
        newTile.GetComponent<RectTransform>().sizeDelta = new Vector2(tileSize, tileSize);
    }

    public void Refresh()
    {
        for (int i = 0; i < _hallListingsParent.childCount; i++)
            Destroy(_hallListingsParent.GetChild(i).gameObject);
        HallSelected = new Hall();
        for (int i = 0; i < _tilesParent.transform.childCount; i++)
            Destroy(_tilesParent.transform.GetChild(i).gameObject);
        _textGORefreshing.SetActive(true);
        _hallPreview.SetActive(false);
        _modeSwitchEdit.gameObject.SetActive(false);
        _modeSwitchNew.gameObject.SetActive(true);
        StartCoroutine(InitializeAllHalls());
    }

    private IEnumerator InitializeAllHalls()
    {
        _cachedHalls = new List<Hall>();

        yield return _hallQueries.GetAllHalls();
        CreateAllHallListings();

        _textGORefreshing.SetActive(false);
        _hallPreview.SetActive(true);
    }
    
    private void CreateAllHallListings()
    {
        foreach (var hall in _cachedHalls)
        {
            var newInstance = Instantiate(_hallListingPrefab, Vector3.zero, Quaternion.identity,
                _hallListingsParent);
            newInstance.gameObject.name = hall.hnum + " - " + hall.name;
            newInstance.GetComponentInChildren<TextMeshProUGUI>().text = hall.name;
            newInstance.onClick.AddListener(() => SelectHallFromButton(hall.hnum, newInstance.gameObject));
        }
        
    }

    private void SelectHallFromButton(int onum, GameObject linkGO)
    {
        for (int i = 0; i < _hallListingsParent.transform.childCount; i++)
        {
            if (_hallListingsParent.GetChild(i).gameObject == linkGO)
            {
                _hallListingsParent.GetChild(i).GetComponent<Image>().color = Color.green;
                _hallListingsParent.GetChild(i).GetComponentInChildren<TextMeshProUGUI>().color = Color.black; 
            }
            else
            {
                _hallListingsParent.GetChild(i).GetComponent<Image>().color = Color.gray;
                _hallListingsParent.GetChild(i).GetComponentInChildren<TextMeshProUGUI>().color = Color.white; 
            }
        }
        SelectHall(onum);
    }

    private void DrawTilesForGotHall()
    {
        if (_currentHallContents == null)
            return;
        float tileSize = _hallPreview.GetComponent<RectTransform>().sizeDelta.x / HallSelected.sizex;
        foreach (var hallContent in _currentHallContents)
        {
            if (hallContent.hnum == 0)
                continue;
            
            Vector2 tilePos = _startTilePos + new Vector2(hallContent.pos_x, hallContent.pos_z);
            Vector2 drawPos = new Vector2
            (
                tilePos.x * tileSize,
                tilePos.y * tileSize
            );
            Paint(drawPos, hallContent);
        }
    }
    
    private IEnumerator FindLeftBottomTile(int num)
    {
        float tileSize = 0f;
        while (tileSize.Equals(0f))
        {
            tileSize = _hallPreview.GetComponent<RectTransform>().sizeDelta.x / HallSelected.sizex;
            yield return new WaitForEndOfFrame();
        }
        for(int i = 0; i < 1920 / tileSize; i++)
        {
            for (int j = 0; j < 1080 / tileSize; j++)
            {
                if (CheckIsOverPreview(tileSize, i, j))
                {
                    _startTilePos = new Vector2
                    (
                        i + 0.5f,
                        j + 0.25f
                    );
                    yield return _hallQueries.GetAllContentsByHnum(num);
                    DrawTilesForGotHall();
                    yield break;
                }
            }
        }
    }
    private bool CheckIsOverPreview(float tileSize, int i, int j)
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

        return isOverPreview;
    }
}
