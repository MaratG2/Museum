using Museum.Scripts.GenerationMap;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Museum.Scripts.Menu
{
    public class RoomsMenu : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI goToHallText;
        [SerializeField] private RoomsContainer roomsContainer;

        [SerializeField] private GenerationConnector converter;
        [SerializeField] private GameObject player;
        [SerializeField] private Button _hallButton;
        [SerializeField] private Button _nextButton;
        [SerializeField] private Button _previousButton;
        private Vector3 _startPosPlayer;
        private int _currentHall;
    
    
        public void Start()
        {
            _startPosPlayer = player.transform.position;
            if(roomsContainer.CachedPublicHallsInfo.Count == 0)
            {
                _hallButton.interactable = false;
                _nextButton.interactable = false;
                _previousButton.interactable = false;
                return;
            }
            string prefix = roomsContainer.CachedPublicHallsInfo[_currentHall].is_maintained
                ? "(в работе) " : "";
            _hallButton.interactable = prefix.Length <= 1;
            goToHallText.text = prefix + roomsContainer.CachedPublicHallsInfo[_currentHall].name;
        
        }

        public void NextHall()
        {
            if(roomsContainer.CachedPublicHallsInfo.Count == 0)
                return;
            if (_currentHall == roomsContainer.CachedPublicHallsInfo.Count - 1)
                _currentHall = 0;
            else _currentHall++;
            string prefix = roomsContainer.CachedPublicHallsInfo[_currentHall].is_maintained
                ? "(в работе) " : "";
            _hallButton.interactable = prefix.Length <= 1;
            goToHallText.text = prefix + roomsContainer.CachedPublicHallsInfo[_currentHall].name;
        }
    
        public void PreviewHall()
        {
            if(roomsContainer.CachedPublicHallsInfo.Count == 0)
                return;
            if (_currentHall == 0)
                _currentHall = roomsContainer.CachedPublicHallsInfo.Count - 1;
            else
                _currentHall--;
            string prefix = roomsContainer.CachedPublicHallsInfo[_currentHall].is_maintained 
                ? "(в работе) " : "";
            _hallButton.interactable = prefix.Length <= 1;
            goToHallText.text = prefix + roomsContainer.CachedPublicHallsInfo[_currentHall].name;
        }

        public void LoadHall()
        {
            var room = converter.GetRoomByRoomDto(roomsContainer
                .CachedRooms[roomsContainer.CachedPublicHallsInfo[_currentHall].hnum]);
            converter.GenerateRoomWithContens(room);
            var posForSpawn = room.GetSpawnPosition();
            player.transform.position = posForSpawn;
        }
    
        public void BackToMainRoom()
        {
            Menu.Instance.ActivateRoomMenu();
            player.transform.position = _startPosPlayer;
        }
    }
}
