using UnityEngine;
using System.Collections;

public class Checkpoint : MonoBehaviour
{
	#region Variables

	// Unity Editor Variables
	[SerializeField] protected Vector3 playerPosition;
	[SerializeField] protected Vector3 cameraPosition;
    [SerializeField] protected Vector3 cameraMaxPosition;
    [SerializeField] protected Vector3 cameraMinPosition;
    [SerializeField] protected bool cameraCanMoveLeft;
	[SerializeField] protected bool cameraCanMoveRight;
	[SerializeField] protected bool cameraCanMoveUp;
	[SerializeField] protected bool cameraCanMoveDown;

    // Properties
    public Vector3 PlayerPosition { get { return playerPosition; } }
    public Vector3 CameraPosition { get { return cameraPosition; } }
    public Vector3 CameraMaxPosition { get { return cameraMaxPosition; } }
    public Vector3 CameraMinPosition { get { return cameraMinPosition; } }
    public bool CanMoveRight { get { return cameraCanMoveRight; } }
    public bool CanMoveLeft { get { return cameraCanMoveLeft; } }
    public bool CanMoveUp { get { return cameraCanMoveUp; } }
    public bool CanMoveDown { get { return cameraCanMoveDown; } }
    #endregion


    #region MonoBehaviour

    // Called when the Collider other enters the trigger.
    protected void OnTriggerEnter2D(Collider2D other) 
	{
		if (other.tag == "Player")
		{
			GameEngine.Player.CheckpointPosition = playerPosition;
			LevelCamera cam = FindObjectOfType<LevelCamera>();
			cam.CheckpointPosition = cameraPosition;
			cam.CheckpointCanMoveLeft = cameraCanMoveLeft;
			cam.CheckpointCanMoveRight = cameraCanMoveRight;
			cam.CheckpointCanMoveUp = cameraCanMoveUp;
			cam.CheckpointCanMoveDown = cameraCanMoveDown;
            cam.CheckpointMinPosition = CameraMinPosition;
            cam.CheckPointMaxPosition = CameraMaxPosition;
			Destroy(this.gameObject);
		}
	}

	#endregion
}

