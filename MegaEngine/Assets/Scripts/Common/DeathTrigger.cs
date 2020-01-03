using UnityEngine;
using System.Collections;
using Assets.Scripts.Interfaces;

public class DeathTrigger : MonoBehaviour//, IResetable
{
    #region MonoBehaviour

    protected void Awake()
    {
    }

    // Kill/Respawn the player when he enters the trigger.
    protected void OnTriggerEnter2D(Collider2D other) 
	{
		if (other.tag == "Player")
		{
            StartCoroutine(GameEngine.Reset());
           // gameObject.GetComponent<Collider2D>().enabled = false;
		}
    }

    #endregion
}
