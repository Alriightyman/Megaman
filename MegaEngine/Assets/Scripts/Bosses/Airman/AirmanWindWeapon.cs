using UnityEngine;
using UnityEngine.Assertions;
using System.Collections;
using System.Collections.Generic;
using Prime31;

public class AirmanWindWeapon : MonoBehaviour
{
	#region Variables

	// Unity Editor Variables
	public Transform weaponPrefab;
    public Transform projectileSpawnPoint;
    public List<Transform> windAttackTransforms;
    [SerializeField] protected float windPower = 400.0f;

    // Public Properties
    public bool IsTurningLeft { get; set; }
	public bool ShouldShoot { get; set; }
	public bool ShouldJump  { get; set; }
	public bool ShouldBlow  { get; set; }

	// Protected Const Variables
	protected const int NUM_OF_SHOTS_BEFORE_JUMPING = 3;

	// Protected Instance Variables
	protected int shootingCounter = 0;
	protected bool isBlowing;
	protected bool isJumping;
	protected bool isShooting;
	protected bool isFighting;
	protected bool isPlayingAnimation;
	protected bool shouldDestroyWind = false;
	protected float fightingTimer;
	protected float blowDelay = 2.0f;	
	protected float windDestroyDelay = 1.0f;
	protected Player player = null;
	protected Animator anim = null;
	protected List<Vector3> nextAttack;
	protected List<Vector3> attack1Right = new List<Vector3>();
	protected List<Vector3> attack2Right = new List<Vector3>();
	protected List<Vector3> attack3Right = new List<Vector3>();
	protected List<Vector3> attack1Left = new List<Vector3>();
	protected List<Vector3> attack2Left = new List<Vector3>();
	protected List<Vector3> attack3Left = new List<Vector3>();
	protected List<AirmanWind> windShots = new List<AirmanWind>();

	#endregion
	
	
	#region MonoBehaviour

	// Constructor
	protected void Awake ()
	{
		player = FindObjectOfType<Player>();
		Assert.IsNotNull(player);

		anim = GetComponent<Animator>();
		Assert.IsNotNull(anim);
	}
	
	// Use this for initialization
	protected void Start ()
	{
		InitAttackLists();
		nextAttack = attack1Left;

        //anim.Stop();
        anim.StopPlayback();
		shootingCounter = 0;
		
		IsTurningLeft = true;
		isBlowing = false;
		isJumping = false;
		isShooting = false;
		isFighting = false;
		isPlayingAnimation = false;
		shouldDestroyWind = false;
		ShouldShoot = false;
		ShouldJump = false;
		ShouldBlow = false;
	}

	// Update is called once per frame
	protected void Update ()
	{
		if (isFighting == true)
		{
			if (isShooting == true)
			{
				Shoot();
			}
			else if (isBlowing)
			{
				Blow();	
			}
			else if (isJumping == true)
			{
				Jump();	
			}
		}
	}

	#endregion


	#region Protected Functions

