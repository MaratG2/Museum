
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public enum TypeObj { Image,Title, Text };
public class ReadObject
{
    
    public GameObject TypedObject = null;
    public RectTransform RectTran = null;
    TextMeshProUGUI TextP = null;
    TypeObj t = TypeObj.Image;
    public TextMeshProUGUI TextParametrs
    {
        get
        {
            if (t != 0)
                return TextP;
            else return null;
        }
        private set
        {
            TextP = value;
        }
    }

    public ReadObject(Sprite image)
    {
        TypedObject =new GameObject();
        TypedObject.transform.SetParent(ReadEvent.Instance.goForm.transform);
        RectTran = TypedObject.AddComponent<RectTransform>();
        TypedObject.AddComponent<Image>();
        TypedObject.GetComponent<Image>().sprite = image;
        TypedObject.GetComponent<Image>().raycastTarget = false;
        RectTran.sizeDelta = new Vector2(ReadEvent.width, ReadEvent.height);

        RectTran.localPosition = new Vector3(100f, 100f, 100f); //Делается для красивого появления
    }
    public ReadObject(string text, TypeObj t)
    {
        this.t = t;

        TypedObject = new GameObject();
        TypedObject.transform.SetParent(ReadEvent.Instance.goForm.transform);
        RectTran = TypedObject.AddComponent<RectTransform>();
        TextParametrs = TypedObject.AddComponent<TextMeshProUGUI>();
        
        TextParametrs.font = ReadEvent.Instance.Font;
        if (t == TypeObj.Text)
        {
            TextParametrs.fontSize = 28;
            TextParametrs.text = text;
        }
        else if (t == TypeObj.Title)
        {
            TextParametrs.fontSize = 42;
            TextParametrs.text = text;
            TextParametrs.alignment = TextAlignmentOptions.Center;
        }
        TextParametrs.raycastTarget = false;        
        TextParametrs.enableWordWrapping = true;
        TextParametrs.color = Color.black;

        RectTran.sizeDelta = new Vector2(ReadEvent.width - ReadEvent.SpaceFortext, 0f);
        RectTran.localPosition = new Vector3(100f, 100f, 100f); //Делается для красивого появления
    }
    public void SetYValue()
    {
        if(t!=TypeObj.Image)
        {
            RectTran.sizeDelta = new Vector2(ReadEvent.width - ReadEvent.SpaceFortext, TextParametrs.preferredHeight);
        }
    }
}
[System.Serializable]
public class ReadFile
{   
    [HideInInspector]
    public ReadObject RD;    
    [Header("Choose one: Image or something else")]
    [InspectorName("Type")]
    public TypeObj t = TypeObj.Image;
    public Sprite sprite = null;
    [TextArea(8, 30)]
    public string text = null;
    public ReadObject ToReadObject()
    {
        if(t == 0)
        {            
            RD = new ReadObject(sprite);
        }
        else if(t == TypeObj.Text)
        {
            RD = new ReadObject(text, TypeObj.Text);
        }
        else if (t==TypeObj.Title)
        {
            RD = new ReadObject(text, TypeObj.Title);
        }
        return RD;
    }    
    public static Sprite ToSpite(Texture2D photo)
    {
        return Sprite.Create(photo, new Rect(new Vector2(0f, 0f), new Vector2(photo.width, photo.height)), new Vector2(0f, 0f));
    }
}

