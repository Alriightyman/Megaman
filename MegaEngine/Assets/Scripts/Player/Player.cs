
using UnityEngine;
using UnityEngine.Assertions;
using System.Collections;
using UnityEngine.SceneManagement;
using Prime31;

// requires a Rigidbody2D simply for the OnTriggerEvents.
[RequireComponent(typeof(Rigidbody2D))]
public class Player : MonoBehaviour
{
    #region Variables

    // Unity Editor Variables

    [SerializeField] protected Rigidbody2D deathParticlePrefab;
    [SerializeField] private float deathParticleSpeed = 75f;
    [SerializeField] private Color deathParticleColor = new Color(0f, 0f, 1f);

    [SerializeField] private float leavingSpeed = 300f;
   

    // Public Properties
    public bool IsPlayerInactive { get; set; }
    public bool IsFrozen { get { return movement.IsFrozen; } set { movement.IsFrozen = value; } }
    public bool IsExternalForceActive { get { return movement.IsExternalForceActive; } set { movement.IsExternalForceActive = value; } }
    public bool IsDead { get { return health.IsDead; } set { health.IsDead = value; } }
    public bool CanShoot { get { return shooting.CanShoot; } set { shooting.CanShoot = value; } }
    public bool IsInvincible { get; set; }
    public bool IsGrounded { get { return movement.isGrounded; } }
    public float CurrentHealth { get { return health.CurrentHealth; } set { health.CurrentHealth = value; } }
    public Vector3 ExternalForce { get { return movement.ExternalForce; } set { movement.ExternalForce = value; } }
    public Vector3 CheckpointPosition { get { return movement.CheckPointPosition; } set { movement.CheckPointPosition = value; } }
    public bool IsLeaving { get; set; }


    // Protected Instance Variables
    protected int walkingTexIndex = 0;
    protected int standingTexIndex = 0;
    protected float walkingTexInterval = 0.2f;
    protected float standingTexInterval = 0.3f;
    protected Health health = null;
    protected BoxCollider2D col = null;
    protected Movement movement = null;
    protected Shooting shooting = null;
    protected LevelCamera levelCamera = null;
    protected bool startPlaying = false;

    private CharacterController2D controller;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private int lives = 3;
    private static Player Instance = null;
    #endregion


    #region MonoBehaviour

