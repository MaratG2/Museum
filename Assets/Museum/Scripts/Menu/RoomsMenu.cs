using Museum.Scripts.GenerationMap;
using TMPro;
using UnityEngine;

namespace Museum.Scripts.Menu
{
    public class RoomsMenu : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI goToHallText;
        [SerializeField] private RoomsContainer roomsContainer;

        [SerializeField] private GenerationConnector converter;
        [SerializeField] private GameObject player;
        private Vector3 _startPosPlayer;
        private int _currentHall;
    
    
        public void Start()
        {
            _startPosPlayer = player.transform.position;
            if(roomsContainer.CachedHallsInfo.Count == 0)
                return;
            goToHallText.text = roomsContainer.CachedHallsInfo[_currentHall].name;
        
        }

        public void NextHall()
        {
            if (_currentHall == roomsContainer.CachedHallsInfo.Count - 1)
                _currentHall = 0;
            else _currentHall++;
            goToHallText.text = roomsContainer.CachedHallsInfo[_currentHall].name;
        }
    
        public void PreviewHall()
        {
            if (_currentHall == 0)
                _currentHall = roomsContainer.CachedHallsInfo.Count - 1;
            else
                _currentHall--;
            goToHallText.text = roomsContainer.CachedHallsInfo[_currentHall].name;
        }

        public void LoadHall()
        {
            var room = converter.GetRoomByRoomDto(roomsContainer.CachedRooms[roomsContainer.CachedHallsInfo[_currentHall].hnum]);
            converter.GenerateRoomWithContens(room);
            var posForSpawn = room.GetSpawnPosition();
            player.transform.position = posForSpawn;
        }
    
        public void BackToMainRoom()
        {
            player.transform.position = _startPosPlayer;
        }
    }
}
