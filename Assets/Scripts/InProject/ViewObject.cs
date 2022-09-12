using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#pragma warning disable 0649
public class ViewObject : MonoBehaviour,IInterative
{
    [SerializeField]
    private string Name;
    bool flag = false;
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
        InteractiveLabel.Instance.ChangeTextLabel(Name);
        ActiveObjectView.Instance.CreateObj(gameObject);
    }
    void Close()
    {        
        State.View(false);
        InteractiveLabel.Instance.SetDefaultText();
        ActiveObjectView.Instance.DestroyObj();
    }
}
#pragma warning restore 0649