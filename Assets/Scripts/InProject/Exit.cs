using UnityEngine;
using UnityEngine.SceneManagement;

namespace InProject
{
    public class Exit : MonoBehaviour, IInteractive
    {
        public void Interact()
        {
            SceneChoice.scene = 0;
            SceneManager.LoadScene("Load", LoadSceneMode.Single);
        }
    }
}
