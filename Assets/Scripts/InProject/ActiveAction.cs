using Museum.Scripts.HandlePlayer;
using Museum.Scripts.Menu;
using UnityEngine;

public class ActiveAction : MonoBehaviour
{
    float rayLength = 2f;
    [SerializeField]
    private Camera camera;
    public Camera Camera => camera;
    
    void Update()
    {
        if (PlayerManager.IsJump == false)
        {
            RaycastHit hit;

            if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, rayLength))
            {
                if (hit.collider.CompareTag("Interactive Object"))
                {
                    InteractiveLabel.Instance.ShowLabal(true);
                    if (Input.GetKeyDown(KeyCode.E))
                    {
                        hit.collider.gameObject.GetComponent<IInteractive>().Interact();
                    }
                }
                else
                {
                    InteractiveLabel.Instance.ShowLabal(false);
                }
            }
            else
                InteractiveLabel.Instance.ShowLabal(false);
            

            if (Input.GetKeyDown(KeyCode.Tab))
            {
                Menu.Instance.Activate();
            }
            
            if (Input.GetKeyDown(KeyCode.R))
            {
                Menu.Instance.ActivateRoomMenu();
            }
        }

    }
}
