using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Museum.Scripts.ReadInfo
{
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
        public GameObject PicturePanel;
        public TMP_FontAsset Font;
        public List<ReadObject> ListObjects = new();
        [HideInInspector]
        public List<ReadFile> ListFile = new();

        public static float Width;
        public static float Height;

        public static float SpaceFortext;
        private static float _spaceBetweenEl;

        float _yValueForScroll;



        void SetPanelProperty()
        {
            Width = Screen.width * 0.8f;
            Height = Width / 16f * 10f;

            SpaceFortext = Width * 0.05f;
            _spaceBetweenEl = Width * 0.04f;
        }


        private float NextPoint(RectTransform x1, RectTransform x2)
        {
            return -x1.sizeDelta.y / 2 - _spaceBetweenEl + x1.localPosition.y + (-x2.sizeDelta.y / 2);
        }

        public IEnumerator SetForm()
        {
            goScroll.GetComponent<RectTransform>().sizeDelta = new Vector2(Width, 0);
            CreateObjects();
            yield return null; // Для постановки системой подходящей высоты
            SetPostion();
        
            goForm.GetComponent<RectTransform>().sizeDelta = new Vector2(Width, _yValueForScroll);
            goForm.GetComponent<RectTransform>().localPosition = new Vector3(0f, -_yValueForScroll / 2 + _spaceBetweenEl, 0f);
        }

        private void SetPostion()
        {      

            foreach (var t in ListObjects)
            {
                t.SetYValue();            
                _yValueForScroll += t.RectTran.sizeDelta.y+ _spaceBetweenEl;
            }
        
            if(ListObjects != null && ListObjects.Count > 0)
                ListObjects[0].RectTran.localPosition = new Vector3(0, _yValueForScroll/2- ListObjects[0].RectTran.sizeDelta.y/2, 0);
        
            for (var i = 1; i < ListObjects.Count; i++)
            {           
                ListObjects[i].RectTran.localPosition = new Vector3(0, NextPoint(ListObjects[i - 1].RectTran, ListObjects[i].RectTran), 0);
            }
        }

        private void CreateObjects()
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
            _yValueForScroll = 0f;
            ListObjects.Clear();
        }
    }
}