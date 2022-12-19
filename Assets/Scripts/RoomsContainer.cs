using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Admin.PHP;
using Admin.Utility;
using GenerationMap;
using UnityEngine;
using UnityEngine.Networking;

public class RoomsContainer : MonoBehaviour
{
    public Dictionary<int, RoomDto> CachedRooms { get; private set; }
    public List<Hall> CachedHallsInfo { get; private set; }
    private HallQueriesAsync _hallQueriesAsync = new();
    private HallQueries _hallQueries = new();
    private WWWForm form;
    private readonly string baseRoute = "https://istu-museum-admin.netlify.app/api/PHP/";

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
        {
            CachedRooms[newContents[0].hnum].Contents = newContents;
        }
    }
    
    public void Awake()
    {
        StartCoroutine(InitializeRoomsContainer());
        form = new WWWForm();
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

    public async void Start()
    {
        //await RefreshHallsInfo().ConfigureAwait(false);
    }

    public async Task RefreshHallsInfo()
    {
        CachedHallsInfo = await _hallQueriesAsync.GetAllHalls().ConfigureAwait(false);
        var rooms = new Dictionary<int, RoomDto>();
        foreach (var hallInfo in CachedHallsInfo)
        {
            var tempRoomDto = await GetRoomInfo(hallInfo).ConfigureAwait(false);
            rooms.Add(hallInfo.hnum, tempRoomDto);
        }

        Debug.Log(CachedHallsInfo.Count);
        CachedRooms = rooms;
    }

    private async Task<RoomDto> GetRoomInfo(Hall hallOptions)
    {
        var contents = await _hallQueriesAsync.GetAllContentsByHnumAsync(hallOptions.hnum,form).ConfigureAwait(false);

        var roomInfo = new RoomDto()
        {
            Contents = contents,
            HallOptions = hallOptions
        };
        return roomInfo;
    }
}