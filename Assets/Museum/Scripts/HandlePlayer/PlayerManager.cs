using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Museum.Scripts.HandlePlayer
{
    public class PlayerManager : MonoBehaviour
    {
        private static PlayerManager _instance;
        public static PlayerManager Instance => _instance;
        [FormerlySerializedAs("Slider")] 
        public GameObject slider;
        
        [HideInInspector] public Transform tran;
        public static bool IsJump = false;
        public static float MouseSensitivity = 150f;

        private void Awake()
        {
            GameObject o;
            _instance = (o = gameObject).GetComponent<PlayerManager>();
            tran = o.transform;
            slider.GetComponent<Slider>().value = MouseSensitivity;
        }

        public void SetSensity()
        {           
            MouseSensitivity = slider.GetComponent<Slider>().value;
        }
    }
}
