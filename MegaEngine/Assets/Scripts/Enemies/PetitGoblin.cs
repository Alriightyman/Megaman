using UnityEngine;
using UnityEngine.Assertions;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Interfaces;
using System;
using Prime31;

public sealed class PetitGoblin : MonoBehaviour//, IResetable
{
	#region Variables
    [SerializeField]
    private float robotSpeed = 35;
    private float distanceToDisappear = 32.0f;
    // Protected Instance Variables
    private bool shouldAttack = false;
    private int damage = 10;
    private int health = 10;
    private float attackDelay = 0.7f;
    private float attackDelayTimer;
    private SpriteRenderer renderer2D = null;
    private Rigidbody2D rigidBody = null;
    private bool onRightSide = false;
    private CharacterController2D controller;


    private float rightXPos;
    private float leftXPos;
    private float yPos;
    //private float xAmount = 2f;
    //[SerializeField]
    //private float yAmount = 2.5f;

    public float RightXPos { get { return rightXPos; } set { rightXPos = value; } }
    public float LefttXPos { get { return leftXPos; } set { leftXPos = value; } }
    public float YPos { get { return yPos; } set { yPos = value; } }

    #endregion


    #region MonoBehaviour

    // Constructor
    private void Awake()
	{
		rigidBody = GetComponent<Rigidbody2D>();
		Assert.IsNotNull(rigidBody);

		renderer2D = GetComponent<SpriteRenderer>();
		Assert.IsNotNull(renderer2D);

        controller = GetComponent<CharacterController2D>();

        //sGameEngine.GetResetableObjectList().Add(this);
    }

    // Use this for initialization 
    private void Start ()
	{
		attackDelayTimer = Time.time;
        if (rigidBody.velocity.x > 0f)
        {
            onRightSide = true;
        }
        rigidBody.velocity = Vector2.zero;
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

            // Kill this object if it is off screen
            //if (direction.magnitude >= distanceToDisappear)
            if(!renderer2D.isVisible)
            {
                KillRobot();
            }
            else
            {
                //rigidBody.velocity = ;
                controller.move(direction.normalized * robotSpeed * Time.deltaTime);
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
        //Debug.Log(String.Format("Rightside: {0}; currentPos: {1}; desiredPosition: {2}", onRightSide, gameObject.transform.localPosition, rightXPos));
        if((onRightSide && gameObject.transform.position.x <= rightXPos) ||
          (!onRightSide && gameObject.transform.position.x >= leftXPos))
        {
            //rigidBody.velocity = new Vector2(direction * robotSpeed * Time.deltaTime, 0f) ;
            controller.move(new Vector2(direction * robotSpeed * Time.deltaTime, 0f));
            return false;
        }

        return true;
    }

    private bool MoveVeritcalToPosition()
    {
        //rigidBody.velocity = new Vector2(0f, robotSpeed * Time.deltaTime );
        controller.move(new Vector2(0f, robotSpeed * Time.deltaTime ));
        if (gameObject.transform.position.y > yPos)
        {
            return true;
        }
        return false;
    }


    private void KillRobot()
    {
        if (this != null)
        {
            transform.parent.gameObject.GetComponent<Goblin>().DecrementRobotCount();
            Destroy(gameObject);
        }
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
    #endregion


    #region Public Functions

    // 
    public void ResetObject()
	{
		KillRobot();	
	}

	#endregion
}

