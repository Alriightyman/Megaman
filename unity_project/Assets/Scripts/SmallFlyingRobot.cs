using UnityEngine;
using UnityEngine.Assertions;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Interfaces;

public sealed class SmallFlyingRobot : MonoBehaviour, IResetable
{
	#region Variables

	// Protected Instance Variables
	private bool shouldAttack = false;
    private int damage = 10;
    private int health = 10;
    private float robotSpeed = 35;
    private float attackDelay = 0.7f;
    private float attackDelayTimer;
    private float distanceToDisappear = 32.0f;
    private Renderer renderer2D = null;
    private Rigidbody2D rigidBody = null;
    private bool onRightSide = false;
    [SerializeField]
    private float xAmount = 2f;
    [SerializeField]
    private float yAmount = 2.5f;
    #endregion


    #region MonoBehaviour

    // Constructor
    private void Awake()
	{
		rigidBody = GetComponent<Rigidbody2D>();
		Assert.IsNotNull(rigidBody);

		renderer2D = GetComponent<Renderer>();
		Assert.IsNotNull(renderer2D);

        GameEngine.GetResetableObjectList().Add(this);
	}

    // Use this for initialization 
    private void Start ()
	{
		attackDelayTimer = Time.time;
        if (rigidBody.velocity.x > 0f)
        {
            onRightSide = true;
        }
	}

    // Update is called once per frame 
    private void Update()
	{
        MoveIntoPosition();

        MoveTowardsPlayer();
		
	}

    //
    private void OnTriggerStay2D(Collider2D other) 
	{
		if (other.tag == "Player")
		{
			GameEngine.Player.TakeDamage(damage);
		}
	}

    //
    private void OnCollisionStay2D(Collision2D collision) 
	{
		if (collision.gameObject.tag == "Player")
		{
			GameEngine.Player.TakeDamage(damage);
		}
	}

	#endregion


	#region Private Functions
	
	//
	private void KillRobot()
	{
		transform.parent.gameObject.GetComponent<RedHornBeast>().MinusRobotCount();
		Destroy(gameObject);
	}

	//
	private void TakeDamage(int damageTaken)
	{
		GameEngine.SoundManager.Play(AirmanLevelSounds.BOSS_HURTING);
		health -= damageTaken;
		if (health <= 0)
		{
			KillRobot();
		}
	}

    private void MoveIntoPosition()
    {
        if (shouldAttack == false)
        {
            bool inPosition = false;

            bool xPosSet = MoveHorizontalToPosition();

            if (xPosSet)
            {
                inPosition = MoveVeritcalToPosition();
                if (inPosition)
                {
                    shouldAttack = true;
                }
            }
        }
    }

    private void MoveTowardsPlayer()
    {
        if (shouldAttack == true)
        {
            Vector3 direction = GameEngine.Player.transform.position - transform.position;

            // Kill this object if the player is too far away
            if (direction.magnitude >= distanceToDisappear)
            {
                KillRobot();
            }
            else
            {
                rigidBody.velocity = direction.normalized * (Time.deltaTime * robotSpeed);
            }
        }
    }

    private bool MoveHorizontalToPosition()
    {
        float direction = 1;

        if (!onRightSide)
        {
            direction = -1;
        }

        if ((onRightSide && gameObject.transform.localPosition.x <= xAmount * direction) ||
             (!onRightSide && gameObject.transform.localPosition.x >= xAmount * direction))
        {
            rigidBody.velocity = new Vector2(direction * (Time.deltaTime * robotSpeed * 1f), 0f);
            return false;
        }

        return true;
    }

    private bool MoveVeritcalToPosition()
    {
        rigidBody.velocity = new Vector2(0f, (Time.deltaTime * robotSpeed * 8f));
        if (gameObject.transform.localPosition.y > yAmount)
        {
            return true;
        }
        return false;
    }

    #endregion


    #region Public Functions

    // 
    public void Reset()
	{
		KillRobot();	
	}

	#endregion
}

