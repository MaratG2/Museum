using System.Collections;
using System.Collections.Generic;
using Admin.Auth;
using Admin.Edit;
using Admin.PHP;
using Admin.Utility;
using UnityEngine;
using UnityEngine.UI;

namespace Admin.View
{
    /// <summary>
    /// Отвечает за выбор зала. 
    /// </summary>
    /// <remarks>
    /// Хранит в себе публичное свойство HallSelected, которое используется множеством других классов.
    /// </remarks>
    public class HallViewer : MonoBehaviour
    {
        [SerializeField] private GameObject _textGORefreshing;
        [SerializeField] private Button _modeSwitchEdit;
        [SerializeField] private Button _modeSwitchNew;
        [SerializeField] private Button _mode3DView;
        private HallQueries _hallQueries = new();
        private Hall _hallSelected;
        private List<Hall> _cachedHalls;
        private TilesDrawer _tilesDrawer;
        private HallLister _hallLister;

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
            _hallLister = GetComponent<HallLister>();
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
            
            _mode3DView.interactable = !string.IsNullOrWhiteSpace(_hallSelected.name);
        }

        public void SelectHall(int hnum)
        {
            Hall current = new Hall();
            bool hasFound = false;
            foreach (var cho in _cachedHalls)
                if (cho.hnum == hnum)
                {
                    current = cho;
                    hasFound = true;
                }

            if (!hasFound)
            {
                Debug.LogError("NOT FOUND OPTION BY THAT ONUM");
                return;
            }

            if (_hallSelected.name != current.name)
            {
                _tilesDrawer.ClearAllTiles();
                FindObjectOfType<HallEditor>().ClearAll();
            }

            _hallSelected = current;
            ModeSwitchChangeActive();
            
            StartCoroutine(_tilesDrawer.DrawTilesForHall(_hallSelected));
        }

        private void ModeSwitchChangeActive()
        {
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
        }

        public void Refresh()
        {
            ResetVariables();
            StartCoroutine(CreateHallListings());
        }

        private void ResetVariables()
        {
            HallSelected = new Hall();
            _hallLister.ClearAllHallListings();
            _tilesDrawer.ClearAllTiles();
            _tilesDrawer.SetPreviewState(false);
            _textGORefreshing.SetActive(true);
            _modeSwitchEdit.gameObject.SetActive(false);
            _modeSwitchNew.gameObject.SetActive(true);
        }

        private IEnumerator CreateHallListings()
        {
            _cachedHalls = new List<Hall>();
            yield return _hallQueries.GetAllHalls();
            _hallLister.CreateAllHallListings(_cachedHalls, SelectHall);
            _textGORefreshing.SetActive(false);
        }
    }
}