	// 
	protected void InitAttackLists()
	{
        // Create the attack 1 list...
        attack1Left.Add(windAttackTransforms[0].localPosition);//new Vector3(-20.0f * 10, 7.0f * 10, 0.0f));
        attack1Left.Add(windAttackTransforms[1].localPosition);//new Vector3(-16.0f * 10, 2.0f * 10, 0.0f));
        attack1Left.Add(windAttackTransforms[2].localPosition);//new Vector3(-14.0f * 10, 5.0f * 10, 0.0f));
        attack1Left.Add(windAttackTransforms[3].localPosition);//new Vector3(-9.0f * 10, 0.0f, 0.0f));
        attack1Left.Add(windAttackTransforms[4].localPosition);//new Vector3(-6.0f * 10, 8.0f * 10, 0.0f));
        attack1Left.Add(windAttackTransforms[5].localPosition);//new Vector3(-3.0f * 10, 4.0f * 10, 0.0f));

        attack1Right.Add(windAttackTransforms[0].localPosition = new Vector3(-windAttackTransforms[0].localPosition.x, 
                                                                              windAttackTransforms[0].localPosition.y, 
                                                                              windAttackTransforms[0].localPosition.z));//new Vector3(20.0f * 10, 7.0f * 10, 0.0f));
        attack1Right.Add(windAttackTransforms[1].localPosition = new Vector3(-windAttackTransforms[1].localPosition.x,
                                                                              windAttackTransforms[1].localPosition.y,
                                                                              windAttackTransforms[1].localPosition.z));//new Vector3(16.0f * 10, 2.0f * 10, 0.0f));
        attack1Right.Add(windAttackTransforms[2].localPosition = new Vector3(-windAttackTransforms[2].localPosition.x,
                                                                              windAttackTransforms[2].localPosition.y,
                                                                              windAttackTransforms[2].localPosition.z));//new Vector3(14.0f * 10, 5.0f * 10, 0.0f));
        attack1Right.Add(windAttackTransforms[3].localPosition = new Vector3(-windAttackTransforms[3].localPosition.x,
                                                                              windAttackTransforms[3].localPosition.y,
                                                                              windAttackTransforms[3].localPosition.z));//new Vector3(9.0f * 10, 0.0f, 0.0f));
        attack1Right.Add(windAttackTransforms[4].localPosition = new Vector3(-windAttackTransforms[4].localPosition.x,
                                                                              windAttackTransforms[4].localPosition.y,
                                                                              windAttackTransforms[4].localPosition.z));//new Vector3(6.0f * 10, 8.0f * 10, 0.0f));
        attack1Right.Add(windAttackTransforms[5].localPosition = new Vector3(-windAttackTransforms[5].localPosition.x,
                                                                              windAttackTransforms[5].localPosition.y,
                                                                              windAttackTransforms[5].localPosition.z));//new Vector3(3.0f * 10, 4.0f * 10, 0.0f));

        // Create the attack 2 list...
        attack2Left.Add(windAttackTransforms[6].localPosition);//new Vector3(-18.0f, 6.0f, 0.0f));
        attack2Left.Add(windAttackTransforms[7].localPosition);//new Vector3(-16.0f, 0.0f, 0.0f));
        attack2Left.Add(windAttackTransforms[8].localPosition);//new Vector3(-12.0f, 12.0f, 0.0f));
        attack2Left.Add(windAttackTransforms[9].localPosition);//new Vector3(-10.0f, 6.5f, 0.0f));
        attack2Left.Add(windAttackTransforms[10].localPosition);//new Vector3(-9.5f, 2.5f, 0.0f));
        attack2Left.Add(windAttackTransforms[11].localPosition);//new Vector3(-3.0f, 0.4f, 0.0f));

        attack2Right.Add(windAttackTransforms[0].localPosition = new Vector3(-windAttackTransforms[6].localPosition.x,
                                                                              windAttackTransforms[6].localPosition.y,
                                                                              windAttackTransforms[6].localPosition.z));//new Vector3(18.0f, 6.0f, 0.0f));
        attack2Right.Add(windAttackTransforms[1].localPosition = new Vector3(-windAttackTransforms[7].localPosition.x,
                                                                              windAttackTransforms[7].localPosition.y,
                                                                              windAttackTransforms[7].localPosition.z));//new Vector3(16.0f, 0.0f, 0.0f));
        attack2Right.Add(windAttackTransforms[2].localPosition = new Vector3(-windAttackTransforms[8].localPosition.x,
                                                                              windAttackTransforms[8].localPosition.y,
                                                                              windAttackTransforms[8].localPosition.z));//new Vector3(12.0f, 12.0f, 0.0f));
        attack2Right.Add(windAttackTransforms[3].localPosition = new Vector3(-windAttackTransforms[9].localPosition.x,
                                                                              windAttackTransforms[9].localPosition.y,
                                                                              windAttackTransforms[9].localPosition.z));//new Vector3(10.0f, 6.5f, 0.0f));
        attack2Right.Add(windAttackTransforms[4].localPosition = new Vector3(-windAttackTransforms[10].localPosition.x,
                                                                              windAttackTransforms[10].localPosition.y,
                                                                              windAttackTransforms[10].localPosition.z));//new Vector3(9.5f, 2.5f, 0.0f));
        attack2Right.Add(windAttackTransforms[5].localPosition = new Vector3(-windAttackTransforms[11].localPosition.x,
                                                                              windAttackTransforms[11].localPosition.y,
                                                                              windAttackTransforms[11].localPosition.z));//new Vector3(3.0f, 0.4f, 0.0f));

        // Create the attack 3 list...
        attack3Left.Add(windAttackTransforms[12].localPosition);//new Vector3(-17.0f, 2.0f, 0.0f));
        attack3Left.Add(windAttackTransforms[13].localPosition);//new Vector3(-15.5f, 5.5f, 0.0f));
        attack3Left.Add(windAttackTransforms[14].localPosition);//new Vector3(-12.0f, 0.0f, 0.0f));
        attack3Left.Add(windAttackTransforms[15].localPosition);//new Vector3(-10.0f, 2.5f, 0.0f));
        attack3Left.Add(windAttackTransforms[16].localPosition);//new Vector3(-4.5f, 2.4f, 0.0f));
        attack3Left.Add(windAttackTransforms[17].localPosition);//new Vector3(-3.5f, 4.5f, 0.0f));

        attack3Right.Add(windAttackTransforms[0].localPosition = new Vector3(-windAttackTransforms[12].localPosition.x,
                                                                              windAttackTransforms[12].localPosition.y,
                                                                              windAttackTransforms[12].localPosition.z));//new Vector3(17.0f, 2.0f, 0.0f));
        attack3Right.Add(windAttackTransforms[1].localPosition = new Vector3(-windAttackTransforms[13].localPosition.x,
                                                                              windAttackTransforms[13].localPosition.y,
                                                                              windAttackTransforms[13].localPosition.z));//new Vector3(15.5f, 5.5f, 0.0f));
        attack3Right.Add(windAttackTransforms[2].localPosition = new Vector3(-windAttackTransforms[14].localPosition.x,
                                                                              windAttackTransforms[14].localPosition.y,
                                                                              windAttackTransforms[14].localPosition.z));//new Vector3(12.0f, 0.0f, 0.0f));
        attack3Right.Add(windAttackTransforms[3].localPosition = new Vector3(-windAttackTransforms[15].localPosition.x,
                                                                              windAttackTransforms[15].localPosition.y,
                                                                              windAttackTransforms[15].localPosition.z));//new Vector3(10.0f, 2.5f, 0.0f));
        attack3Right.Add(windAttackTransforms[4].localPosition = new Vector3(-windAttackTransforms[16].localPosition.x,
                                                                              windAttackTransforms[16].localPosition.y,
                                                                              windAttackTransforms[16].localPosition.z));//new Vector3(4.5f, 2.4f, 0.0f));
        attack3Right.Add(windAttackTransforms[5].localPosition = new Vector3(-windAttackTransforms[17].localPosition.x,
                                                                              windAttackTransforms[17].localPosition.y,
                                                                              windAttackTransforms[17].localPosition.z));//new Vector3(3.5f, 4.5f, 0.0f));
    }

