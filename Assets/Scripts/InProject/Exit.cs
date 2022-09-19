using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Exit : MonoBehaviour,IInterative
{
    
    public void Interact()
    {
        SceneChoice.scene = 0;
        SceneManager.LoadScene("Load", LoadSceneMode.Single);
    }
}
