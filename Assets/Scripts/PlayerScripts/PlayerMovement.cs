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
    Rigidbody rb;

    enum States
    {
        Idle,
        Run,
        Jump,
        Attack
    }

    [SerializeField]
    States playerState = States.Idle;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {

        if (!IsJump())
            PlayerMove();
        PlayerAttack();
        SwitchAnimation();
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

    bool IsJump()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            playerState = States.Jump;
            rb.AddForce(Vector3.up * jumpSpeed, ForceMode.Impulse);
            return true;
        }
        return false;
    }

    void PlayerAttack()
    {
        if (Input.GetKeyDown(KeyCode.X))
            playerState = States.Attack;
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
}
