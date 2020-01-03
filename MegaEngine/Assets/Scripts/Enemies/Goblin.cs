using UnityEngine;
using System.Collections;
using System;
using Assets.Scripts.Interfaces;

public class Goblin : MonoBehaviour//, IResetable
{
	#region Variables
	
	// Unity Editor Variables
	public Rigidbody2D flyingRobot;
    [SerializeField] private Transform spawnPointLeft;
    [SerializeField] private Transform spawnPointRight;
    [SerializeField] private Transform leftYPos;
    [SerializeField] private Transform rightYPos;
    [SerializeField] private float spikeRisingSpeed = 0.075f;              //
    [SerializeField] private float spikeLoweringSpeed = 1.00f;                //
    [SerializeField] private float maxSpikeHeight = 0.6f;
    [SerializeField] private float distanceToDisappear = 32.0f;			//

    // should only be 3 bots MAX, so this value needs to be static
    private static int robotCount = 0;                          
                                                                
    // private Instance Variables
    private bool shouldAppear = false;					//
	private bool startFighting = false;					//
	private bool spikeRising = true;						//
	private bool spikeLowering = false;					//
	private bool createRobotOnRightSide = true;			//
	private float lightStartTime;							//
	private float spikeStartHeight;						//
	private float spikeWaitTimer = 0.0f;					// Timer used for timing the spike while they wait
	private float spikeDelayTime = 2.0f; 					// How long should the spike wait at the top?
    [SerializeField]
	private float robotCreateDelay = 1.0f; 				// Used so that a small delay is between creating robots
	private float robotCreateDelayTimer; 					// Used so that a small delay is between creating robots
	private Vector3 spikeTransforms;						// Used for transforming the spike when fighting
	private Vector3 spikeLeftPos;							//
	private Vector4 color = new Vector4(0f, 0f, 0f, 0f);	//
	private Transform lightTransform;						//
	private GameObject spikeLeft;							//
	private GameObject spikeRight;						//

    bool canMakeRobots = false;
    #endregion


    #region MonoBehaviour

    // The Constructor function in Unity...
    /// <summary>
    /// sets the light transform and the spikes
    /// </summary>
    private void Awake () 
	{
		lightTransform = transform.Find("Light").transform;
		spikeLeft = transform.Find("SpikeLeft").gameObject;
		spikeRight = transform.Find("SpikeRight").gameObject;

        GameEngine.AddResetCallback(new Action(ResetObject));
    }
	
	// Use this for initialization
    /// <summary>
    /// Disable light and make goblin and spikes invisible
    /// </summary>
	private void Start ()
	{
		lightTransform.GetComponent<SpriteRenderer>().enabled = false;
        GetComponent<BoxCollider2D>().enabled = false;
        spikeLeft.GetComponent<BoxCollider2D>().enabled = false;
        spikeRight.GetComponent<BoxCollider2D>().enabled = false;
        spikeLeftPos = spikeLeft.transform.localPosition;
		spikeStartHeight = spikeLeft.transform.position.y;
		
		// Make the robot and its children invisible...
		GetComponent<SpriteRenderer>().color = color;
		spikeLeft.GetComponent<SpriteRenderer>().color = color;
		spikeRight.GetComponent<SpriteRenderer>().color = color;
	}
	
	// Update is called once per frame
	private void Update ()
	{
		if ( startFighting == true ) 
		{
            // update spikes, make light blink, and create
			MoveSpikes();
			MakeLightBlink();
			CreatePetitGoblinRobots();
			
			// Stop fighting if the player is too far away
			if ( (GameEngine.Player.transform.position - transform.position).magnitude >= distanceToDisappear )
			{
				ResetGoblin();
			}
		}
		else if ( shouldAppear == true )
		{
			if ( color.x >= 1.0 )
			{
				shouldAppear = false;
				startFighting = true;
                GetComponent<BoxCollider2D>().enabled = true;
                spikeLeft.GetComponent<BoxCollider2D>().enabled = true;
                spikeRight.GetComponent<BoxCollider2D>().enabled = true;
            }
			// Make the robot and its children visible...
			else 
			{
				color.x = color.y = color.z = color.w += Time.deltaTime * 3.5f;
			}

            GetComponent<SpriteRenderer>().color = color;
            spikeLeft.GetComponent<SpriteRenderer>().color = color;
			spikeRight.GetComponent<SpriteRenderer>().color = color;
		}
	}

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // if the player is colliding, allow Goblin to make robots
        if(collision.gameObject.tag == "Player")
        {
            canMakeRobots = true;
        }
        // if a player shot is colliding, ricochet it at a 45 degree angle.
        else if(collision.gameObject.tag == "shot")
        {
            var boxcollider = collision.gameObject.GetComponent<BoxCollider2D>();
            if (boxcollider != null)
            {
                boxcollider.enabled = false;
            }
            var shot = collision.gameObject.GetComponent<Shot>();
            var velocity = shot.VelocityDirection;
            shot.VelocityDirection = new Vector3(-velocity.x, Math.Abs(velocity.x), velocity.z);
            GameEngine.SoundManager.Play(AirmanLevelSounds.LANDING);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        // when the player leaves the goblin, stop making robots
        if (collision.gameObject.tag == "Player")
        {
            canMakeRobots = false;
        }
    }
    #endregion


    #region private Functions

