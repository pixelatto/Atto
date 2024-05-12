using System.Collections;
using UnityEngine;
using System.Collections.Generic;

[ExecuteAlways]
[RequireComponent(typeof(CircleCollider2D), typeof(Rigidbody2D))]
public class Actor : MonoBehaviour, IControllable
{
    [Header("Main")]
    public ActorFacing facing;
    public int weight = 10;
    public float pixelSize = 3; [HideInInspector] public float pixelSizeModifier = 0;
    public float horizontalAirDrag = 0.01f;

    [Header("Powers")]
    public float walkPower = 2f;
    public bool canWalk => walkPower > 0;
    public float runPower = 4f;
    public bool canRun => runPower > 0;
    public bool canSprint => sprintPower > 0;
    public float sprintPower = 6f;
    public bool canJump => jumpPower > 0;
    public float jumpPower = 10f;
    public bool canFly => flyPower > 0;
    public float flyPower = 0;
    public bool canCrawl => crawlPower > 0;
    public float crawlPower = 1f;
    public bool canRoll => rollPower > 0;
    public float rollPower = 1.2f; public float rollDuration => rollPower / 3f;

    [HideInInspector] public bool wantsToGoUp = false;
    [HideInInspector] public bool wantsToGoDown = false;
    [HideInInspector] public bool wantsToRideDown = false;
    [HideInInspector] public bool wantsToJump = false;

    [HideInInspector] public bool isGrounded = false;
    [HideInInspector] public bool isUnstable = false;
    [HideInInspector] public Momentum horizontalMomentum = Momentum.None;
    [HideInInspector] public Momentum verticalMomentum = Momentum.None;
    [HideInInspector] public Rigidbody2D rb;
    [HideInInspector] public bool isCloudWalkAvailable => weight < 5;
    [HideInInspector] public bool isJumpAvailable => ((canJump && isGrounded && (timeGrounded > minGroundTimeBeforeJump)) || canFly);
    [HideInInspector] public bool isMovingRight => rb.velocity.x > Global.slowMomentumThreeshold;
    [HideInInspector] public bool isMovingLeft => rb.velocity.x < -Global.slowMomentumThreeshold;
    [HideInInspector] public bool isMovingUp => rb.velocity.y > Global.verticalMomentumThreeshold;
    [HideInInspector] public bool isMovingDown => rb.velocity.y < -Global.verticalMomentumThreeshold;

    public float horizontalSpeed => Mathf.Abs(rb.velocity.x);
    public float verticalSpeed => Mathf.Abs(rb.velocity.y);
    public float radius => pixelSize.PixelsToUnits() + pixelSizeModifier.PixelsToUnits();

    public CircleCollider2D mainCollider { get; private set; }

    Controller controller;

    float minGroundTimeBeforeJump = 0.05f;
    float timeGrounded = 0;

    Vector2 standingPoint;
    Vector2 groundNormal;

    LayerMask walkMask => LayerMask.GetMask("Terrain", isCloudWalkAvailable ? "Clouds" : "");

    [HideInInspector] public GameObject currentRide;

    public CameraTarget cameraTarget { get { if (cameraTarget_ == null) { cameraTarget_ = GetComponent<CameraTarget>(); }; return cameraTarget_; } }
    private CameraTarget cameraTarget_;

    public bool isCameraTarget => cameraTarget != null;

