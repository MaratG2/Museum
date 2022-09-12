using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class TeleportToClick : MonoBehaviour
{
    #region singl
    private static TeleportToClick _instance;
    public static TeleportToClick Instance
    {
        get
        {
            return _instance;
        }
    }
    #endregion
    public bool ViewMap = false;
    [SerializeField]
    private CinemachineVirtualCamera playerCam;
    [SerializeField]
    private CinemachineVirtualCamera telCam;
    private void Awake()
    {
        _instance = gameObject.GetComponent<TeleportToClick>();
    }
    public void ChangeCamPriority()
    {
        if (playerCam.Priority > telCam.Priority)
        {
            ViewMap = true;
            telCam.Priority = 10;
            playerCam.Priority = 1;
        }
        else
        {
            ViewMap = false;
            telCam.Priority = 1;
            playerCam.Priority = 10;
        }
    }

}
