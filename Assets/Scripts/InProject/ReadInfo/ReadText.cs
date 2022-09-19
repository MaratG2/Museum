using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReadText : MonoBehaviour,IInterative
{
    bool flag = false;
    
    public List<ReadFile> ListFile = new List<ReadFile>();     
    
    public void Interact()
    {
        if (!flag)
        {            
            Open();
            flag = true;
        }
        else
        {
            Close();
            flag = false;
        }

    }
    void Open()
    {
        State.View(true);
        SetListForRead();
        StartCoroutine(ReadEvent.Instance.SetForm());
        ReadEvent.Instance.PicturePanel.SetActive(true);
    }
    void Close()
    {
        State.View(false);
        ReadEvent.Instance.DestroyList();
        ReadEvent.Instance.PicturePanel.SetActive(false);
    }
    void SetListForRead()
    {        
        ReadEvent.Instance.ListFile = ListFile;
    }
}
