using System.Collections;
using UnityEngine;
using System.Collections.Generic;

[ExecuteAlways]
[RequireComponent(typeof(CircleCollider2D), typeof(Rigidbody2D))]
public class Actor : MonoBehaviour, IControllable
{
    [Header("Main")]
    public ActorFacing facing;
    public ActorWeights weight = ActorWeights.Medium; public enum ActorWeights { Tiny = 1, Light = 2, Medium = 3, Heavy = 4, Massive = 5 }
    public float pixelSize = 3; [HideInInspector] public float pixelSizeModifier = 0;
    public float horizontalAirDrag = 0.01f;

    [HideInInspector] public bool wantsToGoUp = false;
    [HideInInspector] public bool wantsToGoDown = false;
    [HideInInspector] public bool wantsToRideDown = false;
    [HideInInspector] public bool wantsToJump = false;

    [HideInInspector] public bool isGrounded = false;
    [HideInInspector] public bool isUnstable = false;
    [HideInInspector] public Momentum horizontalMomentum = Momentum.None;
    [HideInInspector] public Momentum verticalMomentum = Momentum.None;
    [HideInInspector] public Rigidbody2D rb;
    [HideInInspector] public bool isCloudWalkAvailable => weight <= ActorWeights.Light;
    [HideInInspector] public bool isJumpAvailable => ((Can(Skill.Jump) && isGrounded && (timeGrounded > minGroundTimeBeforeJump)) || Can(Skill.Fly));
    [HideInInspector] public bool isMovingRight => rb.velocity.x > Global.slowMomentumThreeshold;
    [HideInInspector] public bool isMovingLeft => rb.velocity.x < -Global.slowMomentumThreeshold;
    [HideInInspector] public bool isMovingUp => rb.velocity.y > Global.verticalMomentumThreeshold;
    [HideInInspector] public bool isMovingDown => rb.velocity.y < -Global.verticalMomentumThreeshold;

    public float horizontalSpeed => Mathf.Abs(rb.velocity.x);
    public float verticalSpeed => Mathf.Abs(rb.velocity.y);
    public float radius => pixelSize.PixelsToUnits() + pixelSizeModifier.PixelsToUnits();

    public CircleCollider2D mainCollider { get; private set; }

    Controller controller;

    float defaultHorizontalSpeed = 0;
    float fastHorizontalSpeed = 0;
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

    Dictionary<Skill, SkillBase> skills = new Dictionary<Skill, SkillBase>();

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        mainCollider = GetComponent<CircleCollider2D>();
        RegisterSkills();
        InitStateMachine();
    }

    private void RegisterSkills()
    {
        foreach (var skill in GetComponentsInChildren<SkillBase>())
        {
            skills.Add(skill.skillType, skill);
        }
    }

    private void InitStateMachine()
    {
        stateMachine = new StateMachine<ActorStates>();
        stateMachine.AddState
        (
            ActorStates.Grounded, () =>
            {
                SetSpeeds(skills[Skill.Walk].power, Can(Skill.Run) ? skills[Skill.Run].power : skills[Skill.Walk].power);
                LookTowardsMovement();
                HorizontalMovement();
                UpdateGroundedMaterial();
                CheckForCrawling();
                LimitGroundSpeed();
                CheckForAirborne();
            }
        );
        stateMachine.AddState
        (
            ActorStates.Crawling, () =>
            {
                SetSpeeds(skills[Skill.Crawl].power);
                LookTowardsMovement();
                HorizontalMovement();
                CheckForRolling();
                CheckForStandUp();
                CheckForAirborne();
            }
        );
        stateMachine.AddState
        (
            ActorStates.Airborne, () =>
            {
                SetSpeeds(skills[Skill.Walk].power, Can(Skill.Run) ? skills[Skill.Run].power : skills[Skill.Walk].power);
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

    private void SetSpeeds(float defaultSpeed, float fastSpeed = 0)
    {
        defaultHorizontalSpeed = defaultSpeed;
        if (fastSpeed != 0)
        {
            fastHorizontalSpeed = fastSpeed;
        }
        else
        {
            fastHorizontalSpeed = defaultHorizontalSpeed;
        }
    }

    private void CheckForRolling()
    {
        if (horizontalMomentum >= Momentum.Medium && Can(Skill.Roll))
        {
            rb.velocity = new Vector2(rb.velocity.x * skills[Skill.Roll].power, rb.velocity.y);
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
        //HACKVI: Hacky durations
        if (((stateMachine.timeInCurrentState > skills[Skill.Roll].skillDuration) && (horizontalMomentum == Momentum.None || !isGrounded || !wantsToGoDown)) || stateMachine.timeInCurrentState > skills[Skill.Roll].skillDuration * 2.75f)
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

    private void LimitGroundSpeed()
    {
        float maxGroundedSpeed = 0;
        if (Can(Skill.Walk))   { maxGroundedSpeed = skills[Skill.Walk]   .power; }
        if (Can(Skill.Run))    { maxGroundedSpeed = skills[Skill.Run]    .power; }

        if (horizontalSpeed > maxGroundedSpeed)
        {
            rb.velocity = new Vector2(Mathf.Sign(rb.velocity.x) * maxGroundedSpeed, rb.velocity.y);
        }
    }

    private void CheckForCrawling()
    {
        if (Can(Skill.Crawl) && wantsToGoDown)
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

    private void HorizontalMovement()
    {
        if (controller!= null && controller.horizontalAxis != 0)
        {
            var wantsToGoFast = controller != null && controller.actionHeld;
            rb.velocity = new Vector2((Vector2.right * (wantsToGoFast ? fastHorizontalSpeed : defaultHorizontalSpeed) * controller.horizontalAxis).x, rb.velocity.y);
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
                power = skills[Skill.Jump].power;
            }
            else
            {
                power = skills[Skill.Fly].power;
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

    public bool Can(Skill skill)
    {
        if (skills.ContainsKey(skill))
        {
            var targetSkill = skills[skill];
            if (targetSkill.level > 0)
            {
                return true;
            }
        }
        else
        {
            Debug.LogWarning("Skill " + skill.ToString() + " not found.");
        }
        return false;
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
        var belowWorldPosition = standingPoint + Vector2.down * 1.5f.PixelsToUnits();
        var belowPixelPosition = CellularAutomata.WorldToPixelPosition(belowWorldPosition);
        var belowCell = CellularAutomata.instance.GetCell(belowPixelPosition);
        Draw.Pixel(belowWorldPosition, Color.green);
        
        if (belowCell.IsGranular())
        {
            bool canPushGroundParticles = weight >= ActorWeights.Heavy;

            bool pushRealParticle = canPushGroundParticles && Random.value < 0.20f * (int)weight;
            var spawnPoint = (Vector2)standingPoint + Vector2.up * 1.5f.PixelsToUnits();

            Particle dustParticle;
            if (pushRealParticle)
            {
                dustParticle = ParticleAutomata.instance.CellToParticle(belowCell, CellularAutomata.WorldToPixelPosition(spawnPoint));
            }
            else
            {
                dustParticle = ParticleAutomata.instance.CreateParticle(spawnPoint, belowCell.material);
            }

            dustParticle.isEthereal = !pushRealParticle;

            dustParticle.speed = Random.insideUnitCircle - rb.velocity.normalized + Vector2.up*3f;
        }
    }

}
