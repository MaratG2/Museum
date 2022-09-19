using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

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
    private void Awake()
    {
        TextOfLabelGUI = Lable.GetComponent<TextMeshProUGUI>();
        str = Lable.GetComponent<TextMeshProUGUI>().text;
        _instance = GetComponent<InteractiveLabel>();
    }
    public GameObject Lable;
    public void ShowLabal(bool b)
    {
        Lable.SetActive(b);
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
