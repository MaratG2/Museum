using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SetMapText : MonoBehaviour
{
    private Transform father;
    private string name;

    void Start()
    {
        father = GetComponentInParent<Transform>();
        name = GetComponentInParent<Location>().Name;
        TextMeshPro text = GetComponent<TextMeshPro>();
        text.text = name;

        if(father.rotation.y == 0.5f)
        {            
            gameObject.transform.Rotate(new Vector3(0, 0, 180));
        }
    }
    
}
