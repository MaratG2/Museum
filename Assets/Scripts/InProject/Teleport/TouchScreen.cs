using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TouchScreen : MonoBehaviour,IInterative
{
    
    public void Interact()
    {
        State.View();        
    }
}
