using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public CharacterController controller;
    public Transform cam;
    public float speed = 6f;
    public float turnSmoothTime = 0.01f;
    public float jumpSpeed = 5f;
    public Animator anim;
    float turnSmoothVelocity;
    private float gravityValue = -9.81f;
    private Vector3 playerVelocity;

    enum States
    {
        Idle,
        Run,
        Jump,
        Attack
    }

    [SerializeField]
    States playerState = States.Idle;

    [SerializeField]
    private bool groundedPlayer;

    [SerializeField]
    Transform foot;

    [SerializeField]
    float groundRadius = 0.3f;

    [SerializeField]
    AudioClip jump, charge;
    void Update()
    {
        groundedPlayer = GroundCheck();

        if (groundedPlayer && playerVelocity.y < 0)
        {
            playerVelocity.y = 0f;
        }


        PlayerJump();
        PlayerAttack();
        SwitchAnimation();
        PlayerMove();

        playerVelocity.y += gravityValue * Time.deltaTime;
        controller.Move(playerVelocity * Time.deltaTime);
    }

    void PlayerMove()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;

        if (direction.magnitude >= 0.01f)
        {
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime * Time.deltaTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            controller.Move(moveDir.normalized * speed * Time.deltaTime);
            playerState = States.Run;
        }
        else
        {
            playerState = States.Idle;
        }

    }

    void PlayerJump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && groundedPlayer)
        {
            playerState = States.Jump;
            playerVelocity.y += Mathf.Sqrt(jumpSpeed * -3.0f * gravityValue);
            GetComponent<AudioSource>().clip = jump;
            GetComponent<AudioSource>().Play();
        }
    }

    void PlayerAttack()
    {
        if (Input.GetKeyDown(KeyCode.X))
        {
            playerState = States.Attack;
            GetComponent<AudioSource>().clip = charge;
            GetComponent<AudioSource>().Play();
        }
            
    }

    void SwitchAnimation()
    {
        switch (playerState)
        {
            case States.Run:
                anim.SetBool("Run", true);
                break;
            case States.Attack:
                anim.SetBool("Run", false);
                anim.SetTrigger("Attack");
                break;
            case States.Jump:
                anim.SetBool("Run", false);
                anim.SetTrigger("Jump");
                break;
            default:
                anim.SetBool("Run", false);
                break;

        }

    }

    bool GroundCheck()
    {
        return Physics.CheckSphere(foot.position, groundRadius, LayerMask.GetMask("Ground"), QueryTriggerInteraction.Ignore);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(foot.position, groundRadius);
    }
}
