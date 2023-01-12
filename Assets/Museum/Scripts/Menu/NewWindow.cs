using UnityEngine;

namespace Museum.Scripts.Menu
{
    public class NewWindow : MonoBehaviour
    {
        [SerializeField] private GameObject Info;
        [SerializeField] private GameObject Control;
        [SerializeField] private GameObject Panel;

        public void OpenInfo()
        {
            Info.SetActive(true);
            Panel.SetActive(true);
        }
        
        public void OpenControl()
        {
            Control.SetActive(true);
            Panel.SetActive(true);
        }
        
        public void Close()
        {
            Info.SetActive(false);
            Control.SetActive(false);
            Panel.SetActive(false);
        }
    }
}