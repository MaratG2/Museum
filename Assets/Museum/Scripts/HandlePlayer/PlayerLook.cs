using InProject;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Museum.Scripts.HandlePlayer
{
    public class PlayerLook : MonoBehaviour
    {
        [SerializeField] private string mouseXInputName, mouseYInputName;
        [SerializeField] private Transform playerBody;
        [FormerlySerializedAs("_uiLayerMask")] 
        [SerializeField] private LayerMask uiLayerMask;
        
        private ActiveAction _activeAction;
        private float _xAxisClamp;

        private void Awake()
        {
            _activeAction = GetComponent<ActiveAction>();
            LockCursor();
            _xAxisClamp = 0.0f;
        }


        private void LockCursor()
        {
            Cursor.lockState = CursorLockMode.Locked;
        }

        private void Update()
        {
            if (State.Frozen) 
                return;
            
            CameraRotation();
            if (Input.GetMouseButtonDown(0))
                RaycastToUI();
        }

        private void CameraRotation()
        {
            var mouseX = Input.GetAxis(mouseXInputName) * PlayerManager.MouseSensitivity * Time.deltaTime;
            var mouseY = Input.GetAxis(mouseYInputName) * PlayerManager.MouseSensitivity * Time.deltaTime;

            _xAxisClamp += mouseY;

            if (_xAxisClamp > 90.0f)
            {
                _xAxisClamp = 90.0f;
                mouseY = 0.0f;
                ClampXAxisRotationToValue(270.0f);
            }
            else if (_xAxisClamp < -90.0f)
            {
                _xAxisClamp = -90.0f;
                mouseY = 0.0f;
                ClampXAxisRotationToValue(90.0f);
            }

            transform.Rotate(Vector3.left * mouseY);
            playerBody.Rotate(Vector3.up * mouseX);
        }

        private void RaycastToUI()
        {
            var currentTransform = _activeAction.Camera.transform;
            var ray = new Ray(currentTransform.position, currentTransform.forward);
            var hits = Physics.RaycastAll(ray.origin, ray.direction * 1000f, 1000f, layerMask: uiLayerMask);
            if (hits.Length == 0)
                return;
            foreach (var hit in hits)
            {
                var button = hit.collider.gameObject.GetComponent<Button>();
                if (button == null)
                    continue;
                ExecuteEvents.Execute(button.gameObject, new BaseEventData(EventSystem.current), ExecuteEvents.submitHandler);
            }
        }
        
        private void ClampXAxisRotationToValue(float value)
        {
            var currentTransform = transform;
            Vector3 eulerRotation = currentTransform.eulerAngles;
            eulerRotation.x = value;
            currentTransform.eulerAngles = eulerRotation;
        }
    }
}