    // Constructor 
    protected void Awake()
    {
        GameEngine.Player = this;

        Assert.IsNotNull(deathParticlePrefab);

        levelCamera = FindObjectOfType<LevelCamera>();
        Assert.IsNotNull(levelCamera);

        movement = gameObject.GetComponent<Movement>();
        Assert.IsNotNull(movement);

        shooting = gameObject.GetComponent<Shooting>();
        Assert.IsNotNull(shooting);

        health = gameObject.GetComponent<Health>();
        Assert.IsNotNull(health);

        col = GetComponent<BoxCollider2D>();
        Assert.IsNotNull(col);

        animator = GetComponent<Animator>();
        Assert.IsNotNull(animator);

        controller = GetComponent<CharacterController2D>();

        spriteRenderer = GetComponent<SpriteRenderer>();

        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            DontDestroyOnLoad(gameObject);
        }

    }

    // Use this for initialization 
    protected void Start()
    {
        IsPlayerInactive = false;
        health.HealthbarPosition = new Vector2(10, 10);
        health.ShowHealthBar = true;
        movement.IsEnteringLevel = true;
        StartCoroutine("EnterLevel");
    }

    // Update is called once per frame 
    protected void Update()
    {

        if (IsPlayerInactive == false)
        {
            // Handle the horizontal and Vertical movements
            movement.HandleMovement();

            if (startPlaying == true)
            {

                // Handle shooting
                if ((Input.GetKeyDown(KeyCode.LeftControl) || Input.GetKeyDown(KeyCode.LeftShift) || Input.GetButtonDown("Fire1")) && shooting.CanShoot == true)
                {
                    shooting.Shoot(movement.IsTurningLeft);
                }

                // Handle health
                if (health.IsHurting)
                {
                    if (Time.time - health.HurtingTimer >= health.HurtingDelay)
                    {
                        movement.IsHurting = false;
                        health.IsHurting = false;
                        CanShoot = true;

                        StartCoroutine("InvincibleBlink");

                        Invoke("ResetInvincibility", 3f);
                    }
                }

                GetComponent<SpriteRenderer>().flipX = !movement.IsTurningLeft;
                
            }
        }

        // Setup Animations
        SetAnimationState();

        // keep the z axis as zero <-- Should always be zero.
        if (transform.position.z != 0)
        {
            Vector3 pos = transform.position;
            pos.z = 0;
            transform.position = pos;
        }
    }
    private IEnumerator InvincibleBlink()
    {
        while (true)
        {
            var color = GetComponent<SpriteRenderer>().color;
            color.a = (color.a == 0f) ? 1f : 0f;
            GetComponent<SpriteRenderer>().color = color;
            yield return new WaitForSeconds(0f);
        }
        //
    }

    private IEnumerator EnterLevel()
    {
        Time.timeScale = 0;        
        animator.SetTrigger("EnteringLevel");
        yield return new WaitUntil(new System.Func<bool>(() => controller.isGrounded));

        animator.SetBool("IsGrounded", controller.isGrounded);

        // wait for standing animation to start
        yield return new WaitUntil(new System.Func<bool>(() => { return animator.GetCurrentAnimatorStateInfo(0).IsName("Standing"); }));
        Time.timeScale = 1;
        startPlaying = true;
        movement.IsEnteringLevel = false;
        GameEngine.LevelStarting = false;

    }

    private void ResetInvincibility()
    {
        IsInvincible = false;

        StopCoroutine("InvincibleBlink");

        var color = GetComponent<SpriteRenderer>().color;
        color.a = 1f;
        GetComponent<SpriteRenderer>().color = color;

    }
    // Called when the behaviour becomes disabled or inactive
    protected void OnDisable()
    {
        GameEngine.Player = null;
    }

    #endregion


    #region Protected Functions

    // 
    protected void Reset()
    {
        health.Reset();
        movement.Reset();
        shooting.Reset();
        animator.StopPlayback();
        IsPlayerInactive = false;

    }

    // 
    protected void CreateDeathParticle(float speed, Vector3 pos, Vector3 vel)
    {
        Rigidbody2D particle = (Rigidbody2D)Instantiate(deathParticlePrefab, pos, transform.rotation);
        Physics2D.IgnoreCollision(particle.GetComponent<Collider2D>(), col);
        var deathPart = particle.GetComponent<DeathParticle>();
        deathPart.Color = deathParticleColor;
        particle.velocity = vel * speed;
    }

    // 
    protected IEnumerator CreateDeathParticles(Vector3 pos)
    {
        // Before the wait...
        Vector3 p1 = pos + Vector3.up;
        Vector3 p2 = pos - Vector3.up;
        Vector3 p3 = pos + Vector3.right;
        Vector3 p4 = pos - Vector3.right ;

        Vector3 p5 = pos + Vector3.up  + Vector3.right ;
        Vector3 p6 = pos + Vector3.up  - Vector3.right ;
        Vector3 p7 = pos - Vector3.up  - Vector3.right ;
        Vector3 p8 = pos - Vector3.up  + Vector3.right ;


        this.CreateDeathParticle(deathParticleSpeed, p1, (transform.up));
        this.CreateDeathParticle(deathParticleSpeed, p2, (-transform.up));
        this.CreateDeathParticle(deathParticleSpeed, p3, (transform.right));
        this.CreateDeathParticle(deathParticleSpeed, p4, (-transform.right));
        this.CreateDeathParticle(deathParticleSpeed, p5, (transform.up + transform.right));
        this.CreateDeathParticle(deathParticleSpeed, p6, (transform.up - transform.right));
        this.CreateDeathParticle(deathParticleSpeed, p7, (-transform.up - transform.right));
        this.CreateDeathParticle(deathParticleSpeed, p8, (-transform.up + transform.right));

        spriteRenderer.enabled = false;
        // Start the wait...
        yield return new WaitForSeconds(0.7f);

        // After the wait...
        this.CreateDeathParticle(deathParticleSpeed / 2.5f, p1, transform.up);
        this.CreateDeathParticle(deathParticleSpeed / 2.5f, p2, -transform.up);
        this.CreateDeathParticle(deathParticleSpeed / 2.5f, p3, transform.right);
        this.CreateDeathParticle(deathParticleSpeed / 2.5f, p4, -transform.right);
    }

    // 
    protected IEnumerator MovePlayerUp()
    {
        animator.SetBool("LeaveLevel", true);

        IsLeaving = true;
        GameEngine.SoundManager.Play(AirmanLevelSounds.LEAVE_LEVEL);

        yield return new WaitUntil(new System.Func<bool>(() => { return animator.GetCurrentAnimatorStateInfo(0).IsName("LeavingLevel"); }));

        while (true)
        {
            transform.position += Vector3.up * leavingSpeed * Time.deltaTime;
            yield return null;
        }
    }

    // TODO: Fix
    protected IEnumerator MakeThePlayerLeaveStageRoutine()
    {


        StartCoroutine(MovePlayerUp());

        yield return new WaitUntil(new System.Func<bool>(() => { return spriteRenderer.isVisible == false; }));

        IsPlayerInactive = false;

        SceneManager.LoadScene(0);
    }

    // 
    protected IEnumerator WaitAndResetRoutine()
    {
        // Before the wait... 
        health.IsDead = true;
        movement.IsFrozen = true;
        //playerTexRend.enabled = false;
        levelCamera.ShouldStayStill = true;
        shooting.CanShoot = false;

        yield return new WaitForSeconds(3.6f);
        GameEngine.LevelStarting = true;

        // Reset the camera
        levelCamera.Reset();

        // Reset the player
        Reset();

        //SceneManager.LoadScene(0);
    }

    protected void SetAnimationState()
    {
        if (!IsPlayerInactive)
        {
            var characterController = GetComponent<CharacterController2D>();

            animator.SetBool("IsHurt", health.IsHurting && !health.IsDead);
            animator.SetFloat("WalkingSwitch", (movement.IsWalking && shooting.IsShooting) ? 1f : 0f);
            animator.SetBool("IsWalking", movement.IsWalking);
            animator.SetBool("IsJumping", movement.IsJumping);
            animator.SetBool("IsFalling", movement.IsFalling);
            animator.SetBool("IsShooting", shooting.IsShooting);
            animator.SetBool("IsGrounded", characterController.isGrounded);
        }
        else if(!IsLeaving)
        {
            // reset all animation properties
            animator.SetBool("IsHurt", false);
            animator.SetFloat("WalkingSwitch", 0f);
            animator.SetBool("IsWalking", false);
            animator.SetBool("IsJumping", false);
            animator.SetBool("IsFalling", false);
            animator.SetBool("IsShooting", false);
            animator.SetBool("IsGrounded", false);
            animator.Play("Standing");
        }
    }

   #endregion

    #region Public Functions

    // 
    public void PlayEndSequence()
    {
        StartCoroutine(MakeThePlayerLeaveStageRoutine());
    }

    // 
    public void KillPlayer()
    {
        IsInvincible = false;
        StartCoroutine(CreateDeathParticles(transform.position));
        StartCoroutine(WaitAndResetRoutine());
    }

    // 
    public void TakeDamage(float damage)
    {
        // If the player isn't already hurting or dead...
        if (health.IsHurting == false && health.IsDead == false && IsInvincible == false)
        {
            GameEngine.SoundManager.Play(AirmanLevelSounds.HURTING);
            health.ChangeHealth(-damage);
            movement.IsHurting = true;
            IsInvincible = true;
            CanShoot = false;

            if (health.IsDead == true)
            {
                KillPlayer();
            }
        }
    }

    public void RevivePlayer()
    {
        GameEngine.SoundManager.Play(AirmanLevelSounds.STAGE);
        levelCamera.ShouldStayStill = false;
        spriteRenderer.enabled = true;
        transform.position = GameEngine.Player.CheckpointPosition;
        animator.Play("EnterLevel");
        StartCoroutine("EnterLevel");
        GameEngine.LevelStarting = true;
    }
    #endregion


}