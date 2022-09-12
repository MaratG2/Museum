using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoToHall : MonoBehaviour,IInterative
{
    public void Interact()
    {
        State.Frozen = true; 
        PlayerManager.Instance.Teleport("Холл");       
    }
}
