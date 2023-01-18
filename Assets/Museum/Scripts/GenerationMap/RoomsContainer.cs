using System.Collections;
using System.Collections.Generic;
using Admin.PHP;
using Admin.Utility;
using GenerationMap;
using UnityEngine;

namespace Museum.Scripts.GenerationMap
{
    /// <summary>
    /// Отвечает за получение информации о залах с сервера и хранение их.
    /// </summary>
    public class RoomsContainer : MonoBehaviour
    {
        public Dictionary<int, RoomDto> CachedRooms { get; private set; }
        public List<Hall> CachedHallsInfo { get; private set; }
        private HallQueries _hallQueries = new();

        private void OnEnable()
        {
            _hallQueries.OnAllHallsGet += halls => CachedHallsInfo = halls;
            _hallQueries.OnAllHallContentsGet += AddToCachedRooms;
        }
    
        private void OnDisable()
        {
            _hallQueries.OnAllHallsGet -= halls => CachedHallsInfo = halls;
            _hallQueries.OnAllHallContentsGet -= AddToCachedRooms;
        }

        private void AddToCachedRooms(List<HallContent> newContents)
        {
            if (newContents.Count > 0)
                CachedRooms[newContents[0].hnum].Contents = newContents;
        }
    
        public void Awake()
        {
            StartCoroutine(InitializeRoomsContainer());
        }

        private IEnumerator InitializeRoomsContainer()
        {
            CachedRooms = new Dictionary<int, RoomDto>();
            CachedHallsInfo = new List<Hall>();
            yield return _hallQueries.GetAllHalls();
            foreach (var hall in CachedHallsInfo)
            {
                var newRoomDto = new RoomDto();
                newRoomDto.HallOptions = hall;
                newRoomDto.Contents = new List<HallContent>();
                CachedRooms.Add(hall.hnum, newRoomDto);
                yield return _hallQueries.GetAllContentsByHnum(hall.hnum);
            }
        }
    }
}