using UnityEngine;
using UnityEngine.Assertions;
using System.Collections;
using System.Collections.Generic;
using Prime31;

public class AirmanBoss : MonoBehaviour 
{
	#region Variables

	// Unity Editor Variables
	[SerializeField] protected Rigidbody2D deathParticlePrefab;
	[SerializeField] protected float deathParticleSpeed;
	[SerializeField] protected float touchDamage;
	[SerializeField] protected Transform startMarker;
	[SerializeField] protected Transform endMarker;

    public bool IsJumping { get; set; }
    public bool IsShooting { get; set; }
    public bool IsFlexing { get; set; }
    public bool IsBlowing { get; set; }

    // Protected Instance Variables
    protected int texIndex = 0;
	protected bool isPlayingBeginSequence = false;
	protected bool shouldFillHealthBar = false;
	protected bool hasBeenIntroduced = false;
	protected bool isFighting = false;
    private bool isJumping = false;
    protected float texInterval = 0.1f;
	protected float startFallTime;
	protected float journeyLength;
	protected float hurtingTimer;
	protected Player player;
	protected Health health;
	protected Vector3 startingPosition = Vector3.zero;
	protected Vector3 endPos = Vector3.zero;
    [SerializeField] protected float gravity = 118f;
    [SerializeField] protected float jumpAmount = 10.0f;
    [SerializeField] private float jumpLowAmout = 10f;
    [SerializeField] private float jumpHighAmout = 20f;
    [SerializeField] Transform JumpLeftPosition;
    [SerializeField] Transform JumpRightPosition;
    [SerializeField] Transform JumpMidPosition;
    protected float verticalVelocity;
    [SerializeField] private float moveSpeed = 10f;
    private Vector3 movement= Vector3.zero;
    private Vector3 desiredLocation = Vector3.zero;

    protected Collider2D col = null;
	protected SpriteRenderer rend = null;
	protected Animator anim = null;
	protected List<Transform> windShots = new List<Transform>();
	protected AirmanWindWeapon weapon = null;

	#endregion


	#region MonoBehaviour

	// Constructor
	protected void Awake ()
	{
		GameEngine.AirMan = this;

		rend = gameObject.GetComponent<SpriteRenderer>();
		Assert.IsNotNull(rend);

		col = gameObject.GetComponent<Collider2D>();
		Assert.IsNotNull(col);

		anim = gameObject.GetComponent<Animator>();
		Assert.IsNotNull(anim);

		player = FindObjectOfType<Player>();
		Assert.IsNotNull(player);

		health = gameObject.GetComponent<Health>();
		Assert.IsNotNull(health);

		weapon = gameObject.GetComponent<AirmanWindWeapon>();
		Assert.IsNotNull(weapon);
	}

	// Update is called once per frame
	protected void Update () 
	{
		if (health.IsDead == true)
		{
			return;
		}
		else if (isPlayingBeginSequence == true)
		{
			PlayBeginSequence();
		}
		else if (isFighting)
		{
			// Assign the appropriate texture...
			SetAnimationState();

            // update movement
            UpdateMovement();

			if (health.IsHurting == true)
			{
				if (Time.time - hurtingTimer >= health.HurtingDelay)
				{
					health.IsHurting = false;	
				}
			}
		}
	}

    private bool pointReached = false;
	// 
	protected void OnTriggerEnter2D(Collider2D other) 
	{
		if (other.tag == "Player")
		{
			other.gameObject.SendMessage("TakeDamage", touchDamage);
		}

        if(other.tag == "Point")
        {
            pointReached = true;
        }
	}

	// Called when the behaviour becomes disabled or inactive
	protected void OnDisable()
	{
		GameEngine.AirMan = null;
	}


	#endregion


	#region Public Functions

	public void SetUpAirman()
	{
		//anim["JumpLeft"].speed = 1.65f;
		//anim["JumpRight"].speed = 1.65f;
		
		health.ShowHealthBar = true;
		health.HealthbarPosition = new Vector2(30.0f, 10.0f);
		health.CurrentHealth = 0.0f;
		hasBeenIntroduced = true;
		
		// Stop the stage theme and play the boss theme
		GameEngine.SoundManager.Stop (AirmanLevelSounds.STAGE);
		GameEngine.SoundManager.Play(AirmanLevelSounds.BOSS_MUSIC);
		
		// Stop the player from shooting while healthbar is charging
		player.CanShoot = false;		
		
		// Activate the begin sequence and get the time, positions and length for it...
		startingPosition = startMarker.position;
		isPlayingBeginSequence = true;
		
		startFallTime = Time.time;
		endPos = endMarker.position;
		journeyLength = Vector3.Distance(startingPosition, endPos);
    }

