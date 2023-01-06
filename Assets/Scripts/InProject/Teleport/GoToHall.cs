using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoToHall : MonoBehaviour,IInteractive
{
    public void Interact()
    {
        State.Frozen = true; 
        PlayerManager.Instance.Teleport("Холл");       
    }
}
