using TMPro;
using UnityEngine;

public class SetFPS : MonoBehaviour
{
    [SerializeField] private int _targetFPS = 30;
     
    void Awake()
    {
        //QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = _targetFPS;
    }
     
    void Update()
    {
        if(Application.targetFrameRate != _targetFPS)
            Application.targetFrameRate = _targetFPS;
    }
}
