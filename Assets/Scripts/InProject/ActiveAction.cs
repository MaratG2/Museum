using InProject;
using Museum.Scripts.HandlePlayer;
using Museum.Scripts.Menu;
using UnityEngine;

public class ActiveAction : MonoBehaviour
{
    float rayLength = 2f;
    [SerializeField]
    private Camera camera;
    public Camera Camera => camera;

    public bool isForceE = false;
    private IInteractive _oldInteractive;
    
    void Update()
    {
        if (isForceE)
        {
            if(_oldInteractive != null && _oldInteractive.IsOpen())
                _oldInteractive?.Interact();
            InteractiveLabel.Instance.CloseInfoLable();
            isForceE = false;
        }
        if (PlayerManager.IsJump == false)
        {
            RaycastHit hit;

            if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, rayLength))
            {
                if (hit.collider.CompareTag("Interactive Object"))
                {
                    var title = hit.transform.GetComponent<IInteractive>().Title;
                    InteractiveLabel.Instance.ShowInfoLableWithTitleObj(title);
                    if (Input.GetKeyDown(KeyCode.E))
                    {
                        _oldInteractive = hit.collider.gameObject.GetComponent<IInteractive>();
                        _oldInteractive?.Interact();
                    }
                }
                else
                {
                    InteractiveLabel.Instance.CloseInfoLable();
                }
            }
            else
                InteractiveLabel.Instance.CloseInfoLable();


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
