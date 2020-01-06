using UnityEngine;
using UnityEngine.Assertions;
using System.Collections;

public class CameraTrigger : MonoBehaviour
{
	#region Variables

	// Unity Editor Variables
	[SerializeField] private bool isABossDoorTrigger;
	[SerializeField] private BossDoor bossDoor;
	[SerializeField] private bool onEnterCanMoveLeft;
	[SerializeField] private bool onExitCanMoveLeft;
	[SerializeField] private bool onEnterCanMoveRight;
	[SerializeField] private bool onExitCanMoveRight;
	[SerializeField] private bool onEnterCanMoveUp;
	[SerializeField] private bool onExitCanMoveUp;
	[SerializeField] private bool onEnterCanMoveDown;
	[SerializeField] private bool onExitCanMoveDown;
	[SerializeField] private bool shouldMoveCamera;
	[SerializeField] private float transitionDuration;
	[SerializeField] private Vector3 freezeEndPosition;
    [SerializeField] private Vector3 NewCameraMaxPosition;
    [SerializeField] private Vector3 NewCameraMinPosition;

	// private Instance Variables
	private LevelCamera levelCamera;
	private float transitionStatus = 0.0f;
	private bool isTransitioning = false;
	private Vector3 startPosition;
	private float startTime;
    private bool isStartTimeSet = false;
	#endregion


	#region MonoBehaviour

	// Constructor
	private void Awake()
	{
		levelCamera = FindObjectOfType<LevelCamera>();
		Assert.IsNotNull(levelCamera);
	}
	
	// Update is called once per frame
	private void Update ()
	{
		if (isTransitioning == true)
		{

            transitionStatus = (Time.time - startTime) / transitionDuration;
            levelCamera.CameraPosition = Vector3.Lerp(startPosition, freezeEndPosition, transitionStatus);

            if (transitionStatus >= 1.0)
            {
                if (!isABossDoorTrigger)
                {
                    GameEngine.Player.IsFrozen = false;
                }

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
	private void OnTriggerEnter2D(Collider2D other) 
	{
		if (other.tag == "Player")
		{
			levelCamera.CanMoveLeft = onEnterCanMoveLeft;
			levelCamera.CanMoveRight = onEnterCanMoveRight;
			levelCamera.CanMoveUp = onEnterCanMoveUp;
			levelCamera.CanMoveDown = onEnterCanMoveDown;

            if (shouldMoveCamera == true)
            {
                GameEngine.Player.IsFrozen = true;
                startPosition = levelCamera.CameraPosition;
                isTransitioning = true;
                startTime = Time.time;
                levelCamera.IsTransitioning = true;
            }

            if (isABossDoorTrigger)
			{
				bossDoor.OpenDoor();
                GameEngine.Player.CanShoot = false;
				GameEngine.Player.IsFrozen = true;
				levelCamera.IsTransitioning = true;
                isTransitioning = false;
                StartCoroutine("DoorShut");
			}
			

		}
    }
	
	// 
	private void OnTriggerExit2D(Collider2D other) 
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
				bossDoor.CloseDoor();
				GetComponent<Collider2D>().enabled = false;
			}
		}
    }

    private IEnumerator DoorShut()
    {
        yield return new WaitUntil(new System.Func<bool>(() => { return bossDoor.IsDoorOpen == true; }));
        startTime = Time.time;
        isTransitioning = true;
    }

    #endregion



    #region Gizmos
    private void OnDrawGizmos()
    {
        
        //var boxCol2D = GetComponent<BoxCollider2D>();
        //// 
        //// draw an icon for all directions available
        //if (onEnterCanMoveUp)
        //{
        //    Gizmos.DrawLine(transform.position, new Vector3(transform.position.x, transform.position.y + boxCol2D.size.y / 2, transform.position.z));
        //    Gizmos.DrawLine(transform.position, new Vector3(transform.position.x, transform.position.y - boxCol2D.size.y / 2, transform.position.z));

        //    Gizmos.DrawIcon(new Vector3(transform.position.x, transform.position.y + boxCol2D.size.y, transform.position.z)
        //                    , "ArrowUp.png", true);
        //}
        //if (onEnterCanMoveDown)
        //{
        //    Gizmos.DrawLine(transform.position, new Vector3(transform.position.x, transform.position.y + boxCol2D.size.y / 2, transform.position.z));
        //    Gizmos.DrawLine(transform.position, new Vector3(transform.position.x, transform.position.y - boxCol2D.size.y / 2, transform.position.z));

        //    Gizmos.DrawIcon(new Vector3(transform.position.x, transform.position.y - boxCol2D.size.y, transform.position.z)
        //                    , "ArrowDown.png", true);
        //}
        //if (onEnterCanMoveLeft)
        //{
        //    Gizmos.DrawLine(transform.position, new Vector3(transform.position.x + boxCol2D.size.x / 2, transform.position.y, transform.position.z));
        //    Gizmos.DrawLine(transform.position, new Vector3(transform.position.x - boxCol2D.size.x / 2, transform.position.y, transform.position.z));

        //    Gizmos.DrawIcon(new Vector3(transform.position.x - boxCol2D.size.x, transform.position.y, transform.position.z)
        //                    , "ArrowLeft.png", true);
        //}
        //if (onEnterCanMoveRight)
        //{
        //    Gizmos.DrawLine(transform.position, new Vector3(transform.position.x + boxCol2D.size.x / 2, transform.position.y, transform.position.z));
        //    Gizmos.DrawLine(transform.position, new Vector3(transform.position.x - boxCol2D.size.x / 2, transform.position.y, transform.position.z));
        //    Gizmos.DrawIcon(new Vector3(transform.position.x + boxCol2D.size.x, transform.position.y, transform.position.z)
        //                    , "ArrowRight.png",true);
        //}
    }
    #endregion
}