	//
	public void Reset()
	{
		if (hasBeenIntroduced == false)
		{
			return;	
		}
		
		health.Reset();
		health.CurrentHealth = 0.0f;
		weapon.Reset();
		
		transform.position = startingPosition;
		isPlayingBeginSequence = false;
		rend.enabled = true;
		col.enabled = true;
		health.IsDead = false;
		isFighting = false;
		shouldFillHealthBar = false;
		
		GameObject.Find("BossBorder").gameObject.GetComponent<Collider2D>().enabled = true;
		GameObject.Find("BossDoor2").gameObject.SendMessage("Reset");
		GameObject.Find("BossDoorTrigger2").gameObject.GetComponent<Collider2D>().enabled = true;
		GameObject.Find("BossTrigger").gameObject.GetComponent<Collider2D>().enabled = true;
		player.IsExternalForceActive = false;
		
		foreach(Transform wind in windShots)
		{
			Destroy(wind.gameObject);
		}
		windShots.Clear();
		
		gameObject.SetActive (false);
	}

	#endregion


	
	//
	protected void CreateDeathParticle(float speed, Vector3 pos, Vector3 vel)
	{
		Rigidbody2D particle = (Rigidbody2D) Instantiate(deathParticlePrefab, pos, transform.rotation);
		particle.transform.Rotate(90,0,0);
		particle.velocity =  vel * speed;
	}
	
	//
	protected IEnumerator CreateDeathParticles()
	{
		// Before the wait...
		Vector3 p1 = transform.position + Vector3.up;
		Vector3 p2 = transform.position - Vector3.up;
		Vector3 p3 = transform.position + Vector3.right;
		Vector3 p4 = transform.position - Vector3.right;
		
		Vector3 p5 = transform.position + Vector3.up + Vector3.right;
		Vector3 p6 = transform.position + Vector3.up - Vector3.right;
		Vector3 p7 = transform.position - Vector3.up - Vector3.right;
		Vector3 p8 = transform.position - Vector3.up + Vector3.right;
		
		p1.z = p2.z = -5;
		p3.z = p4.z = -7;
		p5.z = p6.z = p7.z = p8.z = -9;
		
		CreateDeathParticle(deathParticleSpeed, p1, (transform.up));
		CreateDeathParticle(deathParticleSpeed, p2, (-transform.up));
		CreateDeathParticle(deathParticleSpeed, p3, (transform.right));
		CreateDeathParticle(deathParticleSpeed, p4, (-transform.right));
		CreateDeathParticle(deathParticleSpeed, p5, (transform.up + transform.right));
		CreateDeathParticle(deathParticleSpeed, p6, (transform.up - transform.right));
		CreateDeathParticle(deathParticleSpeed, p7, (-transform.up - transform.right));
		CreateDeathParticle(deathParticleSpeed, p8, (-transform.up + transform.right));
		
		// Start the wait...
		yield return new WaitForSeconds(0.7f);
		
		// After the wait...
		CreateDeathParticle(deathParticleSpeed / 2.5f, p1, transform.up);
		CreateDeathParticle(deathParticleSpeed / 2.5f, p2,-transform.up);
		CreateDeathParticle(deathParticleSpeed / 2.5f, p3, transform.right);
		CreateDeathParticle(deathParticleSpeed / 2.5f, p4,-transform.right);
	}
	
	//
	protected IEnumerator PlayEndMusic() {
		// Before the wait...
		rend.enabled = false;
		col.enabled = false;
		GameEngine.SoundManager.Stop(AirmanLevelSounds.BOSS_MUSIC);
		GameEngine.SoundManager.Play(AirmanLevelSounds.DEATH);
		
		// Start the wait...
		yield return new WaitForSeconds(3.0f);
		
		// After the wait...
		GameEngine.SoundManager.Play(AirmanLevelSounds.STAGE_END);
		
		// Another wait...
		yield return new WaitForSeconds(6.5f);
		
		// Reload the level...
		player.IsPlayerInactive = true;
		player.PlayEndSequence();
		Destroy(gameObject);
	}
	
	//
	protected void KillRobot()
	{
		weapon.Reset();
		StartCoroutine(CreateDeathParticles());
		StartCoroutine(PlayEndMusic());
	}
	
	//
	protected void TakeDamage (float dam)
	{
		// Make sure that shots can not kill the boss twice...
		if (health.CurrentHealth > 0.0f && health.IsHurting == false)
		{
			GameEngine.SoundManager.Play(AirmanLevelSounds.BOSS_HURTING);
			health.ChangeHealth (- dam);
			hurtingTimer = Time.time;
			
			if (health.CurrentHealth <= 0.0f)
			{
				KillRobot();	
			}
		}
	}
	
