using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour, IControllable
{
    public float speed = 5.0f;
    public float jumpForce = 300f;
    public float coyoteTime = 0.1f;
    public CharacterStates characterState;
    public Momentum horizontalMomentum = Momentum.None;
    public Momentum verticalMomentum = Momentum.None;

    public bool isGrounded = false;
    [HideInInspector] public Rigidbody2D rb;
    [HideInInspector] public bool isMovingRight => rb.velocity.x > slowMomentumThreeshold;
    [HideInInspector] public bool isMovingLeft => rb.velocity.x < -slowMomentumThreeshold;
    [HideInInspector] public bool isMovingUp => rb.velocity.y > verticalMomentumThreeshold;
    [HideInInspector] public bool isMovingDown => rb.velocity.y < -verticalMomentumThreeshold;

    public float horizontalSpeed = 0;
    public float verticalSpeed = 0;
    public float currentStateTime => Time.time - lastStateChangeTimestamp;
    float lastStateChangeTimestamp = 0;

    const float slowMomentumThreeshold = 0.5f;
    const float mediumMomentumThreeshold = 2f;
    const float fastMomentumThreeshold = 4f;

    const float verticalMomentumThreeshold = 1f;

    public enum CharacterStates { Idle, Walk, Run, Jump, Fall }
    public enum Momentum { None, Slow, Medium, Fast }

    CharacterAppearance appearance;

    float timeWithoutContact => Time.time - lastGroundTime;
    float lastGroundTime = 0;
    bool hasContact = false;
    Vector2 contactNormal;
    Vector2 contactPoint;
    public Vector2 contactTangent => new Vector2(contactNormal.y, -contactNormal.x);
    public bool canJump => isGrounded || timeWithoutContact < coyoteTime;

    void ChangeState(CharacterStates newState)
    {
        if (newState != characterState)
        {
            lastStateChangeTimestamp = Time.time;
            characterState = newState;
        }
    }

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        appearance = GetComponentInChildren<CharacterAppearance>();
    }

    void Update()
    {
        isGrounded = (hasContact && contactNormal.y > -0.1f);
        if (isGrounded)
        {
            lastGroundTime = Time.time;
        }

        UpdateMomentum();

        if (isGrounded)
        {
            switch (horizontalMomentum)
            {
                case Momentum.None:
                    ChangeState(CharacterStates.Idle);
                    break;
                case Momentum.Slow:
                    ChangeState(CharacterStates.Walk);
                    break;
                case Momentum.Medium:
                    ChangeState(CharacterStates.Run);
                    break;
                case Momentum.Fast:
                    ChangeState(CharacterStates.Run); //TODO
                    break;
            }
        }
        else
        {
            if (isMovingUp)
            {
                ChangeState(CharacterStates.Jump);
            }
            else
            {
                ChangeState(CharacterStates.Fall);
            }
        }
    }

    private void UpdateMomentum()
    {
        horizontalSpeed = Mathf.Abs(rb.velocity.x);
        verticalSpeed = Mathf.Abs(rb.velocity.y);

        if (horizontalSpeed > fastMomentumThreeshold)
        {
            horizontalMomentum = Momentum.Fast;
        }
        else if (horizontalSpeed > mediumMomentumThreeshold)
        {
            horizontalMomentum = Momentum.Medium;
        }
        else if (horizontalSpeed > slowMomentumThreeshold)
        {
            horizontalMomentum = Momentum.Slow;
        }
        else
        {
            horizontalMomentum = Momentum.None;
        }

        if (verticalSpeed > fastMomentumThreeshold)
        {
            verticalMomentum = Momentum.Fast;
        }
        else if (verticalSpeed > mediumMomentumThreeshold)
        {
            verticalMomentum = Momentum.Medium;
        }
        else if (verticalSpeed > slowMomentumThreeshold)
        {
            verticalMomentum = Momentum.Slow;
        }
        else
        {
            verticalMomentum = Momentum.None;
        }
    }

    public void Control(AController controller)
    {
        if (controller.horizontalAxis != 0)
        {
            float moveHorizontal = controller.horizontalAxis * speed;
            rb.velocity = new Vector2(moveHorizontal, rb.velocity.y);
            if (controller.horizontalAxis < 0)
            {
                appearance.LookLeft();
            }
            if (controller.horizontalAxis > 0)
            {
                appearance.LookRight();
            }
        }

        if (controller.wantsToJump && canJump)
        {
            rb.AddForce(new Vector2(0, jumpForce));
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        hasContact = true;
        contactNormal = Vector2.zero;
        contactPoint = Vector2.zero;
        foreach (var contact in collision.contacts)
        {
            contactNormal += contact.normal;
            contactPoint += contact.point;
        }
        contactNormal = contactNormal.normalized;
        contactPoint = contactPoint / collision.contactCount;
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        hasContact = false;
    }
}
