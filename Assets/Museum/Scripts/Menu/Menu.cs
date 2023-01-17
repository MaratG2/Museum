using System.Collections.Generic;
using InProject;
using UnityEngine;
using UnityEngine.Serialization;

namespace Museum.Scripts.Menu
{
    public class Menu : MonoBehaviour
    {
        private static Menu _instance;
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
        
        bool _flag;
        [SerializeField] private GameObject obj;

        [SerializeField] public GameObject roomsMenu;
    
        [FormerlySerializedAs("MustBeClosed")] [SerializeField]
        List<GameObject> mustBeClosed = new();
        private void Awake()
        {
            _instance = GetComponent<Menu>();
        }

        public void Activate()
        {
            if (State.Frozen && !_flag) return;
            
            State.View();
            obj.SetActive(!obj.activeInHierarchy);
            _flag = !_flag;               
            foreach (var i in mustBeClosed)
                i.SetActive(false);
        }
    
        public void ActivateRoomMenu()
        {
            if (State.Frozen && !_flag) return;
            
            State.View();
            roomsMenu.SetActive(!roomsMenu.activeInHierarchy);
            _flag = !_flag;               
            foreach (var i in mustBeClosed)
                i.SetActive(false);
        }
    }
}
