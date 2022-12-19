using System.Collections.Generic;
using GenerationMap;
using UnityEngine;
using UnityEngine.UI;

public class RoomsMenu : MonoBehaviour
{

    [SerializeField] private Canvas canvas;
    [SerializeField] private Font font;
    [SerializeField] private Button next;
    [SerializeField] private Button back;
    [SerializeField] private Button goToHall;
    [SerializeField] private Text goToHallText;
    [SerializeField] private RoomsContainer _roomsContainer;

    [SerializeField] private DataConverter converter;
    [SerializeField] private GameObject player;
    private List<GameObject> buttons;
    private int currentHall = 0;
    
    
    public void Start()
    {
        if(_roomsContainer.CachedHallsInfo.Count == 0)
            return;
        goToHallText.text = _roomsContainer.CachedHallsInfo[currentHall].name;
        
    }

    public void NextHall()
    {
        if (currentHall == _roomsContainer.CachedHallsInfo.Count - 1)
            currentHall = 0;
        else currentHall++;
        goToHallText.text = _roomsContainer.CachedHallsInfo[currentHall].name;
        
        foreach (var info in _roomsContainer.CachedHallsInfo)
        {
            Debug.Log($"{info.name} {info.sizex} {info.sizez}");
        }
    }
    
    public void PreviewHall()
    {
        if (currentHall == 0)
            currentHall = _roomsContainer.CachedHallsInfo.Count - 1;
        else
            currentHall--;
        goToHallText.text = _roomsContainer.CachedHallsInfo[currentHall].name;
    }

    public void LoadHall()
    {
        var room = converter.GetRoomByRoomDto(_roomsContainer.CachedRooms[currentHall]);
        converter.GenerateRoomWithContens(room);
        var posForSpawn = room.GetSpawnPosition();
        player.transform.position = posForSpawn;
    }


    /*private void OnEnable()
    {
        var rooms = GetAllowsRooms();
        buttons = CreateButtons(rooms);
    }*/

    /*private async Task<List<GameObject>> CreateButtons(List<Hall> rooms)
    {
        return rooms
            .Select(async x=>await CreateButton(x).ConfigureAwait(false))
            .ToList();
    }*/

    /*private async Task<GameObject> CreateButton (Hall room)
    {
        var newButton = new GameObject($"button: {room.name}", typeof(Image), typeof(Button), typeof(LayoutElement));
        newButton.transform.SetParent(gameObject.transform);
        newButton.GetComponent<LayoutElement>().minHeight = 35;
        newButton.GetComponent<LayoutElement>().minWidth = 100;
        
        
        var newText = new GameObject($"text: {room.name}", typeof(Text));
        newText.transform.SetParent(newButton.transform);
        newText.GetComponent<Text>().text = $"{room.name}";
        newText.GetComponent<Text>().font = font;
        var rt = newText.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(0, 0);
        rt.anchorMax = new Vector2(1, 1);
        rt.anchoredPosition = new Vector2(0, 0);
        rt.sizeDelta = new Vector2(0, 0);
        newText.GetComponent<Text>().color = new Color(0, 0, 0);
        newText.GetComponent<Text>().alignment = TextAnchor.MiddleCenter;
        newButton.GetComponent<Button>().onClick.AddListener(delegate { await GetActionPressButton(room); });
        return newButton;
    }*/

    /*public async Task GetActionPressButton(Hall hall)
    {
        var pos = await converter.GenerateRoomByOnum(hall).ConfigureAwait(false);
        player.transform.position = pos;
        foreach (var t in buttons)
        {
            Destroy(t);
        }
        buttons.Clear();
    }*/
}
