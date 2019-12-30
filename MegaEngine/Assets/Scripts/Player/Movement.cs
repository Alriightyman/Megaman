using UnityEngine;
using System.Collections;
using Prime31;
using System;

[RequireComponent (typeof (CharacterController2D))]
public class Movement : MonoBehaviour
{
    #region Variables
	// Properties
	public bool IsTurningLeft { get; protected set; }
	public bool IsJumping { get; protected set; }
	public bool IsWalking { get; protected set; }
	public bool IsHurting { get; set; }
	public bool IsFrozen  { get; set; }
	public bool IsExternalForceActive { get; set; }
	public Vector3 ExternalForce { get; set; }
	public Vector3 CheckPointPosition { get; set; }
    public bool IsEnteringLevel { get; set; }
    public bool IsFalling { get; set; } = false;

    // Downward force, originally 40f		
    [SerializeField] protected float gravity = 75f;
    // Max downward speed
    [SerializeField] protected float terminalVelocity = 30f;
    // Upward speed, origianlly 20f			
    [SerializeField] protected float jumpSpeed = 25f;
    // Left/Right speed, originally 10f
    [SerializeField] protected float moveSpeed = 8f;         		
    [SerializeField] private float landingSpeed = 1f;
    [SerializeField] protected float hurtingForce = 1.0f;
    [SerializeField] protected Vector3 moveVector = Vector3.zero;
    [SerializeField] protected Vector3 startPositionLocation;
    /// <summary>
    /// Used for debugging purposes
    /// </summary>
    [SerializeField] protected GameObject DebugStartPosition;


    // Protected Instance Variables
    protected Vector2 lastInput = Vector2.zero;
    protected bool lastInputJump = false;
    protected float verticalVelocity;
    public bool isGrounded;
    protected CharacterController2D charController;
    
    protected bool cheating = false;
    private bool wasJumping = false;
 
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
        float offset = 128f;

        startPositionLocation = DebugStartPosition != null ? DebugStartPosition.transform.position : CheckPointPosition;
        CheckPointPosition = startPositionLocation;
        IsHurting = false;
        //transform.position = CheckPointPosition = startPositionLocation;
        transform.position = new Vector2(startPositionLocation.x, startPositionLocation.y + offset);
	}
    #endregion


    #region Protected Functions
	//
	protected void ApplyGravity()
	{
        // drop faster if entering level from above
        var finalGravity = IsEnteringLevel ? gravity * landingSpeed : gravity;

        // cap downward speed
		if (moveVector.y > -terminalVelocity)
		{
			moveVector = new Vector3(moveVector.x, moveVector.y - finalGravity * Time.unscaledDeltaTime, moveVector.z);
        } 
		
        // player has landed on the ground
		if (charController.isGrounded && moveVector.y < -1 && !IsFalling)
		{
            // no longer jumping
			IsJumping = false; 
            moveVector = new Vector3(moveVector.x, (-1), moveVector.z);

            // play landing sound
            if(wasJumping == true)
            {
                GameEngine.SoundManager.Play(AirmanLevelSounds.LANDING);
            }

            // no longer jumping
            wasJumping = false;
		}

        // determine if the player is falling
        if (!charController.isGrounded && moveVector.y < 0)
        {
            IsFalling = true;
        }
        else
        {
            IsFalling = false;
        }
	}
	
	//
	protected void ProcessExternalForces()
	{
		// 
		if (IsExternalForceActive == true)
		{
			moveVector += ExternalForce * Time.unscaledDeltaTime;
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
		charController.move(moveVector * Time.unscaledDeltaTime);        
	}

	//
	protected void CheckMovement()
	{
        // hold on to horizontal and vertical movement
        Vector2 currentInput = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));

        // Check Up through jump button or "up" on the keyboard.
        bool isJumpingButtonPressed = Input.GetButton("Jump");
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

            if ( !isJumpingButtonPressed && lastInputJump == true && IsFalling == false )
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
        lastInput = currentInput;
        if (Input.GetButtonUp("Jump"))
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
        IsEnteringLevel = true;
	}

	//
	public void HandleMovement()
	{
        isGrounded = charController.isGrounded;
		if (IsFrozen == true)
		{
			moveVector = Vector3.zero;
			ProcessExternalForces();
			charController.move(moveVector * Time.deltaTime);
		}
		else
		{
            if (!IsEnteringLevel)
            {
                CheckMovement();
            }

			ProcessMovement();
		}

	}

	#endregion
}

