using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#pragma warning disable 0649
public class Location : MonoBehaviour,ITeleportate
{
    
    public string Name;
    
    private void Start()
    {        
        Teleportation.AddPoint(Name, new PosAndRot(new Vector3(transform.position.x, transform.position.y-2f, transform.position.z),transform.rotation));        
    }
    public void Teleportate()
    {
        PlayerManager.Instance.Teleport(Name);
    }
}
public class PosAndRot
{
    public Vector3 pos;
    public Quaternion rot;
    public PosAndRot(Vector3 p , Quaternion r)
    {
        pos = p;
        rot = r;
    }
}

#pragma warning restore 0649