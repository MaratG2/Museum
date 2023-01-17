using TMPro;
using UnityEngine;

namespace InProject
{
    public class InteractiveLabel : MonoBehaviour
    {
        #region Singleton

        private static InteractiveLabel _instance;
        public static InteractiveLabel Instance
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
        private string str;
        TextMeshProUGUI TextOfLabelGUI;
        public GameObject Lable;
    
        [SerializeField] public GameObject lableForTitle;
        [SerializeField]private TextMeshProUGUI _textLableForTitle;

        private void Awake()
        {
            TextOfLabelGUI = Lable.GetComponent<TextMeshProUGUI>();

            str = Lable.GetComponent<TextMeshProUGUI>().text;
            _instance = GetComponent<InteractiveLabel>();
        }

        public void ChangeStateInfoLable(bool b)
        {
            Lable.SetActive(b);
        }
    
        public void ShowInfoLableWithTitleObj(string title)
        {
            Lable.SetActive(true);
            if (string.IsNullOrEmpty(title))
            {
                Debug.LogWarning("Attempt to include an interactive label with an empty object name.");
                return;
            }
            lableForTitle.SetActive(true);
            _textLableForTitle.text = title;
        }
        
        public void CloseInfoLable()
        {
            Lable.SetActive(false);
            lableForTitle.SetActive(false);
        }
        
        public void ChangeTextLabel(string s)
        {
            TextOfLabelGUI.text = s;
        }
        
        public void SetDefaultText()
        {
            TextOfLabelGUI.text = str;
        }
    }
}
