using System;
using System.Collections.Generic;
using Admin.Utility;
using Museum.Scripts.GenerationMap;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

namespace Museum.Scripts.Menu
{
    public class RoomsMenu : MonoBehaviour
    {
        [SerializeField] private RoomsContainer roomsContainer;
        [SerializeField] private GenerationConnector converter;
        [SerializeField] private GameObject player;
        [SerializeField] private Transform _hallsParent;
        [SerializeField] private HallListing _hallInListPrefab;
        [SerializeField] private Button _returnHallButton;
        [SerializeField] private Button _enterHallButton;
        [SerializeField] private Color _activeColor;
        [SerializeField] private Color _inactiveColor;
        private List<HallListing> _hallListings;
        private HallListing _hallSelected;
        private Vector3 _startPosPlayer;
        private int _currentHall;

        private void Start()
        {
            _startPosPlayer = player.transform.position;
        }
        
        private void OnEnable()
        {
            _hallListings = new List<HallListing>();
            _hallSelected = null;
            for(int i = _hallsParent.childCount - 1; i >= 0; i--)
                Destroy(_hallsParent.GetChild(i).gameObject);
            _enterHallButton.interactable = false;
            _returnHallButton.interactable = true;
            SpawnHallList();
            SortHallList();
        }

        private void SpawnHallList()
        {
            foreach (var hallInfo in roomsContainer.CachedPublicHallsInfo)
            {
                var newHallListing = Instantiate(_hallInListPrefab, Vector3.zero, Quaternion.identity, _hallsParent);
                newHallListing.Setup(hallInfo, this);
                _hallListings.Add(newHallListing);
            }
        }

        private void SortHallList()
        {
            for (int i = 0; i < _hallListings.Count; i++)
            {
                if(_hallListings[i].IsActive())
                    _hallListings[i].transform.SetAsFirstSibling();
            }
        }

        public void SelectHall(HallListing hallListingSelected)
        {
            this._hallSelected = hallListingSelected;
            _enterHallButton.interactable = true;

            for (int i = 0; i < _hallsParent.childCount; i++)
            {
                if (_hallsParent.GetChild(i).gameObject == _hallSelected.gameObject)
                {
                    _hallsParent.GetChild(i).GetComponent<Image>().color = _activeColor;
                }
                else
                {
                    _hallsParent.GetChild(i).GetComponent<Image>().color = _inactiveColor;
                }
            }
        }
        
        public void LoadHall()
        {
            var room = converter.GetRoomByRoomDto(roomsContainer.CachedRooms[_hallSelected.GetHNum()]);
            converter.GenerateRoomWithContens(room);
            var posForSpawn = room.GetSpawnPosition();
            player.transform.position = posForSpawn;
            _enterHallButton.interactable = false;
            Menu.Instance.ActivateRoomMenu();
        }
    
        public void BackToMainRoom()
        {
            var videos = FindObjectsOfType<VideoPlayer>();
            foreach (var video in videos)
                video.Stop();
            Menu.Instance.ActivateRoomMenu();
            player.transform.position = _startPosPlayer;
        }
    }
}
