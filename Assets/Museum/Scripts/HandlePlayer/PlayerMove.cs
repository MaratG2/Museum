using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#pragma warning disable 0649
public class PlayerMove : MonoBehaviour
{
    [SerializeField] private float movementSpeed;
    [SerializeField] private float boostspeed;
    float sprint=1f;

    private CharacterController charController;

    [SerializeField] private AnimationCurve jumpFallOff;
    [SerializeField] private float jumpMultiplier;
    [SerializeField] private KeyCode jumpKey;

    

    private void Awake()
    {        
        charController = GetComponent<CharacterController>();        
    }

    private void Update()
    {
        if(!State.Frozen)
            PlayerMovement();
    }

    private void PlayerMovement()
    {
        float horizInput = Input.GetAxis("Horizontal");
        float vertInput = Input.GetAxis("Vertical");
        
        Sprint();
        Vector3 forwardMovement = transform.forward * vertInput;
        Vector3 rightMovement = transform.right * horizInput;       

        charController.SimpleMove(Vector3.Normalize(forwardMovement + rightMovement) * movementSpeed*sprint);

        JumpInput();
    }
    private void Sprint()
    {
        if (Input.GetKey(KeyCode.LeftShift))
        {
            sprint = boostspeed;
        }
        else sprint = 1f;
    }
    private void JumpInput()
    {
        if (Input.GetKeyDown(jumpKey) && !PlayerManager.isJump)
        {
            PlayerManager.isJump = true;
            StartCoroutine("JumpEvent");
        }
    }

    private IEnumerator JumpEvent()
    {        
        float timeInAir = 0.0f;

        do
        {
            float jumpForce = jumpFallOff.Evaluate(timeInAir);
            charController.Move(Vector3.up * jumpForce * jumpMultiplier * Time.deltaTime);
            timeInAir += Time.deltaTime;
            yield return null;
        } while (!charController.isGrounded && charController.collisionFlags != CollisionFlags.Above);


        PlayerManager.isJump = false;
    }

}
#pragma warning restore 0649