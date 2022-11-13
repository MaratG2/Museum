using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PictureBlock : MonoBehaviour, IExhibit
{
    public GameObject Model { get; set; }
    public GameObject Picture { get; set; }
    public Image Image { get; set; }

    void Start()
    {
        Model = gameObject;
    }
}