	// 
	protected void CreateWindShot(Vector3 pos)
	{
		Transform windTransform = (Transform) Instantiate(weaponPrefab, projectileSpawnPoint.position, transform.rotation);
		windTransform.SendMessage("SetPosition", projectileSpawnPoint.position + pos);
		windTransform.transform.parent = gameObject.transform;

		AirmanWind wind = windTransform.GetComponent<AirmanWind>();
		if (wind)
		{
			windShots.Add(wind);
		}
	}
	
	// 
	protected void Blow()
	{
		if (shouldDestroyWind == true)
		{
			if (Time.time - fightingTimer >= windDestroyDelay)
			{
				// Destroy the wind gameobjects and clear the list
				for(int i = 0; i < windShots.Count; i++)
				{
					Destroy(windShots[i].gameObject);
				}
				windShots.Clear();
				
				player.IsExternalForceActive = false;
				isBlowing = false;
				ShouldBlow = false;
				isShooting = true;
				shouldDestroyWind = false;
				fightingTimer = Time.time;	
			}
			else
			{
				if (IsTurningLeft == true)
				{
					player.ExternalForce = -Vector3.right * windPower;
				}
				else if (IsTurningLeft == false)
				{
					player.ExternalForce = Vector3.right * windPower;
				}
			}
		}
		else
		{
			if (Time.time - fightingTimer >= blowDelay)
			{
				// Make the wind go away!
				for(int i = 0; i < windShots.Count; i++)
				{
					windShots[i].GoAway();
				}
				
				player.IsExternalForceActive = true;
				ShouldShoot = false;
				ShouldBlow = true;
				shouldDestroyWind = true;
				fightingTimer = Time.time;
			}
		}
	}
	
