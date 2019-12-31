using UnityEngine;
using Extensions;
using UnityEngine.Assertions;

public class BossDoor : MonoBehaviour 
{
	#region Variables

	// Protected Instance Variables 
	[SerializeField] protected float playerSpeed = 25f;
	[SerializeField] protected float doorSpeed = 10f;

    public bool IsDoorOpen { get; set; }

    private BoxCollider2D boxCol2D;
	protected bool isOpening = false;
	protected bool isClosing = false;
	protected bool hasPlayerGoneThrough = false;
    private Vector3 startPosition;
    private Vector3 stopPosition;
    private GameObject door;

	#endregion


	#region MonoBehaviour
	
	// Use this for initialization 
	protected void Start()
	{
        boxCol2D = GetComponent<BoxCollider2D>();
        Assert.IsNotNull(boxCol2D);

        door = gameObject.GetChildWithName("Door");
        Assert.IsNotNull(door);

        startPosition = transform.position;
        stopPosition = new Vector3(startPosition.x, startPosition.y + boxCol2D.size.y, startPosition.z);

    }

	// Update is called once per frame 
	protected void Update() 
	{
		if (hasPlayerGoneThrough == true)
		{
			return;
		}
		
		if (isOpening)
		{
			MoveDoor(doorSpeed * Time.deltaTime);
			
			if (door.transform.position.y >= stopPosition.y)
			{
                door.transform.position = stopPosition;
                IsDoorOpen = true;
				isOpening = false;
				GameEngine.Player.IsExternalForceActive = true;
				GameEngine.Player.ExternalForce = new Vector3 (playerSpeed, 0.0f, 0.0f);
				GameEngine.SoundManager.Stop(AirmanLevelSounds.BOSS_DOOR);
			}
		}
		
		else if (isClosing)
		{
			MoveDoor(-doorSpeed * Time.deltaTime);

            GameEngine.Player.IsFrozen = false;
            GameEngine.Player.ExternalForce = new Vector3(0.0f, 0.0f, 0.0f);

            if (door.transform.position.y <= startPosition.y)
			{
                IsDoorOpen = false;
                door.transform.position = startPosition;
				isClosing = false;
				hasPlayerGoneThrough = true;
                GameEngine.Player.IsExternalForceActive = false;
                GameEngine.SoundManager.Stop(AirmanLevelSounds.BOSS_DOOR);
			}
		}
	}

	#endregion


	#region Protected Functions

	//
	protected void Reset()
	{
		isOpening = false;
		isClosing = false;
		hasPlayerGoneThrough = false;
        boxCol2D.enabled = true;

    }

    // Moves the oject into the spritemask so it... disappears.
    protected void MoveDoor(float speed)
	{
        door.transform.position = new Vector3(door.transform.position.x, door.transform.position.y + speed, door.transform.position.z);
    }

	#endregion
	
	
	#region Public Functions

	//
	public void OpenDoor()
	{
		GameEngine.SoundManager.Play(AirmanLevelSounds.BOSS_DOOR);
        boxCol2D.enabled = false;
		isOpening = true;
	}
	
	//
	public void CloseDoor()
	{
		GameEngine.SoundManager.Play(AirmanLevelSounds.BOSS_DOOR);
        boxCol2D.enabled = true;
		isClosing = true;
	}	
	
	#endregion
}
