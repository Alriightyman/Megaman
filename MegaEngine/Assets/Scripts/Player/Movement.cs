using UnityEngine;
using System.Collections;
using Prime31;
using System;
using UnityEngine.Assertions;

[RequireComponent (typeof (CharacterController2D))]
public class Movement : MonoBehaviour
{
    #region Variables
	// Properties
	public bool IsTurningLeft { get; private set; }
	public bool IsJumping { get; private set; }
	public bool IsWalking { get; private set; }
	public bool IsHurting { get; set; }
	public bool IsFrozen  { get; set; }
	public bool IsExternalForceActive { get; set; }
    public bool IsEnteringLevel { get; set; }
    public bool IsFalling { get; set; } = false;
    public bool IsGrounded { get { return charController.isGrounded; } }
    public Vector3 ExternalForce { get; set; }
	public Vector3 CheckPointPosition { get; set; }
    

    // Downward force, originally 40f		
    [SerializeField] private float gravity = 75f;
    // Max downward speed
    [SerializeField] private float terminalVelocity = 30f;
    // Upward speed, origianlly 20f			
    [SerializeField] private float jumpSpeed = 25f;
    // Left/Right speed, originally 10f
    [SerializeField] private float moveSpeed = 8f;         		
    [SerializeField] private float landingSpeed = 1f;
    [SerializeField] private float hurtingForce = 1.0f;
    [SerializeField] private Vector3 moveVector = Vector3.zero;

    // private Instance Variables    
    private bool lastInputJump = false;
    private float verticalVelocity;
    private CharacterController2D charController;
    private bool wasJumping = false;
 
    #endregion

    #region MonoBehaviour
    // Use this for initialization
    private void Awake()
	{        
        charController = gameObject.GetComponent<CharacterController2D>();
        Assert.IsNotNull(charController);
    }
	
	// Use this for initialization
	private void Start ()
	{
        float offset = 128f;
        IsHurting = false;
        transform.position = new Vector2(CheckPointPosition.x, CheckPointPosition.y + offset);
	}
    #endregion

    #region Public Functions
    /// <summary>
    /// Resets to defaults
    /// </summary>
    public void ResetToDefaults()
    {
        IsFrozen = false;
        IsHurting = false;
        transform.position = CheckPointPosition;
        IsEnteringLevel = true;
    }

    /// <summary>
    /// Handle Player Movement
    /// </summary>
    public void HandleMovement()
    {
        // Only allow external forces to affect the player
        if (IsFrozen == true)
        {
            moveVector = Vector3.zero;
            ProcessExternalForces();
            charController.move(moveVector * Time.deltaTime);
        }
        else if (IsEnteringLevel)
        {
            // only process movment here
            ProcessMovement();
        }
        else // normal state
        {
            CheckMovement();
            ProcessMovement();
        }
    }

    #endregion


    #region private Functions

    /// <summary>
    /// Get Player input and update movement
    /// </summary>
    private void CheckMovement()
    {
        // hold on to horizontal and vertical movement
        Vector2 currentInput = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));

        // Check Jump button
        bool isJumpButtonHeld = Input.GetButton("Jump");

        // Horizontal movement...
        float deadZone = 0.01f;
        
        // set vertical velocity
        verticalVelocity = moveVector.y;
        moveVector = Vector3.zero;

        IsWalking = false;

        // check right movement and set input value
        CheckHorizontalMovement(currentInput.x, deadZone);
        
        CheckJumpInput(isJumpButtonHeld);
    }

    private void CheckHorizontalMovement(float xInput, float deadzone)
    {
        if (!IsHurting)
        {
            if (xInput > deadzone)
            {
                IsWalking = true;
                IsTurningLeft = false;
                moveVector += new Vector3(xInput, 0, 0);
            }
            else if (xInput < -deadzone)
            {
                IsWalking = true;
                IsTurningLeft = true;
                moveVector += new Vector3(xInput, 0, 0);
            }
        }
    }

    private void CheckJumpInput(bool isJumpButtonHeld)
    {
        if (!IsHurting)
        {
            // Vertical movement...
            if (isJumpButtonHeld && lastInputJump == false)
            {
                if (charController.isGrounded)
                {
                    IsJumping = true;
                    wasJumping = true;
                    verticalVelocity = jumpSpeed;
                }
            }

            if (!isJumpButtonHeld && lastInputJump == true && IsFalling == false)
            {
                verticalVelocity = 0.0f;
            }

            //If there is a collision above...
            if (charController.collisionState.above)
            {
                verticalVelocity = -5.0f;
            }
        }

        if (Input.GetButtonUp("Jump"))
            lastInputJump = false;
        else
            lastInputJump = isJumpButtonHeld;
    }

    private void ProcessMovement()
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
    private void ProcessExternalForces()
    {
        // 
        if (IsExternalForceActive == true)
        {
            moveVector += ExternalForce * Time.unscaledDeltaTime;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    private void ApplyGravity()
	{
        // drop faster if entering level from above
        var finalGravity = IsEnteringLevel ? gravity * landingSpeed * Time.unscaledDeltaTime : gravity;

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




	#endregion
	
	

}

