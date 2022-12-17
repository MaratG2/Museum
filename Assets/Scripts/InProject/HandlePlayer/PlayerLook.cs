using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#pragma warning disable 0649
public class PlayerLook : MonoBehaviour
{
    [SerializeField] private string mouseXInputName, mouseYInputName;
    [SerializeField] private Transform playerBody;
    private ActiveAction _activeAction;

    private float xAxisClamp;

    private void Awake()
    {
        _activeAction = GetComponent<ActiveAction>();
        LockCursor();
        xAxisClamp = 0.0f;
    }


    private void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        if(!State.Frozen)
        {
            CameraRotation();
            RaycastToUI();
        }
    }

    private void CameraRotation()
    {
        float mouseX = Input.GetAxis(mouseXInputName) * PlayerManager.mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis(mouseYInputName) * PlayerManager.mouseSensitivity * Time.deltaTime;

        xAxisClamp += mouseY;

        if (xAxisClamp > 90.0f)
        {
            xAxisClamp = 90.0f;
            mouseY = 0.0f;
            ClampXAxisRotationToValue(270.0f);
        }
        else if (xAxisClamp < -90.0f)
        {
            xAxisClamp = -90.0f;
            mouseY = 0.0f;
            ClampXAxisRotationToValue(90.0f);
        }

        transform.Rotate(Vector3.left * mouseY);
        playerBody.Rotate(Vector3.up * mouseX);
    }

    private void RaycastToUI()
    {
        Ray ray = new Ray(_activeAction.Camera.transform.position, _activeAction.Camera.transform.forward);
        Debug.DrawRay(ray.origin, ray.direction * 10f, Color.red);
    }
    private void ClampXAxisRotationToValue(float value)
    {
        Vector3 eulerRotation = transform.eulerAngles;
        eulerRotation.x = value;
        transform.eulerAngles = eulerRotation;
    }
}
#pragma warning restore 0649