	// 
	protected void Shoot()
	{
		// If the boss has shot three times...
		if (shootingCounter == NUM_OF_SHOTS_BEFORE_JUMPING)
		{
			// Make him jump to the other side and attack again...
			isShooting = false;
			isJumping = true;
			shootingCounter = 0;
		}
		else if(GetComponent<CharacterController2D>().isGrounded)
		{
            var flipx = GetComponent<SpriteRenderer>().flipX;

            if(!flipx)
                projectileSpawnPoint.localPosition = new Vector3(projectileSpawnPoint.localPosition.x < 0 ? projectileSpawnPoint.localPosition.x : -projectileSpawnPoint.localPosition.x
                                                           , projectileSpawnPoint.localPosition.y, projectileSpawnPoint.localPosition.z);
            else
                projectileSpawnPoint.localPosition = new Vector3(projectileSpawnPoint.localPosition.x > 0 ? projectileSpawnPoint.localPosition.x : -projectileSpawnPoint.localPosition.x
                    , projectileSpawnPoint.localPosition.y, projectileSpawnPoint.localPosition.z);

            // Create the wind...
            foreach (Vector3 wind in nextAttack) 
			{
				CreateWindShot(wind);
			}
			
			// Set up the next attack
			if (IsTurningLeft == true)
            {

                if (shootingCounter == 0) nextAttack = attack2Left;
				else if (shootingCounter == 1) nextAttack = attack3Left;
				else if (shootingCounter == 2) nextAttack = attack1Right;
			}
			else if (IsTurningLeft == false)
			{
                if (shootingCounter == 0) nextAttack = attack2Right;
				else if (shootingCounter == 1) nextAttack = attack3Right;
				else if (shootingCounter == 2) nextAttack = attack1Left;
			}
			
			// Set the appropriate variables
			ShouldShoot = true;
			isBlowing = true;
			isShooting = false;
			fightingTimer = Time.time;
			shootingCounter++;
		}
	}

    // 
    protected void Jump()
    {
        if (isPlayingAnimation == false)
        {
            anim.SetBool("Shoot", false);
            anim.SetBool("Stand", false);
            anim.SetBool("Blow", false);
            //anim.SetBool("Flex", true);

            ShouldJump = true;
            GetComponent<AirmanBoss>().IsJumping = true;
            isPlayingAnimation = true;
        }

        // If we're done playing the animation...    
        if (!GetComponent<AirmanBoss>().IsJumping)
        {
            //anim.applyRootMotion = true;
            isPlayingAnimation = false;
			ShouldJump = false;
			IsTurningLeft = !IsTurningLeft;
			isShooting = true;
			isJumping = false;
		}
        else
        {
            
        }
	}

	#endregion


	#region Public Functions

	// 
	public void Attack()
	{
		isFighting = true;
		isShooting = true;
	}

	public void Reset()
	{
		anim.StopPlayback();
		shootingCounter = 0;
		nextAttack = attack1Left;
		IsTurningLeft = true;
		isFighting = false;
		isShooting = false;
		isBlowing = false;
		isJumping = false;
		isPlayingAnimation = false;
		shouldDestroyWind = false;
		ShouldShoot = false;
		ShouldJump = false;
		ShouldBlow = false;
		
		player.IsExternalForceActive = false;
		for(int i = 0; i < windShots.Count; i++)
		{
			Destroy(windShots[i].gameObject);
		}
		windShots.Clear();
	}

	#endregion
}

