using System.Collections;
using System.Collections.Generic;
using Museum.Scripts.HandlePlayer;
using UnityEngine;

public class RoomText : MonoBehaviour
{
    // Start is called before the first frame update
    Transform player;
    Quaternion forward;
    Quaternion backward;
    public GameObject text;
    float angle;
    
    void Start()
    {
        player = PlayerManager.Instance.tran;
        angle = (gameObject.transform.rotation.eulerAngles[1]+360)%360;  
        
    }

    //// Update is called once per frame
    void Update()
    {
        switch (angle)
        {
            case 0:
                if (player.transform.position.z + 0.3f > gameObject.transform.position.z)
                    text.SetActive(false);
                else
                    text.SetActive(true);
                break;
            case 90:
                if (player.transform.position.x + 0.3f > gameObject.transform.position.x)
                    text.SetActive(false);
                else
                    text.SetActive(true);
                break;
            case 180:
                if (player.transform.position.z - 0.3f < gameObject.transform.position.z)
                    text.SetActive(false);
                else
                    text.SetActive(true);
                break;
            case 270:
                if (player.transform.position.x - 0.3f < gameObject.transform.position.x)
                    text.SetActive(false);
                else
                    text.SetActive(true);
                break;
        }
    }
}
