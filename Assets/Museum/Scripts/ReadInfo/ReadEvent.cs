using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

#pragma warning disable 0649
public class ReadEvent : MonoBehaviour
{
    #region Singleton

    private static ReadEvent _instance;
    public static ReadEvent Instance
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

    private void Awake()
    {
        _instance = gameObject.GetComponent<ReadEvent>();
        SetPanelProperty();
    }

    public GameObject goForm;
    [SerializeField] private GameObject goScroll;
    public TMP_FontAsset Font;
    public GameObject PicturePanel;
    [HideInInspector]
    public List<ReadObject> ListObjects = new List<ReadObject>();
    [HideInInspector]
    public List<ReadFile> ListFile = new List<ReadFile>();
    [HideInInspector]
    public static float width;
    [HideInInspector]
    public static float height;

    [HideInInspector]
    public static float SpaceFortext;
    [HideInInspector]
    public static float SpaceBetweenEl;

    float yValueForScroll = 0f;



    void SetPanelProperty()
    {
        width = Screen.width * 0.8f;
        height = width / 16f * 10f;

        SpaceFortext = width * 0.05f;
        SpaceBetweenEl = width * 0.04f;
    }



    float NextPoint(RectTransform x1, RectTransform x2)
    {
        return (-x1.sizeDelta.y / 2) - SpaceBetweenEl + x1.localPosition.y + (-x2.sizeDelta.y / 2);
    }

    public IEnumerator SetForm()
    {
        goScroll.GetComponent<RectTransform>().sizeDelta = new Vector2(width, 0);
        CreatObjects();
        yield return null; // Для постановки системой подходящей высоты
        SetPostion();
        
        goForm.GetComponent<RectTransform>().sizeDelta = new Vector2(width, yValueForScroll);
        goForm.GetComponent<RectTransform>().localPosition = new Vector3(0f, -yValueForScroll / 2 + SpaceBetweenEl, 0f);
    }

    void SetPostion()
    {      

        for (var i = 0; i < ListObjects.Count; i++)
        {

            ListObjects[i].SetYValue();            
            yValueForScroll += ListObjects[i].RectTran.sizeDelta.y+ SpaceBetweenEl;

        }
        
        ListObjects[0].RectTran.localPosition = new Vector3(0, yValueForScroll/2- ListObjects[0].RectTran.sizeDelta.y/2, 0);
        
        for (var i = 1; i < ListObjects.Count; i++)
        {           
            ListObjects[i].RectTran.localPosition = new Vector3(0, NextPoint(ListObjects[i - 1].RectTran, ListObjects[i].RectTran), 0);
        }
    }
    public void CreatObjects()
    {
        foreach (var i in ListFile)
        {
            ListObjects.Add(i.ToReadObject());
        }
    }
    public void DestroyList()
    {
        foreach (var k in ListObjects)
            Destroy(k.TypedObject);
        yValueForScroll = 0f;
        ListObjects.Clear();
    }
}

#pragma warning restore 0649