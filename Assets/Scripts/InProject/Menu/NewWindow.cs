using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#pragma warning disable 0649
public class NewWindow : MonoBehaviour
{
    [SerializeField]
    GameObject Info;
    [SerializeField]
    GameObject Control;
    [SerializeField]
    GameObject Panel;

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
#pragma warning restore 0649