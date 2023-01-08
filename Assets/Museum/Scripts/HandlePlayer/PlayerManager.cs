using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class PlayerManager : MonoBehaviour
{
    private static PlayerManager _instance;
    public static PlayerManager Instance    
    {
        get {
            return _instance;
        }
    }
    public GameObject Slider;
    private void Awake()
    {
        _instance = gameObject.GetComponent<PlayerManager>();
        tran = gameObject.transform;
        Slider.GetComponent<Slider>().value = mouseSensitivity;
    }
    [HideInInspector] public Transform tran;
    public static bool isJump = false;
    public static float mouseSensitivity=150f;

    public void SetSensity()
    {           
        mouseSensitivity = Slider.GetComponent<Slider>().value;
    }

}
