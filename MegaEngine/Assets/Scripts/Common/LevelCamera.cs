using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.Assertions;
using System;

public class LevelCamera : MonoBehaviour 
{
    private static LevelCamera instance = null;

	#region Variables

	// Public Properties
	public Vector3 CameraPosition   { get{return transform.position;} set{transform.position = value;} }
	public Vector3 CheckpointPosition   { get; set; }
	public bool CheckpointCanMoveLeft 	{ get; set; }
	public bool CheckpointCanMoveRight 	{ get; set; }
	public bool CheckpointCanMoveUp { get; set; }
	public bool CheckpointCanMoveDown   { get; set; }
    public bool CanMoveLeft	    { get; set; }
    public bool CanMoveRight    { get; set; }
    public bool CanMoveUp		{ get; set; }
    public bool CanMoveDown		{ get; set; }
    public bool IsTransitioning	{ get; set; }
    public bool ShouldStayStill	{ get; set; }
    public Vector3 CheckPointMaxPosition { get; set; }
    public Vector3 CheckpointMinPosition { get; set; }

    public Vector3 MaxPosition;
    public Vector3 MinPosition;

    // Private Instance Variables
    private Vector3 playerCheckpointPosition;
	private Vector3 playerPos;
	private Vector3 deltaPos;
    private BoxCollider2D boxCol2D;

    [SerializeField] private Checkpoint startPosition = null;
    #endregion


    #region MonoBehaviour
    protected void Awake()
    {
        Assert.IsNotNull(startPosition);
        DontDestroyOnLoad(gameObject);

        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    // Use this for initialization
    protected void Start () 
	{
        Vector3 position = startPosition.CameraPosition;
        playerCheckpointPosition = startPosition.PlayerPosition;

        GameEngine.Player.CheckpointPosition = startPosition.PlayerPosition;

        Debug.Log(String.Format("Player Position: {0}", GameEngine.Player.CheckpointPosition));

		//Vector3 startPosition = new Vector3(13.34303f, 11.51588f, -10f);
		transform.position = new Vector3(position.x, position.y, -10f); 
		CheckpointPosition = new Vector3(position.x, position.y, -10f);

        MinPosition = startPosition.CameraMinPosition;
        MaxPosition = startPosition.CameraMaxPosition;
        CheckpointMinPosition = MinPosition;
        CheckPointMaxPosition = MaxPosition;

        ShouldStayStill = false;
		IsTransitioning = false;
		CanMoveRight = startPosition.CanMoveRight;
		CanMoveLeft = startPosition.CanMoveLeft;
		CanMoveUp = startPosition.CanMoveUp;
		CanMoveDown = startPosition.CanMoveDown;
		CheckpointCanMoveLeft = false;
		CheckpointCanMoveRight = true;
		CheckpointCanMoveUp = false;
		CheckpointCanMoveDown = false;
	}
	
	// Update is called once per frame
	protected void Update () 
	{
		// If the camera is transitioning between parts of the scene...
		if (IsTransitioning == true || ShouldStayStill == true)
		{
			return;
		}
		
		// Make the camera follow the player...
		playerPos = GameEngine.Player.transform.position;
		deltaPos = playerPos - transform.position;
        var newPosition = transform.position;
        

		// Check the x pos 
		if ( (deltaPos.x < 0.0f && CanMoveLeft) 
            || (deltaPos.x > 0.0f && CanMoveRight))	
		{
            newPosition = new Vector3(playerPos.x, newPosition.y, newPosition.z);
		}
		
		// Check the y pos 
		if ((deltaPos.y < 0.0f && CanMoveDown) || (deltaPos.y > 0.0f && CanMoveUp)) 		
		{
            newPosition = new Vector3(newPosition.x, playerPos.y, newPosition.z);
		}
		
        // check if camera has gone too far
        if(newPosition.x >= MaxPosition.x)
        {
            newPosition = new Vector3(MaxPosition.x, newPosition.y, newPosition.z);
        }

        if (newPosition.x <= MinPosition.x)
        {
            newPosition = new Vector3(MinPosition.x, newPosition.y, newPosition.z);
        }

        if (newPosition.y >= MaxPosition.y)
        {
            newPosition = new Vector3(newPosition.x, MaxPosition.y, newPosition.z);
        }

        if (newPosition.y <= MinPosition.y)
        {
            newPosition = new Vector3(newPosition.x, MinPosition.y, newPosition.z);
        }


        transform.position = newPosition;

        // Make the level restart if the user presses escape...
        if (Input.GetKeyDown (KeyCode.Escape)) 
		{
            SceneManager.LoadScene(0);
        } 

	}

	#endregion


	#region Public Functions

	// 
	public void Reset()
	{
		ShouldStayStill = false;
		IsTransitioning = false;
		transform.position = CheckpointPosition;
		CanMoveLeft = CheckpointCanMoveLeft;
		CanMoveRight = CheckpointCanMoveRight;
		CanMoveUp = CheckpointCanMoveUp;
		CanMoveDown = CheckpointCanMoveDown;
        MinPosition = CheckpointMinPosition;
        MaxPosition = CheckPointMaxPosition;
        GameEngine.Player.transform.position = playerCheckpointPosition;
        GameEngine.Player.CheckpointPosition = playerCheckpointPosition;
        Debug.Log(String.Format("Player Position: {0}", GameEngine.Player.CheckpointPosition));

    }
    #endregion
}
