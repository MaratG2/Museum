using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ActiveAction : MonoBehaviour
{
    float rayLength = 2f;
    [SerializeField]
    private Camera camera;
    
    void Update()
    {
        if (PlayerManager.isJump == false)
        {
            RaycastHit hit;

            if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, rayLength))
            {
                if (hit.collider.CompareTag("Interactive Object"))
                {
                    InteractiveLabel.Instance.ShowLabal(true);
                    if (Input.GetKeyDown(KeyCode.E))
                    {
                        hit.collider.gameObject.GetComponent<IInterative>().Interact();
                    }
                }
                else
                {
                    InteractiveLabel.Instance.ShowLabal(false);
                }
            }
            else
                InteractiveLabel.Instance.ShowLabal(false);

            if (SceneManager.GetActiveScene().name == "Inside" && TeleportToClick.Instance.ViewMap == true)
            {
                Ray ray = camera.ScreenPointToRay(Input.mousePosition);

                if (Physics.Raycast(ray, out hit))
                {
                    if (hit.collider.CompareTag("TeleportSpot"))
                    {
                        if (Input.GetKeyDown(KeyCode.Mouse0))
                        {
                            hit.collider.gameObject.GetComponent<ITeleportate>().Teleportate();
                            TeleportToClick.Instance.ChangeCamPriority();
                            //State.SetCursorLock();
                        }
                    }
                }
            }

            if (Input.GetKeyDown(KeyCode.Tab))
            {
                Menu.Instance.Activate();
            }

            if (Input.GetKeyDown(KeyCode.M))
            {
                try
                {
                    State.View(true);
                    TeleportToClick.Instance.ChangeCamPriority();                    
                }
                catch
                {
                    print("В данной сцене нет телепортации");
                }
            }
            
        }

    }
}
