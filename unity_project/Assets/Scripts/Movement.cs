using UnityEngine;
using System.Collections;
using Prime31;

[RequireComponent (typeof (CharacterController2D))]
public class Movement : MonoBehaviour
{
	#region Variables

	// Properties
	public bool IsTurningLeft			{ get; protected set; }
	public bool IsJumping 				{ get; protected set; }
	public bool IsWalking				{ get; protected set; }
	public bool IsHurting				{ get; set; }
	public bool IsFrozen 				{ get; set; }
	public bool IsExternalForceActive 	{ get; set; }
	public Vector3 ExternalForce 		{ get; set; }
	public Vector3 CheckPointPosition 	{ get; set; }
	
	// Protected Instance Variables
	protected CharacterController2D charController;
    protected bool isFalling = false;
	protected bool cheating = false;
    [SerializeField]
    protected float gravity = 75f; //40f;	 			// Downward force
    [SerializeField]
	protected float terminalVelocity = 30f;	// Max downward speed
    [SerializeField]
    protected float jumpSpeed = 25f; //20f;			// Upward speed
    [SerializeField]
    protected float moveSpeed = 8f; //10f;			// Left/Right speed    
	protected float verticalVelocity;
	protected float hurtingForce = 1.0f;
    [SerializeField]
	protected Vector3 moveVector = Vector3.zero;
    [SerializeField]
    protected Vector3 startPosition = new Vector3(13.34303f, 11.51588f, 0f);

    protected Vector2 lastInput = Vector2.zero;
    [SerializeField]
    protected bool lastInputJump = false;

	#endregion
	
	
	#region MonoBehaviour

	// Use this for initialization
	protected void Awake()
	{
		charController = gameObject.GetComponent<CharacterController2D>();
	}
	
	// Use this for initialization
	protected void Start ()
	{
		IsHurting = false;
		transform.position = CheckPointPosition = startPosition;
	}

    #endregion


    #region Protected Functions
    private bool wasJumping = false;
	//
	protected void ApplyGravity()
	{
        // cap downward speed
		if (moveVector.y > -terminalVelocity)
		{
			moveVector = new Vector3(moveVector.x, (moveVector.y - gravity * Time.deltaTime), moveVector.z);
        }
		
        // player has landed on the ground
		if (charController.isGrounded && moveVector.y < -1)
		{
			IsJumping = false;
			moveVector = new Vector3(moveVector.x, (-1), moveVector.z);

            // play landing sound
            if(wasJumping == true)
            {
                GameEngine.SoundManager.Play(AirmanLevelSounds.LANDING);
            }

            wasJumping = false;
		}

        // determine if the player is falling
        if( moveVector.y < 0)
        {
            isFalling = true;
        }
        else
        {
            isFalling = false;
        }
	}
	
	//
	protected void ProcessExternalForces()
	{
		// 
		if (IsExternalForceActive == true)
		{
			moveVector += ExternalForce * Time.deltaTime;
		}
	}
	
	//
	protected void ProcessMovement()
	{
		//transform moveVector into world-space relative to character rotation
		moveVector = transform.TransformDirection(moveVector);
		
		//normalize moveVector if magnitude > 1
		if (moveVector.magnitude > 1)
		{
			moveVector = Vector3.Normalize(moveVector);
		}
		
		//multiply moveVector by moveSpeed
		moveVector *= moveSpeed;
		
		//
		ProcessExternalForces();
		
		// 
		if (IsHurting == true)
		{
			moveVector += (((IsTurningLeft == true) ? Vector3.right : -Vector3.right) * hurtingForce);
		}
		
		//reapply vertical velocity to moveVector.y
		moveVector = new Vector3(moveVector.x, verticalVelocity, 0.0f);
		
		//apply gravity
		ApplyGravity();
		
		//move character in world-space
		charController.move(moveVector * Time.deltaTime);        
	}

	//
	protected void CheckMovement()
	{
        // hold on to horizontal and vertical movement
        Vector2 currentInput = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        // Check Up through jump button or "up" on the keyboard.
        bool isJumpingButtonPressed = Input.GetButton("Jump") | currentInput.y > 0f;
		// Horizontal movement...
		float deadZone = 0.01f;
		verticalVelocity = moveVector.y;
		moveVector = Vector3.zero;

        if (!IsHurting)
        {

            if (currentInput.x > deadZone)
            {
                IsWalking = true;
                IsTurningLeft = false;
                moveVector += new Vector3(currentInput.x, 0, 0);
            }
            else if (currentInput.x < -deadZone)
            {
                IsWalking = true;
                IsTurningLeft = true;
                moveVector += new Vector3(currentInput.x, 0, 0);
            }
            else
            {
                IsWalking = false;
            }

            // Vertical movement...
            if (isJumpingButtonPressed && lastInputJump == false)
            {
                if (charController.isGrounded)
                {
                    IsJumping = true;
                    wasJumping = true;
                    verticalVelocity = jumpSpeed;
                }
            }

            if ( !isJumpingButtonPressed && lastInputJump == true && isFalling == false )
            {
                verticalVelocity = 0.0f;
            }

            //If there is a collision above...
            if (charController.collisionState.above)
            {
                verticalVelocity = -5.0f;
            }
        }
        lastInput = currentInput;
	    if (charController.isGrounded)
	        lastInputJump = false;
        else
            lastInputJump = isJumpingButtonPressed;

    }

	#endregion
	
	
	#region Public Functions
	
	//
	public void Reset()
	{
		IsFrozen = false;
		IsHurting = false;
		transform.position = CheckPointPosition;
	}

	//
	public void HandleMovement()
	{
		if (IsFrozen == true)
		{
			moveVector = Vector3.zero;
			ProcessExternalForces();
			charController.move(moveVector * Time.deltaTime);
		}
		else
		{
			CheckMovement();
			ProcessMovement();
		}
	}

	#endregion
}

