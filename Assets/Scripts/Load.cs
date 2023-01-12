using System.Collections;
using System.Collections.Generic;
using Museum.Scripts.HandlePlayer;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Load : MonoBehaviour
{
    public GameObject c;
    // Start is called before the first frame update
    IEnumerator Start()
    {
        PlayerManager.IsJump = false;
        AsyncOperation L = SceneManager.LoadSceneAsync("Inside");
        while (L.isDone == false)
        {
            c.transform.Rotate(Vector3.forward, -15f * Time.deltaTime);
            yield return null;
        }
        yield return null;
    }    
}
