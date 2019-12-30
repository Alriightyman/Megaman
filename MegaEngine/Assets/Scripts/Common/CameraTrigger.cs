using UnityEngine;
using UnityEngine.Assertions;
using System.Collections;

public class CameraTrigger : MonoBehaviour
{
	#region Variables

	// Unity Editor Variables
	[SerializeField] protected bool isABossDoorTrigger;
	[SerializeField] protected BossDoor bossDoor;
	[SerializeField] protected bool onEnterCanMoveLeft;
	[SerializeField] protected bool onExitCanMoveLeft;
	[SerializeField] protected bool onEnterCanMoveRight;
	[SerializeField] protected bool onExitCanMoveRight;
	[SerializeField] protected bool onEnterCanMoveUp;
	[SerializeField] protected bool onExitCanMoveUp;
	[SerializeField] protected bool onEnterCanMoveDown;
	[SerializeField] protected bool onExitCanMoveDown;
	[SerializeField] protected bool shouldMoveCamera;
	[SerializeField] protected float transitionDuration;
	[SerializeField] protected Vector3 freezeEndPosition;
    [SerializeField] private Vector3 NewCameraMaxPosition;
    [SerializeField] private Vector3 NewCameraMinPosition;

	// Protected Instance Variables
	protected LevelCamera levelCamera;
	protected float transitionStatus = 0.0f;
	protected bool isTransitioning = false;
	protected Vector3 startPosition;
	protected float startTime;

	#endregion


	#region MonoBehaviour

	// Constructor
	protected void Awake()
	{
		levelCamera = FindObjectOfType<LevelCamera>();
		Assert.IsNotNull(levelCamera);
	}
	
	// Update is called once per frame
	protected void Update ()
	{
		if (isTransitioning == true)
		{
			transitionStatus = (Time.time - startTime) / transitionDuration;
			levelCamera.CameraPosition = Vector3.Lerp(startPosition, freezeEndPosition, transitionStatus);
			
			if (transitionStatus  >= 1.0)
			{
                GameEngine.Player.IsFrozen = false;
				isTransitioning = false;
				levelCamera.IsTransitioning = false;
				levelCamera.CameraPosition = freezeEndPosition;
				levelCamera.CanMoveLeft = onExitCanMoveLeft;
				levelCamera.CanMoveRight = onExitCanMoveRight;
				levelCamera.CanMoveUp = onExitCanMoveUp;
				levelCamera.CanMoveDown = onExitCanMoveDown;
                levelCamera.MinPosition = NewCameraMinPosition;
                levelCamera.MaxPosition = NewCameraMaxPosition;
			}
		}
	}

	// Called when the Collider other enters the trigger.
	protected void OnTriggerEnter2D(Collider2D other) 
	{
		if (other.tag == "Player")
		{
			levelCamera.CanMoveLeft = onEnterCanMoveLeft;
			levelCamera.CanMoveRight = onEnterCanMoveRight;
			levelCamera.CanMoveUp = onEnterCanMoveUp;
			levelCamera.CanMoveDown = onEnterCanMoveDown;
			
            if(!onEnterCanMoveRight)
            {
                levelCamera.MaxPosition = levelCamera.transform.position;
            }

			if (isABossDoorTrigger)
			{
				bossDoor.OpenDoor();
				GameEngine.Player.IsFrozen = true;
				levelCamera.IsTransitioning = true;
			}
			
			if (shouldMoveCamera == true)
			{
                GameEngine.Player.IsFrozen = true;
				startPosition = levelCamera.CameraPosition;
				isTransitioning = true;
				startTime = Time.time;
				levelCamera.IsTransitioning = true;
               
			}
		}
    }
	
	// 
	protected void OnTriggerExit2D(Collider2D other) 
	{
		if (other.tag == "Player")
		{
			levelCamera.CanMoveLeft = onExitCanMoveLeft;
			levelCamera.CanMoveRight = onExitCanMoveRight;
			levelCamera.CanMoveUp = onExitCanMoveUp;
			levelCamera.CanMoveDown = onExitCanMoveDown;


            levelCamera.MaxPosition = NewCameraMaxPosition;
            levelCamera.MinPosition = NewCameraMinPosition;


            if (isABossDoorTrigger)
			{
				GameEngine.Player.IsFrozen = false;
				bossDoor.CloseDoor();
				GetComponent<Collider2D>().enabled = false;
			}
		}
    }
	
	#endregion
}

