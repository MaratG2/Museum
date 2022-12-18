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
    public Dictionary<int, RoomDto> CachedRooms;
    public List<Hall> CachedHallsInfo;
    private HallQueriesAsync _hallQueriesAsync = new();
    private HallQueries _hallQueries = new();
    private WWWForm form;
    private readonly string baseRoute = "https://istu-museum-admin.netlify.app/api/PHP/";

    /*private void OnEnable()
    {
        _hallQueries.OnAllHallsGet += halls => CachedHallsInfo = halls;
    }*/
    public void Awake()
    {
        form = new WWWForm();
    }

    public async void Start()
    {
        await RefreshHallsInfo().ConfigureAwait(false);
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