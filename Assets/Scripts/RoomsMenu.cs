using System;
using System.Collections.Generic;
using System.Linq;
using GenerationMap;
using UnityEngine;
using UnityEngine.UI;

public class RoomsMenu : MonoBehaviour
{

    [SerializeField] private Canvas canvas;
    [SerializeField] private Font font;
    [SerializeField] private DataConverter converter;
    [SerializeField] private GameObject player;
    private List<GameObject> buttons; 

    private void OnEnable()
    {
        var rooms = GetAllowsRooms();
        Debug.Log(rooms.Count);
        buttons = CreateButtons(rooms);
    }
    private List<GameObject> CreateButtons(List<RoomInfo> rooms)
    {
        return rooms
            .Select(CreateButton)
            .ToList();
    }

    private GameObject CreateButton (RoomInfo room)
    {
        var newButton = new GameObject($"button: {room.Name}", typeof(Image), typeof(Button), typeof(LayoutElement));
        newButton.transform.SetParent(gameObject.transform);
        newButton.GetComponent<LayoutElement>().minHeight = 35;
        newButton.GetComponent<LayoutElement>().minHeight = 100;
        
        
        var newText = new GameObject($"text: {room.Name}", typeof(Text));
        newText.transform.SetParent(newButton.transform);
        newText.GetComponent<Text>().text = $"{room.Name}";
        newText.GetComponent<Text>().font = font;
        var rt = newText.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(0, 0);
        rt.anchorMax = new Vector2(1, 1);
        rt.anchoredPosition = new Vector2(0, 0);
        rt.sizeDelta = new Vector2(0, 0);
        newText.GetComponent<Text>().color = new Color(0, 0, 0);
        newText.GetComponent<Text>().alignment = TextAnchor.MiddleCenter;
        newButton.GetComponent<Button>().onClick.AddListener(delegate { GetActionPressButton(room.Id); });
        return newButton;
    }

    public void GetActionPressButton(int num)
    {
        var pos = converter.GenerateRoomByOnum(num);
        player.transform.position = pos;
        foreach (var t in buttons)
        {
            Destroy(t);
        }
        buttons.Clear();
    }


    private List<RoomInfo> GetAllowsRooms()
    {
        var roomsRaw = ConnectionDb.GetAllOptions();
        return roomsRaw
            .Select(ConvertRoom)
            /*.Where(x => x.IsAvailable)*/
            .ToList();
    }

    private RoomInfo ConvertRoom(AdminNewMode.HallOptions rawData)
    {
        var roomInfo = new RoomInfo()
        {
            Id = rawData.onum,
            Name = rawData.name,
            IsAvailable = rawData.is_date_b && !rawData.is_date_e && !rawData.is_hidden
        };
        return roomInfo;
    }


}
