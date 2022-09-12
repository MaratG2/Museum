using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenDoor : MonoBehaviour
{
    public Transform player;
    Animator anim;
    public float distance = 5f;
   
    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        Open_Door();
    }
    void Open_Door()
    {
        if (Vector3.Distance(player.transform.position, this.transform.position) < distance)
            anim.SetBool("character_nearby", true);
        else
            anim.SetBool("character_nearby", false);
    }
    
}
