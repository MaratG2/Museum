using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class ToTheMuseum : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        SceneChoice.scene = 1;
        SceneManager.LoadScene("Load", LoadSceneMode.Single);        
    }
}
