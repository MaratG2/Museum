using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PictureBlock : MonoBehaviour, IExhibit
{
    public GameObject Model { get; set; }

    void Start()
    {
        Model = gameObject;
    }
}
