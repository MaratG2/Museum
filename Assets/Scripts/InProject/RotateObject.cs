using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#pragma warning disable 0649
public class RotateObject : MonoBehaviour
{    
    Vector3 nowPosMouse;
    Vector3 prevPosMouse;
    Vector3 delta;
    private bool flagRotatingForce = false;
    [SerializeField]
    private Transform ParentObj;
    [SerializeField]
    private Transform customObj;
    [SerializeField]
    private Transform Camera;
    [SerializeField]
    [Space]
    [Header("Paramets")]
    private float minOfChangingSize;
    [SerializeField]
    private float maxOfChangingSize;
    [SerializeField]
    private float speed;
    [SerializeField]
    private float forceMultiply;
    private float magnitude;
    private float force;
    
    
    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {            
            nowPosMouse = Input.mousePosition;
            flagRotatingForce = false;
        }
        if(Input.GetMouseButton(0))
        {            
            prevPosMouse = nowPosMouse;
            nowPosMouse = Input.mousePosition;

            delta = nowPosMouse - prevPosMouse;            
            magnitude = delta.magnitude*speed;

            customObj.Rotate(new Vector3(delta.y, -delta.x,0), magnitude,Space.World);            
        }
        if(Input.GetMouseButtonUp(0))
        {
            force = magnitude;
            flagRotatingForce = true;
        }

        if(flagRotatingForce)
        {
            RotatingWithForce();
        }

        ChangeSize();
        
    }
    void RotatingWithForce()
    {
        if (Mathf.Abs(force) > 0.1f)
        {
            customObj.Rotate(new Vector3(delta.y, -delta.x, 0), force, Space.World);
            force *= forceMultiply;
        }
        else
            flagRotatingForce = false;
    }
    void ChangeSize()
    {
        float val = Vector3.Distance(ParentObj.transform.position, Camera.transform.position);
        float sd = Input.mouseScrollDelta.y;
        if (val> minOfChangingSize && sd<0)
            ParentObj.transform.Translate(new Vector3(0, 0, sd));
        else if(val < maxOfChangingSize && sd>0)
            ParentObj.transform.Translate(new Vector3(0, 0, sd));
    }
}
#pragma warning restore 0649