using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleportation:MonoBehaviour
{
    
    public static Dictionary<string, PosAndRot> points = new Dictionary<string, PosAndRot>();
    public static void AddPoint(string n, PosAndRot p)
    {        
        if(!points.ContainsKey(n))
            points.Add(n, p);
    }
    public void Teleportate(string s)
    {        
        gameObject.transform.position = points[s].pos;
        gameObject.transform.rotation = points[s].rot;
        gameObject.transform.Rotate(Vector3.up, 90f);
        print(s);
        StartCoroutine("AfterTeleportation");
        
    }
    public IEnumerator AfterTeleportation()
    {        
        yield return null;        
        State.SetCursorLock(true);
        State.Frozen=false;        
        yield return null;
    }
    
}
