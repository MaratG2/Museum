using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Museum.Scripts.ReadInfo
{
    public enum TypeObj
    {
        Image,
        Title,
        Text
    }
    
    public class ReadObject
    {
        public GameObject TypedObject;
        public RectTransform RectTran;
        TextMeshProUGUI TextP;
        TypeObj type = TypeObj.Image;
        public TextMeshProUGUI TextParametrs
        {
            get
            {
                if (type != 0)
                    return TextP;
                return null;
            }
            private set => TextP = value;
        }

        public ReadObject(Sprite image)
        {
            TypedObject = new GameObject();
            TypedObject.transform.SetParent(ReadEvent.Instance.goForm.transform);
            RectTran = TypedObject.AddComponent<RectTransform>();
            TypedObject.AddComponent<Image>();
            TypedObject.GetComponent<Image>().sprite = image;
            TypedObject.GetComponent<Image>().raycastTarget = false;
            RectTran.sizeDelta = new Vector2(ReadEvent.Width, ReadEvent.Height);

            RectTran.localPosition = new Vector3(100f, 100f, 100f); //Делается для красивого появления
        }
        public ReadObject(string text, TypeObj type)
        {
            this.type = type;

            TypedObject = new GameObject();
            TypedObject.transform.SetParent(ReadEvent.Instance.goForm.transform);
            RectTran = TypedObject.AddComponent<RectTransform>();
            TextParametrs = TypedObject.AddComponent<TextMeshProUGUI>();
        
            TextParametrs.font = ReadEvent.Instance.Font;
            if (type == TypeObj.Text)
            {
                TextParametrs.fontSize = 28;
                TextParametrs.text = text;
            }
            else if (type == TypeObj.Title)
            {
                TextParametrs.fontSize = 42;
                TextParametrs.text = text;
                TextParametrs.alignment = TextAlignmentOptions.Center;
            }
            TextParametrs.raycastTarget = false;        
            TextParametrs.enableWordWrapping = true;
            TextParametrs.color = Color.black;

            RectTran.sizeDelta = new Vector2(ReadEvent.Width - ReadEvent.SpaceFortext, 0f);
            RectTran.localPosition = new Vector3(100f, 100f, 100f); //Делается для красивого появления
        }
        public void SetYValue()
        {
            if(type!=TypeObj.Image)
            {
                RectTran.sizeDelta = new Vector2(ReadEvent.Width - ReadEvent.SpaceFortext, TextParametrs.preferredHeight);
            }
        }
    }
    [System.Serializable]
    public class ReadFile
    {
        public ReadObject RD;    
        [FormerlySerializedAs("t")]
        [Header("Choose one: Image or something else")]
        [InspectorName("Type")]
        public TypeObj type = TypeObj.Image;
        public Sprite sprite;
        [TextArea(8, 30)]
        public string text;

        public ReadObject ToReadObject()
        {
            if (type == TypeObj.Image)
                RD = new ReadObject(sprite);
        
            else if (type == TypeObj.Text)
                RD = new ReadObject(text, TypeObj.Text);

            else if (type == TypeObj.Title)
                RD = new ReadObject(text, TypeObj.Title);

            return RD;
        }

        public static Sprite ToSpite(Texture2D photo)
        {
            return Sprite.Create(photo, new Rect(new Vector2(0f, 0f), new Vector2(photo.width, photo.height)), new Vector2(0f, 0f));
        }
    }
}