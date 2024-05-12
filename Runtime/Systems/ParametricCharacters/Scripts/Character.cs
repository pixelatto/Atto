using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
[RequireComponent(typeof(CircleCollider2D), typeof(Rigidbody2D))]
public class Character : MonoBehaviour, IControllable
{
    public Facing facing;
    public bool canFly = false;
    public bool canLevitate = false;
    public bool canCloudWalk = false;
    public bool canCrawl = true;
    public bool canRoll = true;
    public float pixelSize = 3;
    public float crawlSpeed = 1f;
    public float walkSpeed = 2f;

    public float runSpeed = 4f;
    public float sprintSpeed = 6f;
    public float jumpForce = 300f;
    public float horizontalAirDrag = 0.01f;
    public float rollDuration = 0.35f;
    public float rollBoost = 1.2f;
    public CharacterStates characterState;
    public Momentum horizontalMomentum = Momentum.None;
    public Momentum verticalMomentum = Momentum.None;

    public PhysicsMaterial2D brakeMaterial;
    public PhysicsMaterial2D stickyMaterial;
    public PhysicsMaterial2D slipperyMaterial;
    public PhysicsMaterial2D unstableMaterial;
    public PhysicsMaterial2D rollMaterial;

    public bool wantsToGoUp = false;
    public bool wantsToGoDown = false;
    public bool wantsToRideDown = false;
    public bool isGrounded = false;
    public bool isUnstable = false;
    public bool isRolling = false;
    [HideInInspector] public Rigidbody2D rb;
    [HideInInspector] public bool isMovingRight => rb.velocity.x > slowMomentumThreeshold;
    [HideInInspector] public bool isMovingLeft => rb.velocity.x < -slowMomentumThreeshold;
    [HideInInspector] public bool isMovingUp => rb.velocity.y > verticalMomentumThreeshold;
    [HideInInspector] public bool isMovingDown => rb.velocity.y < -verticalMomentumThreeshold;

    public float horizontalSpeed => Mathf.Abs(rb.velocity.x);
    public float verticalSpeed => Mathf.Abs(rb.velocity.y);

    public float currentStateTime => Time.time - lastStateChangeTimestamp;
    public float radius => pixelSize.PixelsToUnits();
    float lastStateChangeTimestamp = 0;

    const float slowMomentumThreeshold = 0.5f;
    const float mediumMomentumThreeshold = 2.5f;
    const float fastMomentumThreeshold = 5f;

    const float verticalMomentumThreeshold = 1f;

    public enum CharacterStates { Idle, Walk, Run, Sprint, Jump, Fall, Sit, Crawl, Roll, Float, Levitate, LevitateJump, LevitateFall, Unstable, Look, Ride }
    public enum Momentum { None, Slow, Medium, Fast }

    public CircleCollider2D mainCollider { get; private set; }
    Controller controller;
    CharacterAppearance appearance;
    CharacterParameters parameters;

    float lastGroundTime = 0;
    float rollStartTime = 0;

    float minGroundTimeBeforeJump = 0.05f;
    float timeGrounded = 0;
    public bool canJump => ((isGrounded && timeGrounded > minGroundTimeBeforeJump) || canFly);

    LayerMask walkMask => LayerMask.GetMask("Terrain", canCloudWalk ? "Clouds" : "");
    LayerMask cloudsMask;

    Vector2 standingPoint;
    Vector2 groundNormal;
    Vector2 groundTangent => groundNormal.RightPerpendicular();

    public bool jumpRequested = false;

    PixelPositioner pixelPositioner;

    [HideInInspector] public GameObject currentRide;

    public CameraTarget cameraTarget { get { if (cameraTarget_ == null) { cameraTarget_ = GetComponent<CameraTarget>(); }; return cameraTarget_; } }
    private CameraTarget cameraTarget_;

