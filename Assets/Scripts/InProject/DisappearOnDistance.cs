using System.Collections;
using System.Collections.Generic;
using Museum.Scripts.HandlePlayer;
using UnityEngine;

public class DisappearOnDistance : MonoBehaviour
{
    public float Dist = 40f;
    public List<GameObject> gObj;
    // Update is called once per frame
    void Update()
    {
        if (Vector3.Distance(gameObject.transform.position, PlayerManager.Instance.tran.position) > Dist)
        {
            foreach(var i in gObj)
            {
                i.SetActive(false);
            }
        }
        else
        {
            foreach(var i in gObj)
            {
                i.SetActive(true);
            }
        }
    }
}
