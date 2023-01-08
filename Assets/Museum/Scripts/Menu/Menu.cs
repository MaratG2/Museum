using System;
using System.Collections.Generic;
using UnityEngine;
#pragma warning disable 0649
public class Menu : MonoBehaviour
{
    #region Singlton
    public static Menu _instance;
    public static Menu Instance
    {
        get
        {
            if (_instance == null)
            {
                print("Instance");
            }
            return _instance;
        }
    }
    #endregion
    bool flag;
    [SerializeField]
    GameObject obj;

    [SerializeField]
    public GameObject roomsMenu;
    
    [SerializeField]
    List<GameObject> MustBeClosed = new();
    private void Awake()
    {
        _instance = GetComponent<Menu>();
    }

    public void Activate()
    {
        if (!State.Frozen || flag)
        {
            State.View();
            obj.SetActive(!obj.activeInHierarchy);
            flag = !flag;               
            foreach (var i in MustBeClosed)
                i.SetActive(false);
        }
    }
    
    public void ActivateRoomMenu()
    {
        if (!State.Frozen || flag)
        {
            State.View();
            roomsMenu.SetActive(!roomsMenu.activeInHierarchy);
            flag = !flag;               
            foreach (var i in MustBeClosed)
                i.SetActive(false);
        }
    }
}
#pragma warning restore 0649