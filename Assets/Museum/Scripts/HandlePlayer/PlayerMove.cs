using System.Collections;
using InProject;
using UnityEngine;

namespace Museum.Scripts.HandlePlayer
{
    public class PlayerMove : MonoBehaviour
    {
        [SerializeField] private float movementSpeed;
        [SerializeField] private float boostspeed;
        float _sprint = 1f;

        private CharacterController _charController;

        [SerializeField] private AnimationCurve jumpFallOff;
        [SerializeField] private float jumpMultiplier;
        [SerializeField] private KeyCode jumpKey;
        
        private void Awake()
        {        
            _charController = GetComponent<CharacterController>();        
        }

        private void Update()
        {
            if(!State.Frozen)
                PlayerMovement();
        }

        private void PlayerMovement()
        {
            var horizInput = Input.GetAxis("Horizontal");
            var vertInput = Input.GetAxis("Vertical");
        
            Sprint();
            var curentTransform = transform;
            var forwardMovement = curentTransform.forward * vertInput;
            var rightMovement = curentTransform.right * horizInput;       

            _charController.SimpleMove(Vector3.Normalize(forwardMovement + rightMovement) * movementSpeed*_sprint);

            JumpInput();
        }
        
        private void Sprint()
        {
            _sprint = Input.GetKey(KeyCode.LeftShift) ? boostspeed : 1f;
        }
        private void JumpInput()
        {
            if (!Input.GetKeyDown(jumpKey) || PlayerManager.IsJump) 
                return;
            
            PlayerManager.IsJump = true;
            StartCoroutine("JumpEvent");
        }

        private IEnumerator JumpEvent()
        {        
            var timeInAir = 0.0f;

            do
            {
                float jumpForce = jumpFallOff.Evaluate(timeInAir);
                _charController.Move(Vector3.up * jumpForce * jumpMultiplier * Time.deltaTime);
                timeInAir += Time.deltaTime;
                yield return null;
            } while (!_charController.isGrounded && _charController.collisionFlags != CollisionFlags.Above);
            
            PlayerManager.IsJump = false;
        }
    }
}
