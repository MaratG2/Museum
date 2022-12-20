using System;
using System.Collections;
using System.Collections.Generic;
using Admin.Auth;
using Admin.PHP;
using Admin.Utility;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Admin.View
{
    public class AdminViewMode : MonoBehaviour
    {
        [SerializeField] private GameObject _textGORefreshing;
        [SerializeField] private Button _hallListingPrefab;
        [SerializeField] private RectTransform _hallListingsParent;
        [SerializeField] private Button _modeSwitchEdit;
        [SerializeField] private Button _modeSwitchNew;
        private HallQueries _hallQueries = new();
        private Hall _hallSelected;
        private List<Hall> _cachedHalls;
        private TilesDrawer _tilesDrawer;

        public Hall HallSelected
        {
            get => _hallSelected;
            set => _hallSelected = value;
        }

        private void OnEnable()
        {
            _hallQueries.OnAllHallsGet += halls => _cachedHalls = halls;
        }

        private void OnDisable()
        {
            _hallQueries.OnAllHallsGet -= halls => _cachedHalls = halls;
        }

        private void Awake()
        {
            _tilesDrawer = GetComponent<TilesDrawer>();
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

        public void SelectHall(int hnum)
        {
            Hall current = new Hall();
            bool hasFound = false;
            foreach (var cho in _cachedHalls)
            {
                if (cho.hnum == hnum)
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
                _tilesDrawer.ClearAllTiles();
                FindObjectOfType<AdminEditMode>().ClearAll();
            }

            _hallSelected = current;
            var user = FindObjectOfType<Login>().CurrentUser;
            if (user.access_level == AccessLevel.Editor)
            {
                if (_hallSelected.author == user.email)
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

            StartCoroutine(_tilesDrawer.DrawTilesForHall(_hallSelected));
        }

        public void Refresh()
        {
            for (int i = 0; i < _hallListingsParent.childCount; i++)
                Destroy(_hallListingsParent.GetChild(i).gameObject);
            HallSelected = new Hall();
            _tilesDrawer.ClearAllTiles();
            _tilesDrawer.SetPreviewState(false);
            _textGORefreshing.SetActive(true);
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
    }
}