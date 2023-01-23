using InProject;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Museum.Scripts.HandlePlayer
{
    public class PlayerLook : MonoBehaviour
    {
        [Range(0f, 90f)][SerializeField] private float _yRotationLimit = 88f;
        [SerializeField] private string mouseXInputName, mouseYInputName;
        [SerializeField] private Transform _playerBody;
        [SerializeField] private LayerMask _uiLayerMask;

        private Vector2 _rotation = Vector2.zero;
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

            _rotation.x += mouseX;
            _rotation.y += mouseY;
            _rotation.y = Mathf.Clamp(_rotation.y, -_yRotationLimit, _yRotationLimit);
            var xQuat = Quaternion.AngleAxis(_rotation.x, Vector3.up);
            var yQuat = Quaternion.AngleAxis(_rotation.y, Vector3.left);
            var newRotation = xQuat * yQuat;
            transform.rotation = newRotation;
            _playerBody.rotation = newRotation;
        }

        private void RaycastToUI()
        {
            var currentTransform = _activeAction.Camera.transform;
            var ray = new Ray(currentTransform.position, currentTransform.forward);
            var hits = Physics.RaycastAll(ray.origin, ray.direction * 1000f,
                1000f, layerMask: _uiLayerMask);
            if (hits.Length == 0)
                return;
            foreach (var hit in hits)
            {
                var button = hit.collider.gameObject.GetComponent<Button>();
                if (button == null)
                    continue;
                ExecuteEvents.Execute(button.gameObject, 
                    new BaseEventData(EventSystem.current), ExecuteEvents.submitHandler);
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