	/* Make the boss fall down, flex his muscles a little and fill his health bar */
	protected void PlayBeginSequence()
	{
		if (shouldFillHealthBar == true)
		{
            // Make the robot flex his muscles a little bit...
            anim.SetBool("Shoot", false);
            anim.SetBool("Stand", false);
            anim.SetBool("Flex", false);
            anim.Play("Flex");
            // Fill up the health bar...
            if (health.CurrentHealth < health.MaximumHealth)
			{
				health.CurrentHealth = (health.CurrentHealth + 2.0f);
			}
			
			// If the health bar is full, make the robot start to fight!
			else
			{
				GameEngine.SoundManager.Stop(AirmanLevelSounds.HEALTHBAR_FILLING);
				isPlayingBeginSequence = false;
				isFighting = true;
				player.CanShoot = true;
                player.IsFrozen = false;
				GameObject.Find("BossBorder").gameObject.GetComponent<Collider2D>().enabled = false;
				weapon.Attack();
			}
		}
		
		// Make the boss fall down...
		else
		{
            if(player.IsGrounded)
            {
                player.IsFrozen = true;
                
                player.CanShoot = false;
            }
			float distCovered = (Time.time - startFallTime) * 50.0f;
	        float fracJourney = distCovered / journeyLength;
			transform.position = Vector3.Lerp(startingPosition, endPos, fracJourney);

            if(transform.position.y <= endMarker.position.y)
			{
				shouldFillHealthBar = true;
				GameEngine.SoundManager.Play(AirmanLevelSounds.HEALTHBAR_FILLING);
			}
		}
	}
	
	//	
	protected void SetAnimationState()
	{
		if (weapon.ShouldJump == true)
		{
            if(!isJumping && IsJumping)
                StartCoroutine("JumpRoutine");	
		}
		else if (weapon.ShouldShoot == true)
		{
            anim.SetBool("Shoot", true);
            anim.SetBool("Stand", false);
            anim.SetBool("Blow", false);
        }
		else if (weapon.ShouldBlow == true)
		{
            anim.SetBool("Shoot", false);
            anim.SetBool("Stand", false);
            anim.SetBool("Blow", true); ;
		}
		else if( !isJumping )
		{
            anim.SetBool("Shoot", false);
            anim.SetBool("Stand", true);
            anim.SetBool("Blow", false); ;
        }
		
		if (health.IsHurting == true)
		{
			rend.color *= 0.75f + Random.value;
		}
		
		rend.flipX = !weapon.IsTurningLeft;
	}

    private void UpdateMovement()
    {

        // 
        verticalVelocity = movement.y;

        if (isJumping)
        {
            movement = (desiredLocation - transform.position) * moveSpeed;
        }
        if (GetComponent<CharacterController2D>().isGrounded)
        {
            movement = Vector3.zero;
            verticalVelocity = jumpAmount;
        }

        movement = new Vector3(movement.x, verticalVelocity, 0f);
        // apply gravity
        movement = new Vector3(movement.x, (movement.y - gravity * Time.deltaTime), movement.z);

        // update position
        GetComponent<CharacterController2D>().move( movement * Time.deltaTime);
        
    }

    private IEnumerator JumpRoutine()
    {
        // small jump
        isJumping = true;
        anim.SetBool("IsJumping", true);
        desiredLocation = JumpMidPosition.position;
        jumpAmount = jumpLowAmout;
        yield return new WaitForEndOfFrame();

        jumpAmount = 0f;
        pointReached = false;
        yield return new WaitUntil(new System.Func<bool>(() => 
        {
            //bool returnValue = rend.flipX ? transform.position.y >= desiredLocation.y : transform.position.y <= desiredLocation.y;
            //returnValue &= rend.flipX 
            //? transform.position.x >= desiredLocation.x - 1  
            //: transform.position.x <= desiredLocation.x + 1;
            return pointReached;
        }));

        
        anim.SetBool("IsJumping", false);
        anim.SetBool("Flex", true);
        yield return new WaitForSeconds(1f);

        // high jump
        anim.SetBool("IsJumping", true);
        anim.SetBool("Flex", false);
        desiredLocation = rend.flipX ? JumpRightPosition.position : JumpLeftPosition.position;
        jumpAmount = jumpHighAmout;

        yield return new WaitForSeconds(.25f);

        yield return new WaitForEndOfFrame();

        jumpAmount = 0f;

        yield return new WaitUntil(new System.Func<bool>(() => 
        {
            //bool returnValue = rend.flipX ? transform.position.y >= desiredLocation.y : transform.position.y <= desiredLocation.y;
            //returnValue &= rend.flipX ? transform.position.x >= desiredLocation.x -1: transform.position.x <= desiredLocation.x+1;
            return pointReached;
        }));
        pointReached = false;
        anim.SetBool("IsJumping", false);
        anim.Play("Wait");
        isJumping = false;
        IsJumping = false;
        jumpAmount = 0f;
        // done
    }
}
