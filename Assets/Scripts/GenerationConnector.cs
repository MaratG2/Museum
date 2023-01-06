using System.Collections;
using System.Collections.Generic;
using Admin.PHP;
using Admin.Utility;
using GenerationMap;
using UnityEngine;

public class GenerationConnector : MonoBehaviour
{
    public RoomDto CachedRoom { get; } = new();
    
    private HallQueries _hallQueries = new();

    private void OnEnable()
    {
        _hallQueries.OnHallGet += hall => CachedRoom.HallOptions = hall;
        _hallQueries.OnAllHallContentsGet += contents => CachedRoom.Contents = contents;
    }
    
    private void OnDisable()
    {
        _hallQueries.OnHallGet -= hall => CachedRoom.HallOptions = hall;
        _hallQueries.OnAllHallContentsGet -= contents => CachedRoom.Contents = contents;
    }

    public RoomDto GetRoomDtoByHnum(int hnum)
    {
        InitializeHallInfo(hnum);
        return CachedRoom;
    }

    public void InitializeHallInfo(int hnum)
    {
        StartCoroutine(InitializeRoomsContainer(hnum));
    }

    private IEnumerator InitializeRoomsContainer(int hnum)
    {
        yield return _hallQueries.GetHallByHnum(hnum);
        yield return _hallQueries.GetAllContentsByHnum(hnum);
    }
}
