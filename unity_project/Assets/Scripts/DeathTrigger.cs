using UnityEngine;
using System.Collections;

public class DeathTrigger : MonoBehaviour 
{	
	#region MonoBehaviour

	// Kill/Respawn the player when he enters the trigger.
	protected void OnTriggerEnter2D(Collider2D other) 
	{
		if (other.tag == "Player")
		{
			GameEngine.Player.KillPlayer();
			gameObject.GetComponent<Collider2D>().enabled = false;
		}
    }

	#endregion
}