    /// <summary>
    /// Creates a new petit goblin. Sets its position, speed, velocity
    /// </summary>
    /// <param name="speed">new petit goblin's speed</param>
    /// <param name="pos">new petit goblin's position</param>
    /// <param name="vel">new petit goblin's velocity</param>
    /// <returns>PetitGoblin </returns>
    private PetitGoblin CreateRobot( float speed, Vector3 pos, Vector3 vel)
	{
        Rigidbody2D robot = Instantiate(flyingRobot, transform.position, transform.rotation);
        Physics2D.IgnoreCollision(robot.GetComponent<Collider2D>(), GetComponent<Collider2D>());
        robot.gameObject.transform.position = pos;
        robot.transform.parent = gameObject.transform;
        robot.velocity = vel;

        return robot.gameObject.GetComponent<PetitGoblin>();
	}
	
	/// <summary>
    /// Creates a new petit goblin after a small delay
    /// Can only have 3 petit goblins at a time between all Goblins
    /// </summary>
	private void CreatePetitGoblinRobots()
	{
        // have 3 robots max :)
		if (canMakeRobots && robotCount < 3 && Time.time - robotCreateDelayTimer >= robotCreateDelay)
		{
            // get the gameobject transform that is a child of this gameobject
            if ( createRobotOnRightSide )
			{                
				Vector3 pos = spawnPointRight.position;
				PetitGoblin bot = CreateRobot( 0.0f, pos, Vector3.right );
                bot.RightXPos = rightYPos.position.x;
                bot.YPos = leftYPos.position.y;
			}
			else 
			{
				Vector3 pos = spawnPointLeft.position;
                PetitGoblin bot = CreateRobot( 0.0f, pos, -Vector3.right );
                bot.LefttXPos = leftYPos.position.x;
                bot.YPos = leftYPos.position.y;
            }

            robotCreateDelayTimer = Time.time;
            createRobotOnRightSide = !createRobotOnRightSide;
			robotCount++;
		}
	}
	
	/// <summary>
    /// Lowers and raises the spikes on the Goblin
    /// </summary>
	private void MoveSpikes()
	{
		if ( spikeRising )
		{
			spikeTransforms = new Vector3(0f, spikeLeftPos.y * Time.deltaTime * -spikeRisingSpeed, 0f);
			spikeLeft.transform.localPosition += spikeTransforms;
			spikeRight.transform.localPosition += spikeTransforms;
			
			if ( spikeLeft.transform.localPosition.y  >= maxSpikeHeight)
			{
				spikeRising = false;
				spikeLowering = false;
			}
		}
		// 
		else if ( spikeLowering )
		{
			spikeTransforms = new Vector3(0f, spikeLeftPos.y * Time.deltaTime * -spikeLoweringSpeed, 0f);
			spikeLeft.transform.localPosition -= spikeTransforms;
			spikeRight.transform.localPosition -= spikeTransforms;
			
			if ( spikeLeft.transform.localPosition.y - spikeLeftPos.y <= 0.00f )
			{
				spikeRising = true;
			}
		}
		else 
		{
			spikeWaitTimer += Time.deltaTime;
			if ( spikeWaitTimer > spikeDelayTime )
			{
				spikeLowering = true;
				spikeWaitTimer = 0.0f;
			}
		}
	}
	
	/// <summary>
    /// Makes the light rapidly blink
    /// </summary>
	private void MakeLightBlink()
	{
		if ( Time.time - lightStartTime >= 0.1f )
		{
			lightStartTime = Time.time;
			lightTransform.GetComponent<SpriteRenderer>().enabled = !lightTransform.GetComponent<SpriteRenderer>().enabled;
		}
	}

    #endregion


    #region Public Functions

    /// <summary>
    /// Reset Object
    /// </summary>
    public void ResetObject()
    {
        ResetGoblin();
        KillRobotChildren();
    }

    /// <summary>
    /// Set Goblin to appear
    /// </summary>
    public void Appear()
    {
        shouldAppear = true;
        lightStartTime = Time.time;
    }

    /// <summary>
    /// Decrement Robot Count and create a new robot
    /// </summary>
    public void DecrementRobotCount()
    {
        robotCount--;
        CreatePetitGoblinRobots();
    }

    #endregion


    private void ResetGoblin()
	{
		shouldAppear = false;
		startFighting = false;
		color = new Vector4(0f, 0f, 0f, 0f);
		GetComponent<SpriteRenderer>().color = color;
		
		Vector3 spikePos;
		spikePos = spikeLeft.transform.position;
		spikePos.y = spikeStartHeight;
		spikeLeft.transform.position = spikePos;
		spikePos.x = spikeRight.transform.position.x;
		spikeRight.transform.position = spikePos;
		
		spikeLeft.GetComponent<SpriteRenderer>().color = color;
		spikeRight.GetComponent<SpriteRenderer>().color = color;
		lightTransform.GetComponent<SpriteRenderer>().enabled = false;
		createRobotOnRightSide = true;
        this.GetComponent<BoxCollider2D>().enabled = false;
        spikeLeft.GetComponent<BoxCollider2D>().enabled = false;
        spikeRight.GetComponent<BoxCollider2D>().enabled = false;
    }
	
	// 
	private void KillRobotChildren()
	{
		// Reset all the enemy bots...
		Transform robot = transform.Find("PetitGoblin(Clone)");
		if ( robot != null)
		{
			Destroy(robot.gameObject);
		}
		robotCount = 0;	
	}
}

