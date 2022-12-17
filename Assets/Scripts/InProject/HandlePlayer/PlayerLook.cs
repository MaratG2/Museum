using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

#pragma warning disable 0649
public class PlayerLook : MonoBehaviour
{
    [SerializeField] private string mouseXInputName, mouseYInputName;
    [SerializeField] private Transform playerBody;
    [SerializeField] private LayerMask _uiLayerMask;
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
            if (Input.GetMouseButtonDown(0))
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
        RaycastHit[] hits = Physics.RaycastAll(ray.origin, ray.direction * 1000f, 1000f, layerMask: _uiLayerMask);
        if (hits.Length == 0)
            return;
        foreach (var hit in hits)
        {
            Button button = hit.collider.gameObject.GetComponent<Button>();
            if (button == null)
                continue;
            ExecuteEvents.Execute(button.gameObject, new BaseEventData(EventSystem.current), ExecuteEvents.submitHandler);
        }
        
    }
    private void ClampXAxisRotationToValue(float value)
    {
        Vector3 eulerRotation = transform.eulerAngles;
        eulerRotation.x = value;
        transform.eulerAngles = eulerRotation;
    }
}
#pragma warning restore 0649