    public ActorStates currentState => stateMachine.currentStateLabel;
    public StateMachine<ActorStates> stateMachine;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        mainCollider = GetComponent<CircleCollider2D>();
        InitStateMachine();
    }

    private void InitStateMachine()
    {
        stateMachine = new StateMachine<ActorStates>();
        stateMachine.AddState
        (
            ActorStates.Grounded, () =>
            {
                LookTowardsMovement();
                HorizontalMovement();
                UpdateGroundedMaterial();
                CheckForCrawling();
                LimitToSprintSpeed();
                CheckForAirborne();
            }
        );
        stateMachine.AddState
        (
            ActorStates.Crawling, () =>
            {
                LookTowardsMovement();
                HorizontalMovement(crawlPower);
                CheckForRolling();
                CheckForStandUp();
                CheckForAirborne();
            }
        );
        stateMachine.AddState
        (
            ActorStates.Airborne, () =>
            {
                LookTowardsMovement();
                HorizontalMovement();
                UpdateAirborneMaterial();
                HorizontalAirDrag();
                CheckForGrounded();
            }
        );
        stateMachine.AddState
        (
            ActorStates.Rolling, () =>
            {
                UpdateRollingMaterial();
                CheckForEndRolling();
            }
        );
    }

    private void CheckForRolling()
    {
        if (horizontalMomentum >= Momentum.Medium && canRoll)
        {
            rb.velocity = new Vector2(rb.velocity.x * rollPower, rb.velocity.y);
            stateMachine.ChangeState(ActorStates.Rolling);
        }
    }

    private void CheckForStandUp()
    {
        if (!wantsToGoDown)
        {
            stateMachine.ChangeState(ActorStates.Grounded);
        }
    }

    private void CheckForEndRolling()
    {
        if (((stateMachine.timeInCurrentState > rollDuration) && (horizontalMomentum == Momentum.None || !isGrounded || !wantsToGoDown)) || stateMachine.timeInCurrentState > rollDuration * 2.75f)
        {
            stateMachine.ChangeState(ActorStates.Grounded);
        }
    }

    private void CheckForGrounded()
    {
        if (isGrounded)
        {
            stateMachine.ChangeState(ActorStates.Grounded);
        }
    }

    private void CheckForAirborne()
    {
        if (!isGrounded)
        {
            stateMachine.ChangeState(ActorStates.Airborne);
        }
    }

    private void LimitToSprintSpeed()
    {
        if (horizontalSpeed > sprintPower)
        {
            rb.velocity = new Vector2(Mathf.Sign(rb.velocity.x) * sprintPower, rb.velocity.y);
        }
    }

    private void CheckForCrawling()
    {
        if (canCrawl && wantsToGoDown)
        {
            stateMachine.ChangeState(ActorStates.Crawling);
        }
    }

    private void HorizontalAirDrag()
    {
        if (controller != null && controller.horizontalAxis == 0)
        {
            rb.velocity -= new Vector2(rb.velocity.x * horizontalAirDrag * Time.fixedDeltaTime, 0);
        }
    }

    private void UpdateAirborneMaterial()
    {
        rb.sharedMaterial = Global.slipperyMaterial;
        rb.freezeRotation = false;
    }

    private void UpdateRollingMaterial()
    {
        rb.sharedMaterial = Global.rollMaterial;
        rb.freezeRotation = true;
        rb.rotation = 0;
    }

    private void UpdateGroundedMaterial()
    {
        if (controller != null)
        {
            if (controller.horizontalAxis != 0)
            {
                rb.sharedMaterial = Global.slipperyMaterial;
                rb.freezeRotation = false;
            }
            else
            {
                rb.sharedMaterial = isUnstable ? Global.unstableMaterial : Global.stickyMaterial;
                rb.freezeRotation = true;
                rb.rotation = 0;
            }
        }
    }

    void LookTowardsMovement()
    {
        if (controller != null)
        {
            if (controller.horizontalAxis < 0)
            {
                LookLeft();
            }
            if (controller.horizontalAxis > 0)
            {
                LookRight();
            }
        }
    }

    private void HorizontalMovement(float overrideSpeed = 0)
    {
        if (controller!= null && controller.horizontalAxis != 0)
        {
            var speed = controller.actionHeld ? sprintPower : runPower;
            if (overrideSpeed != 0)
            {
                speed = overrideSpeed;
            }
            rb.velocity = new Vector2((Vector2.right * speed * controller.horizontalAxis).x, rb.velocity.y);
        }
    }

    void Update()
    {
        if (Application.isPlaying)
        {
            UpdateStateMachine();
            UpdateControls();
            UpdateMomentums();
        }
        UpdateCollider();
    }

    void FixedUpdate()
    {
        UpdatePhysics();
    }

    void UpdateCollider()
    {
        if (mainCollider == null)
        {
            mainCollider = GetComponent<CircleCollider2D>();
        }
        mainCollider.radius = radius;
        bool hasReducedHitBox = currentState == ActorStates.Crawling || currentState == ActorStates.Rolling;
        pixelSizeModifier = hasReducedHitBox ? -0.5f : 0;
    }

    private void UpdateStateMachine()
    {
        stateMachine.Update();
    }

    private void UpdateControls()
    {
        if (controller != null)
        {
            wantsToGoDown = controller.verticalAxis < 0;
            wantsToGoUp = controller.verticalAxis > 0;
            wantsToRideDown = wantsToGoDown && controller.actionHeld;

            if (!wantsToJump)
            {
                if (controller.jumpPressed)
                {
                    wantsToJump = true;
                }
            }
            else
            {
                wantsToJump = controller.jumpHeld;
            }
        }
    }

    void UpdatePhysics()
    {
        CheckCloudWalk();
        CheckGround();
        CheckJump();
    }

    private void CheckGround()
    {
        isGrounded = false;
        groundNormal = Vector2.zero;
        var extrapolation = 4.PixelsToUnits();
        var feetSeparation = radius * 0.5f;

        standingPoint = transform.position;
        var leftOrigin = transform.position + Vector3.left * feetSeparation;
        var rightOrigin = transform.position + Vector3.right * feetSeparation;
        var leftHitPoint = leftOrigin + Vector3.down * (radius + extrapolation);
        var rightHitPoint = rightOrigin + Vector3.down * (radius + extrapolation);
        var leftHit = Physics2D.Raycast(leftOrigin, Vector2.down, radius + extrapolation, walkMask);
        var rightHit = Physics2D.Raycast(rightOrigin, Vector2.down, radius + extrapolation, walkMask);
        var heightToFloor = (standingPoint - (Vector2)transform.position).magnitude - radius;
        if (leftHit) { leftHitPoint = leftHit.point; }
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
        }
        else
        {
            timeGrounded = 0;
        }
    }

    private void CheckCloudWalk()
    {
        var nearbyClouds = Scan.NearbyObjects(transform.position, radius * 2f, Global.cloudsMask);

        if (nearbyClouds.Length > 0)
        {
            foreach (var cloud in nearbyClouds)
            {
                Physics2D.IgnoreCollision(mainCollider, cloud, !isCloudWalkAvailable);
            }
        }
    }

    private void CheckJump()
    {
        if (wantsToJump && isJumpAvailable)
        {
            float power = 0;
            if (isGrounded)
            {
                power = jumpPower;
            }
            else
            {
                power = flyPower;
            }

            if (rb.velocity.y > 0)
            {
                rb.velocity += Vector2.up * power;
            }
            else
            {
                rb.velocity = new Vector2(rb.velocity.x, power);
            }
            rb.velocity = new Vector2(rb.velocity.x, Mathf.Min(rb.velocity.y, power));
            rb.angularVelocity = 0;
            wantsToJump = false;
        }
    }

    private void UpdateMomentums()
    {
        horizontalMomentum = Global.ClassifyMomentum(horizontalSpeed);
        verticalMomentum   = Global.ClassifyMomentum(verticalSpeed);
    }

    public void SetControl(Controller controller)
    {
        this.controller = controller;
    }

    private void LookRight()
    {
        facing = ActorFacing.Right;
    }

    private void LookLeft()
    {
        facing = ActorFacing.Left;
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
            dustParticle.speed = Random.insideUnitCircle - rb.velocity.normalized + Vector2.up*3f;
            standingCell.Destroy();
        }
    }

}
