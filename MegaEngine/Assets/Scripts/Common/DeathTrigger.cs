using UnityEngine;
using System.Collections;
using Assets.Scripts.Interfaces;

public class DeathTrigger : MonoBehaviour//, IResetable
{
    #region MonoBehaviour

    private void Awake()
    {
    }

    // Kill/Respawn the player when he enters the trigger.
    private void OnTriggerEnter2D(Collider2D other) 
	{
		if (other.tag == "Player")
		{
            StartCoroutine(GameEngine.Restart());
		}
    }

    #endregion
}