    public bool isCameraTarget => cameraTarget != null;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        pixelPositioner = GetComponent<PixelPositioner>();
        appearance = GetComponentInChildren<CharacterAppearance>();
        parameters = GetComponentInChildren<CharacterParameters>();
        mainCollider = GetComponent<CircleCollider2D>();
        cloudsMask = LayerMask.GetMask("Clouds");
    }

    void Update()
    {
        if (Application.isPlaying)
        {
            EditorUpdate();
            RuntimeUpdate();
        }
        else
        {
            EditorUpdate();
        }
    }

    void FixedUpdate()
    {
        UpdatePhysics();
    }

    private void EditorUpdate()
    {
        if (mainCollider == null)
        {
            mainCollider = GetComponent<CircleCollider2D>();
        }
        mainCollider.radius = radius;

        if (facing != Facing.Undefined)
        {
            var spriteTransform = transform.GetChild(0);
            switch (facing)
            {
                case Facing.Left:
                    spriteTransform.localScale = new Vector3(-1, 1, 1);
                    break;
                case Facing.Right:
                    spriteTransform.localScale = new Vector3(1, 1, 1);
                    break;
            }
        }
    }

    void RuntimeUpdate()
    {
        UpdateState();
        UpdateBehaviours();
        UpdateMomentum();
    }

    private void UpdateState()
    {
        if (currentRide != null )
        {
            ChangeState(CharacterStates.Ride);
        }
        else if (isUnstable)
        {
            ChangeState(CharacterStates.Unstable);
        }
        else if (isRolling)
        {
            var elapsedRollTime = Time.time - rollStartTime;
            if (((elapsedRollTime > rollDuration) && (horizontalMomentum == Momentum.None || !isGrounded || !wantsToGoDown)) || elapsedRollTime > rollDuration * 2.75f)
            {
                isRolling = false;
                ChangeState(CharacterStates.Sit);
            }
        }
        else if (isGrounded)
        {
            switch (horizontalMomentum)
            {
                case Momentum.None:
                    if (wantsToGoDown)
                    {
                        ChangeState(CharacterStates.Sit);
                    }
                    else if (canLevitate)
                    {
                        ChangeState(CharacterStates.Float);
                    }
                    else
                    {
                        ChangeState(CharacterStates.Idle);
                    }
                    break;
                case Momentum.Slow:
                    if (wantsToGoDown && canCrawl)
                    {
                        ChangeState(CharacterStates.Crawl);
                    }
                    else
                    {
                        if (canLevitate)
                        {
                            ChangeState(CharacterStates.Levitate);
                        }
                        else
                        {
                            ChangeState(CharacterStates.Walk);
                        }
                    }
                    break;
                case Momentum.Medium:
                    if (canLevitate)
                    {
                        ChangeState(CharacterStates.Levitate);
                    }
                    else
                    {
                        ChangeState(CharacterStates.Run);
                    }
                    break;
                case Momentum.Fast:
                    if (canLevitate)
                    {
                        ChangeState(CharacterStates.Levitate);
                    }
                    else
                    {
                        ChangeState(CharacterStates.Sprint);
                    }
                    break;
            }
        }
        else
        {
            if (isMovingUp)
            {
                if (canLevitate)
                {
                    ChangeState(CharacterStates.LevitateJump);
                }
                else
                {
                    ChangeState(CharacterStates.Jump);
                }
            }
            else
            {
                if (canLevitate)
                {
                    ChangeState(CharacterStates.LevitateFall);
                }
                else
                {
                    ChangeState(CharacterStates.Fall);
                }
            }
        }
    }

    void ChangeState(CharacterStates newState)
    {
        if (newState != characterState)
        {
            lastStateChangeTimestamp = Time.time;
            characterState = newState;
        }
    }

    public bool HasReducedHitBox()
    {
        return characterState == CharacterStates.Crawl || characterState == CharacterStates.Sit || characterState == CharacterStates.Roll;
    }

    private void UpdateBehaviours()
    {
        if (controller != null)
        {
            wantsToGoDown = controller.verticalAxis < 0;
            wantsToGoUp = controller.verticalAxis > 0;
            wantsToRideDown = wantsToGoDown && controller.actionHeld;
            if (isGrounded)
            {
                //TODO: If over slippery material (EG: Ice), set material to slippery

                if (isRolling)
                {
                    rb.sharedMaterial = rollMaterial;
                    rb.freezeRotation = true;
                }
                else if (controller.horizontalAxis != 0)
                {
                    rb.sharedMaterial = slipperyMaterial;
                    rb.freezeRotation = false;
                }
                else
                {
                    rb.sharedMaterial = isUnstable ? unstableMaterial : stickyMaterial;
                    rb.freezeRotation = true;
                    rb.rotation = 0;
                }
            }
            else
            {
                rb.sharedMaterial = slipperyMaterial;
                rb.freezeRotation = false;
                if (controller.horizontalAxis == 0)
                {
                    rb.velocity -= new Vector2(rb.velocity.x * horizontalAirDrag * Time.fixedDeltaTime, 0);
                }
            }

            if (controller.horizontalAxis != 0 && !isRolling)
            {
                var currentSpeed = (wantsToGoDown && isGrounded) ? crawlSpeed : (controller.actionHeld ? sprintSpeed : runSpeed);
                rb.velocity = new Vector2((Vector2.right * currentSpeed * controller.horizontalAxis).x, rb.velocity.y);

                if (controller.horizontalAxis < 0)
                {
                    LookLeft();
                }
                if (controller.horizontalAxis > 0)
                {
                    LookRight();
                }

                if (canRoll)
                {
                    if (isGrounded && wantsToGoDown && horizontalMomentum >= Momentum.Medium)
                    {
                        isRolling = true;
                        rollStartTime = Time.time;
                        rb.velocity = new Vector2(rb.velocity.x * rollBoost, rb.velocity.y);
                        ChangeState(CharacterStates.Roll);
                    }
                }
            }

            if (!jumpRequested)
            {
                if (controller.jumpPressed)
                {
                    jumpRequested = true;
                }
            }
            else
            {
                jumpRequested = controller.jumpHeld;
            }

            if (parameters != null)
            {
                parameters.verticalLookDirection = wantsToGoUp ? 1 : 0;
            }
        }
    }

    void UpdatePhysics()
    {
        CheckCloudWalk();

        isGrounded = false;
        groundNormal = Vector2.zero;
        var extrapolation = 4.PixelsToUnits();
        var feetSeparation = radius*0.5f;

        standingPoint = transform.position;
        var leftOrigin = transform.position + Vector3.left * feetSeparation;
        var rightOrigin = transform.position + Vector3.right * feetSeparation;
        var leftHitPoint = leftOrigin + Vector3.down * (radius+extrapolation);
        var rightHitPoint = rightOrigin + Vector3.down * (radius + extrapolation);
        var leftHit = Physics2D.Raycast(leftOrigin, Vector2.down, radius + extrapolation, walkMask);
        var rightHit = Physics2D.Raycast(rightOrigin, Vector2.down, radius + extrapolation, walkMask);
        var heightToFloor = (standingPoint - (Vector2)transform.position).magnitude - radius;
        if (leftHit)  { leftHitPoint = leftHit.point; }
        if (rightHit) { rightHitPoint = rightHit.point; }
        standingPoint = (leftHitPoint + rightHitPoint) * 0.5f;

        Draw.Vector(leftOrigin, leftHitPoint, leftHit ? Color.yellow : Color.white);
        Draw.Vector(rightOrigin, rightHitPoint, rightHit ? Color.yellow : Color.white);

        var closeToGround = heightToFloor < 3.PixelsToUnits();
        var slope = Vector2.Angle(Vector2.right, (rightHitPoint - leftHitPoint));
        var anyHit = leftHit || rightHit;

        isGrounded = closeToGround && anyHit && slope < 60f;
        isUnstable = closeToGround && anyHit && slope > 45f;
        
        Draw.Vector(transform.position, standingPoint, isGrounded ? Color.red : Color.white);
        Debug.DrawLine(leftHitPoint, rightHitPoint, Color.yellow);
        Draw.Vector(standingPoint, standingPoint + groundNormal * radius * 2f, Color.cyan, 0.25f);

        if (isGrounded)
        {
            timeGrounded += Time.fixedDeltaTime;
            groundNormal = (rightHit.point - leftHit.point).LeftPerpendicular().normalized;
            lastGroundTime = Time.time;
        }
        else
        {
            timeGrounded = 0;
        }

        if (jumpRequested && canJump)
        {
            if (rb.velocity.y > 0)
            {
                rb.velocity += Vector2.up * jumpForce;
            }
            else
            {
                rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            }
            rb.velocity = new Vector2(rb.velocity.x, Mathf.Min(rb.velocity.y, jumpForce));
            rb.angularVelocity = 0;
            jumpRequested = false;
        }

        if (horizontalSpeed > sprintSpeed)
        {
            rb.velocity = new Vector2(Mathf.Sign(rb.velocity.x) * sprintSpeed, rb.velocity.y);
        }
    }

    private void CheckCloudWalk()
    {
        var nearbyClouds = Scan.NearbyObjects(transform.position, radius * 2f, cloudsMask);

        if (nearbyClouds.Length > 0)
        {
            foreach (var cloud in nearbyClouds)
            {
                Physics2D.IgnoreCollision(mainCollider, cloud, !canCloudWalk);
            }
        }
    }

    private void UpdateMomentum()
    {
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

    public void SetControl(Controller controller)
    {
        this.controller = controller;
    }

    private void LookRight()
    {
        facing = Facing.Right;
        if (appearance != null)
        {
            appearance.LookRight();
        }
    }

    private void LookLeft()
    {
        facing = Facing.Left;
        if (appearance != null)
        {
            appearance.LookLeft();
        }
    }

    void OnDrawGizmos()
    {
        if (Application.isPlaying)
        {
            Draw.Circle(standingPoint, 0.25f.PixelsToUnits(), Color.cyan, 8);
            var standingPixelPosition = standingPoint + Vector2.down * 1.5f.PixelsToUnits();
            Draw.Circle(standingPixelPosition, 0.25f.PixelsToUnits(), Color.cyan, 8);
        }
    }

    public void OnRunFootstep()
    {
        var belowStandingPosition = standingPoint + Vector2.down * 2.5f.PixelsToUnits();
        var pixelPosition = CellularAutomata.WorldToPixelPosition(belowStandingPosition);
        var standingCell = CellularAutomata.instance.GetCell(pixelPosition);
        if (standingCell.IsGranular())
        {
            var spawnPoint = standingPoint + Vector2.up * 0.5f.PixelsToUnits();
            var dustParticle = ParticleAutomata.instance.CellToParticle(standingCell, CellularAutomata.WorldToPixelPosition(spawnPoint));
            dustParticle.speed = (UnityEngine.Random.insideUnitCircle - rb.velocity.normalized + Vector2.up*3f);
            standingCell.Destroy();
        }
    }

}

public enum Facing { Left = -1, Undefined = 0, Right = 1 }