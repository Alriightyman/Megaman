using UnityEngine;
using UnityEngine.Assertions;
using System.Collections;
using System.Collections.Generic;

public class Matasaburo : MonoBehaviour 
{
    #region Variables

    // Unity Editor Variables
    public GameObject powerup;
	[SerializeField] private float windRange = 12.0f;
	[SerializeField] private float windPower = 250.0f;

	// private Instance Variables
	private bool armsUp = false;
	private bool weaponActivated = false;
	private bool isTurningLeft = true;
	private bool isDead = false;
	private int damage = 6;
	private int health = 30;
	private int texIndex;
	private int currentHealth;

	private float distanceToPlayer;
	private float texChangeTimer;
	private float texArmsUpInterval = 0.1f;
	private float texArmsDownInterval = 0.1f;
	private Vector2 texScale = new Vector2(-1.0f, -1.0f);
	private Vector2 texScaleRight = new Vector2(1.0f, -1.0f);
	private Vector2 texScaleLeft = new Vector2(-1.0f, -1.0f);
	private Vector3 windDirection = new Vector3(-1.0f, 0f, 0f);
    private Animator anim = null;
	#endregion


	#region MonoBehaviour

	// Constructor
	private void Awake() 
	{
        //Assert.IsTrue(animationMaterials.Count == 4);
        anim = GetComponent<Animator>();
	}

	// Use this for initialization
	private void Start() 
	{
		texChangeTimer = Time.time;
		currentHealth = health;
	}

	// Update is called once per frame
	private void Update() 
	{
		if (isDead == false)
		{
			distanceToPlayer = GameEngine.Player.transform.position.x - transform.position.x;
			
			// Make sure the robot is facing the player...
			if (isTurningLeft == false && distanceToPlayer <= 0.0f)
			{
				MakeRobotTurnLeft();
			}
			else if (isTurningLeft == true && distanceToPlayer > 0.0f)
			{
				MakeRobotTurnRight();
			}
			
			// Turn on / off the weapon if the player is in range...
			if (Mathf.Abs(distanceToPlayer) <= windRange)
			{
				if (weaponActivated == false)
				{
					TurnWindWeaponOn();
				}
			}
			else if (weaponActivated == true)
			{
				TurnWindWeaponOff();
			}
			
			// Put a texture on the robot...
			AssignTexture();
		}
	}
    private void OnDestroy()
    {
        Instantiate(powerup, transform);
    }

    #endregion


    #region private Functions

    //
    private void KillRobot()
	{
		isDead = true;
		GetComponent<Renderer>().enabled = false;
		GetComponent<Collider2D>().enabled = false;
	}
	
	//
	private void OnTriggerStay2D(Collider2D other) 
	{
		if (other.tag == "Player")
		{
			other.gameObject.SendMessage("TakeDamage", damage);
		}
	}
	
	// Make the robot take damage
	private void TakeDamage(int damageTaken)
	{
		GameEngine.SoundManager.Play(AirmanLevelSounds.BOSS_HURTING);
		currentHealth -= damageTaken;
		
		if (currentHealth <= 0)
		{
			TurnWindWeaponOff();
			KillRobot();
		}
	}
	
	//
	private void SendWindInfoToPlayer()
	{
		GameEngine.Player.ExternalForce = windDirection * windPower;
	}
	
	// Turn on the wind weapon
	private void TurnWindWeaponOn()
	{
		weaponActivated = true;
		GameEngine.Player.IsExternalForceActive = true;
		SendWindInfoToPlayer();
	}
	
	// Turn off the wind weapon
	private void TurnWindWeaponOff()
	{
		GameEngine.Player.IsExternalForceActive = false;
		weaponActivated = false;
	}
	
	//
	private void MakeRobotTurnLeft()
	{
		isTurningLeft = true;
		texScale = texScaleLeft;
		windDirection *= -1;
		SendWindInfoToPlayer();
	}
	
	//
	private void MakeRobotTurnRight()
	{
		isTurningLeft = false;
		texScale = texScaleRight;
		windDirection *= -1;
		SendWindInfoToPlayer();
	}

	//  Three textures are used to simulate animation on the robot TODO
	private void AssignTexture()
	{
        anim.Play("Idle");
		//if (armsUp == true)
		//{
		//	texIndex = (int) (Time.time / texArmsUpInterval);
		//	GetComponent<Renderer>().material = animationMaterials[(texIndex % 2) + 2 ];
			
		//	if (Time.time - texChangeTimer >= 0.35f)
		//	{
		//		texChangeTimer = Time.time;
		//		armsUp = !armsUp;
		//	}
		//}
		//else
		//{
		//	texIndex = (int) (Time.time / texArmsDownInterval);
		//	GetComponent<Renderer>().material = animationMaterials[(texIndex % 2) ];
			
		//	if (Time.time - texChangeTimer >= 1.99f)
		//	{
		//		texChangeTimer = Time.time;
		//		armsUp = !armsUp;
		//	}
		//}
		
		//GetComponent<Renderer>().material.SetTextureScale("_MainTex", texScale);
	}

	#endregion


	#region Public Functions

	// 
	public void Reset()
	{
		isDead = false;
		GetComponent<Renderer>().enabled = true;
		GetComponent<Collider2D>().enabled = true;
		currentHealth = health;
	}

    #endregion
	
    #region Gizmos
	private void OnDrawGizmosSelected()
	{
		var renderer = gameObject.GetComponent<SpriteRenderer>();
        var center = renderer.sprite.rect.center.y /renderer.sprite.rect.width;
		var direction = renderer.flipX ? 1 : -1;
		var airFlowLine = new Vector3(transform.position.x + windRange * direction, 
                                        transform.position.y - center, 
                                        transform.position.z);
		var centerPosition = new Vector3(transform.position.x, transform.position.y - center, transform.position.z);
        Gizmos.color = Color.red;
		Gizmos.DrawLine(centerPosition, airFlowLine);
	}

    #endregion
}
